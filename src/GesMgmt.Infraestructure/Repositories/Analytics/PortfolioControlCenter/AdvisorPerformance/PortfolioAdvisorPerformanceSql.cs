using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence.Analytics;
using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal static class PortfolioAdvisorPerformanceSql
{
    public const string AdvisorPerformance = """
        DECLARE @ClientKey INT =
        (
            SELECT c.client_key
            FROM analytics.dim_client AS c
            WHERE c.crm_client_id = @CrmClientId
        );

        IF @BusinessUnit IS NULL
           AND @ClientKey IS NOT NULL
           AND
           (
               SELECT COUNT_BIG(*)
               FROM
               (
                   SELECT DISTINCT
                       CASE
                           WHEN NULLIF(
                               LTRIM(RTRIM(scope_portfolio.source_business_unit)),
                               '') IS NULL THEN CONVERT(BIT, 0)
                           ELSE CONVERT(BIT, 1)
                       END AS HasBusinessUnit,
                       NULLIF(
                           LTRIM(RTRIM(scope_portfolio.source_business_unit)),
                           '') AS BusinessUnitCode
                   FROM analytics.fact_portfolio_daily AS scope_fact
                   INNER JOIN analytics.dim_portfolio AS scope_portfolio
                       ON scope_portfolio.portfolio_key = scope_fact.portfolio_key
                   WHERE scope_fact.client_key = @ClientKey
               ) AS available_business_unit_scope
           ) > 1
        BEGIN
            /* pcc:business-unit:unscoped-ambiguity-guard */
            SET @ClientKey = NULL;
        END;

        SELECT @ClientKey AS ClientKey
        WHERE @ClientKey IS NOT NULL
          AND
          (
              @BusinessUnit IS NULL
              OR EXISTS
              (
                  SELECT 1
                  FROM analytics.fact_portfolio_daily AS business_unit_fact
                  INNER JOIN analytics.dim_portfolio AS business_unit_portfolio
                      ON business_unit_portfolio.portfolio_key = business_unit_fact.portfolio_key
                  WHERE business_unit_fact.client_key = @ClientKey
                    AND business_unit_portfolio.source_business_unit = @BusinessUnit
              )
          );

        /* pcc:advisor-performance */
        WITH CampaignScope AS
        (
            SELECT c.campaign_key
            FROM analytics.dim_campaign AS c
            WHERE c.client_key = @ClientKey
              AND (@CampaignCode IS NULL OR c.campaign_code = CONVERT(VARCHAR(20), @CampaignCode))
        ),
        FilteredActivity AS
        (
            SELECT
                a.calendar_date,
                a.advisor_key,
                a.advisor_name,
                a.management_events,
                a.recovered_amount,
                a.loaded_at
            FROM analytics.v_supervisor_advisor_daily_attribution AS a
            INNER JOIN CampaignScope AS cs
                ON cs.campaign_key = a.campaign_key
            WHERE a.client_key = @ClientKey
              AND (@SubPortfolioId IS NULL OR a.portfolio_key = @SubPortfolioId)
              AND (
                  @BusinessUnit IS NULL
                  OR EXISTS
                  (
                      SELECT 1
                      FROM analytics.dim_portfolio AS business_unit_portfolio
                      WHERE business_unit_portfolio.portfolio_key = a.portfolio_key
                        AND business_unit_portfolio.source_business_unit = @BusinessUnit
                  )
              )
              AND (@SupervisorId IS NULL OR a.supervisor_key = @SupervisorId)
              AND (@DateFrom IS NULL OR a.calendar_date >= @DateFrom)
              AND (@DateTo IS NULL OR a.calendar_date <= @DateTo)
        ),
        ManagementMetrics AS
        (
            SELECT
                advisor_key,
                MAX(advisor_name) AS advisor_name,
                MIN(calendar_date) AS date_from,
                MAX(calendar_date) AS date_to,
                SUM(CONVERT(BIGINT, management_events)) AS management_count,
                SUM(CONVERT(DECIMAL(19,4), recovered_amount)) AS attributable_recovered_amount,
                MAX(loaded_at) AS loaded_at
            FROM FilteredActivity
            GROUP BY advisor_key
        ),
        EffectiveRange AS
        (
            SELECT
                MIN(date_from) AS date_from,
                MAX(date_to) AS date_to
            FROM ManagementMetrics
        ),
        ContactPerDebtor AS
        (
            SELECT
                c.advisor_key,
                c.campaign_key,
                c.portfolio_key,
                c.source_debtor_id,
                MAX(CONVERT(TINYINT, c.had_direct_contact)) AS had_direct_contact,
                MAX(CONVERT(TINYINT, c.had_indirect_contact)) AS had_indirect_contact,
                MAX(CONVERT(TINYINT, c.had_no_contact)) AS had_no_contact,
                MAX(c.loaded_at) AS loaded_at
            FROM analytics.v_supervisor_debtor_contact_daily AS c
            INNER JOIN CampaignScope AS cs
                ON cs.campaign_key = c.campaign_key
            CROSS JOIN EffectiveRange AS er
            WHERE c.client_key = @ClientKey
              AND (@SubPortfolioId IS NULL OR c.portfolio_key = @SubPortfolioId)
              AND (
                  @BusinessUnit IS NULL
                  OR EXISTS
                  (
                      SELECT 1
                      FROM analytics.dim_portfolio AS business_unit_portfolio
                      WHERE business_unit_portfolio.portfolio_key = c.portfolio_key
                        AND business_unit_portfolio.source_business_unit = @BusinessUnit
                  )
              )
              AND (@SupervisorId IS NULL OR c.supervisor_key = @SupervisorId)
              AND er.date_from IS NOT NULL
              AND c.calendar_date >= er.date_from
              AND c.calendar_date <= er.date_to
            GROUP BY
                c.advisor_key,
                c.campaign_key,
                c.portfolio_key,
                c.source_debtor_id
        ),
        ContactMetrics AS
        (
            SELECT
                advisor_key,
                SUM(CASE WHEN had_direct_contact = 1 THEN 1 ELSE 0 END)
                    AS direct_contact_clients,
                SUM(
                    CASE
                        WHEN had_direct_contact = 1
                          OR had_indirect_contact = 1
                          OR had_no_contact = 1
                            THEN 1
                        ELSE 0
                    END
                ) AS classifiable_clients,
                MAX(loaded_at) AS loaded_at
            FROM ContactPerDebtor
            GROUP BY advisor_key
        ),
        PromisePerDebtor AS
        (
            SELECT
                p.advisor_key,
                p.campaign_key,
                p.portfolio_key,
                p.source_debtor_id,
                COUNT_BIG(p.promise_fact_key) AS promise_count,
                MAX(p.loaded_at) AS loaded_at
            FROM analytics.v_supervisor_promise_operational AS p
            INNER JOIN CampaignScope AS cs
                ON cs.campaign_key = p.campaign_key
            CROSS JOIN EffectiveRange AS er
            WHERE p.client_key = @ClientKey
              AND p.advisor_key IS NOT NULL
              AND p.is_valid_promise = 1
              AND (@SubPortfolioId IS NULL OR p.portfolio_key = @SubPortfolioId)
              AND (
                  @BusinessUnit IS NULL
                  OR EXISTS
                  (
                      SELECT 1
                      FROM analytics.dim_portfolio AS business_unit_portfolio
                      WHERE business_unit_portfolio.portfolio_key = p.portfolio_key
                        AND business_unit_portfolio.source_business_unit = @BusinessUnit
                  )
              )
              AND (@SupervisorId IS NULL OR p.supervisor_key = @SupervisorId)
              AND er.date_from IS NOT NULL
              AND p.management_at >= er.date_from
              AND p.management_at < DATEADD(DAY, 1, CONVERT(DATETIME2, er.date_to))
            GROUP BY
                p.advisor_key,
                p.campaign_key,
                p.portfolio_key,
                p.source_debtor_id
        ),
        PromiseMetrics AS
        (
            SELECT
                advisor_key,
                COALESCE(SUM(promise_count), 0) AS promise_count,
                COUNT_BIG(*) AS valid_promise_clients,
                MAX(loaded_at) AS loaded_at
            FROM PromisePerDebtor
            GROUP BY advisor_key
        ),
        PayerDebtors AS
        (
            SELECT
                p.advisor_key,
                p.campaign_key,
                p.portfolio_key,
                p.source_debtor_id,
                MAX(p.loaded_at) AS loaded_at
            FROM analytics.v_supervisor_debtor_payment_daily AS p
            INNER JOIN CampaignScope AS cs
                ON cs.campaign_key = p.campaign_key
            CROSS JOIN EffectiveRange AS er
            WHERE p.client_key = @ClientKey
              AND (@SubPortfolioId IS NULL OR p.portfolio_key = @SubPortfolioId)
              AND (
                  @BusinessUnit IS NULL
                  OR EXISTS
                  (
                      SELECT 1
                      FROM analytics.dim_portfolio AS business_unit_portfolio
                      WHERE business_unit_portfolio.portfolio_key = p.portfolio_key
                        AND business_unit_portfolio.source_business_unit = @BusinessUnit
                  )
              )
              AND (@SupervisorId IS NULL OR p.supervisor_key = @SupervisorId)
              AND er.date_from IS NOT NULL
              AND p.calendar_date >= er.date_from
              AND p.calendar_date <= er.date_to
            GROUP BY
                p.advisor_key,
                p.campaign_key,
                p.portfolio_key,
                p.source_debtor_id
        ),
        PaymentMetrics AS
        (
            SELECT
                advisor_key,
                COUNT_BIG(*) AS payment_count,
                MAX(loaded_at) AS loaded_at
            FROM PayerDebtors
            GROUP BY advisor_key
        ),
        CurrentSupervisor AS
        (
            SELECT
                a.advisor_key,
                a.supervisor_key,
                a.supervisor_name
            FROM analytics.v_advisor_supervisor_current AS a
            WHERE a.client_key = @ClientKey
        )
        SELECT
            m.advisor_key AS AdvisorId,
            m.advisor_name AS AdvisorName,
            cs.supervisor_key AS CurrentSupervisorId,
            cs.supervisor_name AS CurrentSupervisorName,
            er.date_from AS DateFrom,
            er.date_to AS DateTo,
            CONVERT(BIGINT, ISNULL(m.management_count, 0)) AS ManagementCount,
            CAST(
                1.0 * ISNULL(c.direct_contact_clients, 0)
                / NULLIF(c.classifiable_clients, 0)
                AS DECIMAL(18,6)
            ) AS RpcRate,
            CAST(
                1.0 * ISNULL(pm.valid_promise_clients, 0)
                / NULLIF(c.direct_contact_clients, 0)
                AS DECIMAL(18,6)
            ) AS CloseRate,
            CONVERT(BIGINT, ISNULL(pm.promise_count, 0)) AS PromiseCount,
            CONVERT(BIGINT, ISNULL(pay.payment_count, 0)) AS PaymentCount,
            CONVERT(
                DECIMAL(19,4),
                ISNULL(m.attributable_recovered_amount, 0)
            ) AS AttributableRecoveredAmount,
            updated_at.UpdatedAtUtc
        FROM ManagementMetrics AS m
        CROSS JOIN EffectiveRange AS er
        LEFT JOIN ContactMetrics AS c
            ON c.advisor_key = m.advisor_key
        LEFT JOIN PromiseMetrics AS pm
            ON pm.advisor_key = m.advisor_key
        LEFT JOIN PaymentMetrics AS pay
            ON pay.advisor_key = m.advisor_key
        LEFT JOIN CurrentSupervisor AS cs
            ON cs.advisor_key = m.advisor_key
        CROSS APPLY
        (
            SELECT MAX(v.loaded_at) AS UpdatedAtUtc
            FROM
            (
                VALUES
                    (m.loaded_at),
                    (c.loaded_at),
                    (pm.loaded_at),
                    (pay.loaded_at)
            ) AS v(loaded_at)
        ) AS updated_at
        ORDER BY m.advisor_name, m.advisor_key;
        """;
}
