using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence.Analytics;
using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal static class PortfolioCampaignPerformanceSql
{
    public const string CampaignPerformance = """
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

        WITH /* pcc:campaign-performance */ SelectedCampaigns AS
        (
            SELECT
                cp.campaign_key,
                cp.campaign_code,
                cp.campaign_name,
                MIN(d.calendar_date) AS available_date_from,
                MAX(d.calendar_date) AS available_date_to
            FROM analytics.dim_campaign AS cp
            INNER JOIN analytics.fact_portfolio_daily AS f
                ON f.client_key = cp.client_key
               AND f.campaign_key = cp.campaign_key
            INNER JOIN analytics.dim_date AS d
                ON d.date_key = f.date_key
            WHERE cp.client_key = @ClientKey
              AND (@CampaignCode IS NULL OR cp.campaign_code = @CampaignCode)
              AND (@SubPortfolioId IS NULL OR f.portfolio_key = @SubPortfolioId)
              AND (
                  @BusinessUnit IS NULL
                  OR EXISTS
                  (
                      SELECT 1
                      FROM analytics.dim_portfolio AS business_unit_portfolio
                      WHERE business_unit_portfolio.portfolio_key = f.portfolio_key
                        AND business_unit_portfolio.source_business_unit = @BusinessUnit
                  )
              )
            GROUP BY
                cp.campaign_key,
                cp.campaign_code,
                cp.campaign_name
        ),
        CampaignRanges AS
        (
            SELECT
                sc.campaign_key,
                sc.campaign_code,
                sc.campaign_name,
                CASE
                    WHEN @DateFrom IS NULL
                      OR @DateFrom < sc.available_date_from
                        THEN sc.available_date_from
                    ELSE @DateFrom
                END AS date_from,
                CASE
                    WHEN @DateTo IS NULL
                      OR @DateTo > sc.available_date_to
                        THEN sc.available_date_to
                    ELSE @DateTo
                END AS date_to
            FROM SelectedCampaigns AS sc
        ),
        EligibleCampaigns AS
        (
            SELECT
                campaign_key,
                campaign_code,
                campaign_name,
                date_from,
                date_to
            FROM CampaignRanges
            WHERE date_from <= date_to
        ),
        SnapshotDates AS
        (
            SELECT
                r.campaign_key,
                MAX(v.calendar_date) AS snapshot_date
            FROM EligibleCampaigns AS r
            INNER JOIN analytics.v_portfolio_daily_metrics AS v
                ON v.client_key = @ClientKey
               AND v.campaign_key = r.campaign_key
               AND v.has_source_snapshot = 1
               AND v.calendar_date >= r.date_from
               AND v.calendar_date <= r.date_to
               AND (@SubPortfolioId IS NULL OR v.portfolio_key = @SubPortfolioId)
               AND (
                   @BusinessUnit IS NULL
                   OR EXISTS
                   (
                       SELECT 1
                       FROM analytics.dim_portfolio AS business_unit_portfolio
                       WHERE business_unit_portfolio.portfolio_key = v.portfolio_key
                         AND business_unit_portfolio.source_business_unit = @BusinessUnit
                   )
               )
            GROUP BY r.campaign_key
        ),
        SnapshotMetrics AS
        (
            SELECT
                r.campaign_key,
                sd.snapshot_date,
                COALESCE(SUM(v.assigned_clients_snapshot), 0) AS assigned_clients,
                COALESCE(SUM(v.managed_clients_snapshot), 0) AS managed_clients,
                COALESCE(SUM(v.pending_clients_snapshot), 0) AS pending_clients,
                COALESCE(SUM(v.contacted_clients_snapshot), 0) AS contacted_clients,
                MAX(v.loaded_at) AS loaded_at
            FROM EligibleCampaigns AS r
            INNER JOIN SnapshotDates AS sd
                ON sd.campaign_key = r.campaign_key
            INNER JOIN analytics.v_portfolio_daily_metrics AS v
                ON v.client_key = @ClientKey
               AND v.campaign_key = r.campaign_key
               AND v.has_source_snapshot = 1
               AND v.calendar_date = sd.snapshot_date
               AND (@SubPortfolioId IS NULL OR v.portfolio_key = @SubPortfolioId)
               AND (
                   @BusinessUnit IS NULL
                   OR EXISTS
                   (
                       SELECT 1
                       FROM analytics.dim_portfolio AS business_unit_portfolio
                       WHERE business_unit_portfolio.portfolio_key = v.portfolio_key
                         AND business_unit_portfolio.source_business_unit = @BusinessUnit
                   )
               )
            GROUP BY
                r.campaign_key,
                sd.snapshot_date
        ),
        FlowMetrics AS
        (
            SELECT
                r.campaign_key,
                COALESCE(SUM(v.management_events_day), 0) AS management_count,
                COALESCE(SUM(v.recovered_amount_day), 0) AS recovered_amount,
                MAX(v.loaded_at) AS loaded_at
            FROM EligibleCampaigns AS r
            LEFT JOIN analytics.v_portfolio_daily_metrics AS v
                ON v.client_key = @ClientKey
               AND v.campaign_key = r.campaign_key
               AND v.calendar_date >= r.date_from
               AND v.calendar_date <= r.date_to
               AND (@SubPortfolioId IS NULL OR v.portfolio_key = @SubPortfolioId)
               AND (
                   @BusinessUnit IS NULL
                   OR EXISTS
                   (
                       SELECT 1
                       FROM analytics.dim_portfolio AS business_unit_portfolio
                       WHERE business_unit_portfolio.portfolio_key = v.portfolio_key
                         AND business_unit_portfolio.source_business_unit = @BusinessUnit
                   )
               )
            GROUP BY r.campaign_key
        ),
        ContactPerDebtor AS
        (
            SELECT
                r.campaign_key,
                f.portfolio_key,
                f.source_debtor_id,
                MAX(CONVERT(TINYINT, f.had_direct_contact)) AS had_direct_contact,
                MAX(CONVERT(TINYINT, f.had_indirect_contact)) AS had_indirect_contact,
                MAX(CONVERT(TINYINT, f.had_no_contact)) AS had_no_contact,
                MAX(f.loaded_at) AS loaded_at
            FROM EligibleCampaigns AS r
            INNER JOIN analytics.fact_debtor_contact_daily AS f
                ON f.client_key = @ClientKey
               AND f.campaign_key = r.campaign_key
               AND (@SubPortfolioId IS NULL OR f.portfolio_key = @SubPortfolioId)
               AND (
                   @BusinessUnit IS NULL
                   OR EXISTS
                   (
                       SELECT 1
                       FROM analytics.dim_portfolio AS business_unit_portfolio
                       WHERE business_unit_portfolio.portfolio_key = f.portfolio_key
                         AND business_unit_portfolio.source_business_unit = @BusinessUnit
                   )
               )
            INNER JOIN analytics.dim_date AS d
                ON d.date_key = f.date_key
               AND d.calendar_date >= r.date_from
               AND d.calendar_date <= r.date_to
            GROUP BY
                r.campaign_key,
                f.portfolio_key,
                f.source_debtor_id
        ),
        ContactMetrics AS
        (
            SELECT
                r.campaign_key,
                COALESCE(SUM(
                    CASE WHEN c.had_direct_contact = 1 THEN 1 ELSE 0 END
                ), 0) AS direct_contact_clients,
                COALESCE(SUM(
                    CASE
                        WHEN c.had_direct_contact = 1
                          OR c.had_indirect_contact = 1
                          OR c.had_no_contact = 1
                            THEN 1
                        ELSE 0
                    END
                ), 0) AS classifiable_clients,
                MAX(c.loaded_at) AS loaded_at
            FROM EligibleCampaigns AS r
            LEFT JOIN ContactPerDebtor AS c
                ON c.campaign_key = r.campaign_key
            GROUP BY r.campaign_key
        ),
        PromisePerDebtor AS
        (
            SELECT
                r.campaign_key,
                p.portfolio_key,
                p.source_debtor_id,
                COUNT_BIG(p.source_debtor_id) AS promise_count,
                COALESCE(SUM(
                    CASE
                        WHEN p.status_code IN
                        (
                            'FULFILLED',
                            'PARTIAL',
                            'FULFILLED_OUT_OF_RANGE'
                        )
                            THEN p.paid_amount
                        ELSE 0
                    END
                ), 0) AS fulfillment_paid_amount,
                COALESCE(SUM(
                    CASE
                        WHEN p.status_code IN
                        (
                            'FULFILLED',
                            'PARTIAL',
                            'FULFILLED_OUT_OF_RANGE',
                            'BROKEN'
                        )
                            THEN p.promise_amount
                        ELSE 0
                    END
                ), 0) AS fulfillment_promise_amount,
                MAX(p.loaded_at) AS loaded_at
            FROM EligibleCampaigns AS r
            INNER JOIN analytics.fact_promise AS p
                ON p.client_key = @ClientKey
               AND p.campaign_key = r.campaign_key
               AND p.is_valid_promise = 1
               AND p.management_at >= r.date_from
               AND p.management_at < DATEADD(DAY, 1, CONVERT(DATETIME2, r.date_to))
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
            GROUP BY
                r.campaign_key,
                p.portfolio_key,
                p.source_debtor_id
        ),
        PromiseMetrics AS
        (
            SELECT
                r.campaign_key,
                COALESCE(SUM(p.promise_count), 0) AS promise_count,
                COUNT_BIG(p.source_debtor_id) AS valid_promise_clients,
                COALESCE(SUM(p.fulfillment_paid_amount), 0) AS fulfillment_paid_amount,
                COALESCE(SUM(p.fulfillment_promise_amount), 0) AS fulfillment_promise_amount,
                MAX(p.loaded_at) AS loaded_at
            FROM EligibleCampaigns AS r
            LEFT JOIN PromisePerDebtor AS p
                ON p.campaign_key = r.campaign_key
            GROUP BY r.campaign_key
        ),
        PaymentDebtors AS
        (
            SELECT
                r.campaign_key,
                f.portfolio_key,
                f.source_debtor_id,
                MAX(f.loaded_at) AS loaded_at
            FROM EligibleCampaigns AS r
            INNER JOIN analytics.fact_debtor_payment_daily AS f
                ON f.client_key = @ClientKey
               AND f.campaign_key = r.campaign_key
               AND (@SubPortfolioId IS NULL OR f.portfolio_key = @SubPortfolioId)
               AND (
                   @BusinessUnit IS NULL
                   OR EXISTS
                   (
                       SELECT 1
                       FROM analytics.dim_portfolio AS business_unit_portfolio
                       WHERE business_unit_portfolio.portfolio_key = f.portfolio_key
                         AND business_unit_portfolio.source_business_unit = @BusinessUnit
                   )
               )
            INNER JOIN analytics.dim_date AS d
                ON d.date_key = f.date_key
               AND d.calendar_date >= r.date_from
               AND d.calendar_date <= r.date_to
            GROUP BY
                r.campaign_key,
                f.portfolio_key,
                f.source_debtor_id
        ),
        PaymentMetrics AS
        (
            SELECT
                r.campaign_key,
                COUNT_BIG(p.source_debtor_id) AS payment_count,
                MAX(p.loaded_at) AS loaded_at
            FROM EligibleCampaigns AS r
            LEFT JOIN PaymentDebtors AS p
                ON p.campaign_key = r.campaign_key
            GROUP BY r.campaign_key
        ),
        TargetMetrics AS
        (
            SELECT
                r.campaign_key,
                MAX(t.target_recovered_amount) AS target_amount,
                MAX(t.source_as_of_at) AS loaded_at
            FROM EligibleCampaigns AS r
            LEFT JOIN analytics.fact_target_monthly AS t
                ON @IncludeClientLevelTarget = 1
               AND t.client_key = @ClientKey
               AND t.campaign_key = r.campaign_key
               AND t.portfolio_key IS NULL
            GROUP BY r.campaign_key
        )
        SELECT
            r.campaign_code AS CampaignCode,
            r.campaign_name AS CampaignName,
            r.date_from AS DateFrom,
            r.date_to AS DateTo,
            sm.snapshot_date AS SnapshotDate,
            CONVERT(BIGINT, sm.assigned_clients) AS AssignedPortfolio,
            CONVERT(BIGINT, sm.managed_clients) AS ManagedPortfolio,
            CONVERT(BIGINT, sm.pending_clients) AS PendingPortfolio,
            CAST(
                1.0 * sm.managed_clients
                / NULLIF(sm.assigned_clients, 0)
                AS DECIMAL(18,6)
            ) AS ProgressRate,
            CONVERT(BIGINT, fm.management_count) AS ManagementCount,
            CAST(
                1.0 * sm.contacted_clients
                / NULLIF(sm.assigned_clients, 0)
                AS DECIMAL(18,6)
            ) AS ContactabilityRate,
            CAST(
                1.0 * cm.direct_contact_clients
                / NULLIF(cm.classifiable_clients, 0)
                AS DECIMAL(18,6)
            ) AS RpcRate,
            CAST(
                1.0 * pm.valid_promise_clients
                / NULLIF(cm.direct_contact_clients, 0)
                AS DECIMAL(18,6)
            ) AS CloseRate,
            CONVERT(BIGINT, pm.promise_count) AS PromiseCount,
            CAST(
                pm.fulfillment_paid_amount
                / NULLIF(pm.fulfillment_promise_amount, 0)
                AS DECIMAL(18,6)
            ) AS PromiseFulfillmentRate,
            CONVERT(BIGINT, paym.payment_count) AS PaymentCount,
            CONVERT(DECIMAL(19,4), fm.recovered_amount) AS RecoveredAmount,
            CASE
                WHEN @SubPortfolioId IS NULL AND @IncludeClientLevelTarget = 1 THEN tm.target_amount
                ELSE NULL
            END AS TargetAmount,
            updated_at.UpdatedAtUtc
        FROM EligibleCampaigns AS r
        INNER JOIN SnapshotMetrics AS sm
            ON sm.campaign_key = r.campaign_key
        INNER JOIN FlowMetrics AS fm
            ON fm.campaign_key = r.campaign_key
        INNER JOIN ContactMetrics AS cm
            ON cm.campaign_key = r.campaign_key
        INNER JOIN PromiseMetrics AS pm
            ON pm.campaign_key = r.campaign_key
        INNER JOIN PaymentMetrics AS paym
            ON paym.campaign_key = r.campaign_key
        INNER JOIN TargetMetrics AS tm
            ON tm.campaign_key = r.campaign_key
        CROSS APPLY
        (
            SELECT MAX(v.loaded_at) AS UpdatedAtUtc
            FROM
            (
                VALUES
                    (sm.loaded_at),
                    (fm.loaded_at),
                    (cm.loaded_at),
                    (pm.loaded_at),
                    (paym.loaded_at),
                    (tm.loaded_at)
            ) AS v(loaded_at)
        ) AS updated_at
        ORDER BY r.date_to DESC, r.campaign_code DESC;
        """;
}
