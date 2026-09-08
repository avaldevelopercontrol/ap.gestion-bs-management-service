using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence.Analytics;
using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal static class PortfolioSummarySql
{
    public const string ResolveContext = """
        DECLARE @RejectAmbiguousUnscopedBusinessUnit INT =
        (
            SELECT /* pcc:business-unit:unscoped-ambiguity-guard */
                CASE
                    WHEN @BusinessUnit IS NOT NULL THEN 0
                    WHEN COUNT_BIG(*) > 1 THEN 1
                    ELSE 0
                END
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
                INNER JOIN analytics.dim_client AS scope_client
                    ON scope_client.client_key = scope_fact.client_key
                WHERE scope_client.crm_client_id = @CrmClientId
                  AND @BusinessUnit IS NULL
            ) AS available_business_unit_scope
        );

        SELECT /* pcc:summary-context */ TOP (1)
            c.client_key AS ClientKey,
            cp.campaign_key AS CampaignKey,
            cp.campaign_code AS CampaignCode,
            cp.campaign_name AS CampaignName,
            cp.start_date AS StartDate,
            cp.end_date AS EndDate,
            data.LatestDataDate
        FROM analytics.dim_client AS c
        INNER JOIN analytics.dim_campaign AS cp
            ON cp.client_key = c.client_key
        OUTER APPLY
        (
            SELECT MAX(v.calendar_date) AS LatestDataDate
            FROM analytics.v_portfolio_summary_state_daily AS v
            WHERE v.client_key = cp.client_key
              AND v.campaign_key = cp.campaign_key
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
        ) AS data
        WHERE c.crm_client_id = @CrmClientId
          AND @RejectAmbiguousUnscopedBusinessUnit = 0
          AND (@CampaignCode IS NULL OR cp.campaign_code = @CampaignCode)
          AND
          (
              @CampaignCode IS NOT NULL
              OR @BusinessUnit IS NULL
              OR cp.campaign_key =
              (
                  SELECT TOP (1) latest_campaign.campaign_key
                  FROM analytics.dim_campaign AS latest_campaign
                  WHERE latest_campaign.client_key = cp.client_key
                    AND EXISTS
                    (
                        SELECT 1
                        FROM analytics.fact_portfolio_daily AS latest_campaign_fact
                        WHERE latest_campaign_fact.client_key = latest_campaign.client_key
                          AND latest_campaign_fact.campaign_key = latest_campaign.campaign_key
                    )
                  ORDER BY
                      latest_campaign.start_date DESC,
                      latest_campaign.campaign_key DESC
              )
          )
          AND
          (
              (@SubPortfolioId IS NULL AND @BusinessUnit IS NULL)
              OR data.LatestDataDate IS NOT NULL
          )
        ORDER BY
            CASE WHEN data.LatestDataDate IS NULL THEN 1 ELSE 0 END,
            cp.start_date DESC,
            cp.campaign_key DESC;
        """;

    public const string Summary = """
        WITH /* pcc:summary */ SnapshotMetrics AS
        (
            SELECT
                snapshot.SnapshotDate,
                COALESCE(snapshot.AssignedPortfolio, 0) AS AssignedPortfolio,
                COALESCE(snapshot.ManagedPortfolio, 0) AS ManagedPortfolio,
                COALESCE(snapshot.PendingPortfolio, 0) AS PendingPortfolio,
                snapshot.ContactedPortfolio,
                snapshot.SnapshotLoadedAtUtc
            FROM (VALUES (1)) AS anchor(single_row)
            OUTER APPLY
            (
                SELECT TOP (1)
                    candidate.SnapshotDate,
                    candidate.AssignedPortfolio,
                    candidate.ManagedPortfolio,
                    candidate.PendingPortfolio,
                    candidate.ContactedPortfolio,
                    candidate.SnapshotLoadedAtUtc
                FROM
                (
                    SELECT
                        d.calendar_date AS SnapshotDate,
                        CONVERT(TINYINT, 1) AS HasRealSnapshot,
                        SUM(f.assigned_clients_snapshot) AS AssignedPortfolio,
                        SUM(f.managed_clients_snapshot) AS ManagedPortfolio,
                        SUM(f.pending_clients_snapshot) AS PendingPortfolio,
                        SUM(f.contacted_clients_snapshot) AS ContactedPortfolio,
                        MAX(f.loaded_at) AS SnapshotLoadedAtUtc
                    FROM analytics.fact_portfolio_daily AS f
                    INNER JOIN analytics.dim_date AS d
                        ON d.date_key = f.date_key
                    WHERE f.client_key = @ClientKey
                      AND f.campaign_key = @CampaignKey
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
                      AND f.has_source_snapshot = 1
                      AND d.calendar_date >= @DateFrom
                      AND d.calendar_date < @DateToExclusive
                    GROUP BY d.calendar_date

                    UNION ALL

                    SELECT
                        d.calendar_date AS SnapshotDate,
                        CONVERT(TINYINT, 0) AS HasRealSnapshot,
                        SUM(CONVERT(INT, e.assigned_clients)) AS AssignedPortfolio,
                        SUM(CONVERT(INT, e.managed_clients)) AS ManagedPortfolio,
                        SUM(CONVERT(INT, e.pending_clients)) AS PendingPortfolio,
                        CAST(NULL AS INT) AS ContactedPortfolio,
                        MAX(e.loaded_at) AS SnapshotLoadedAtUtc
                    FROM analytics.fact_portfolio_evolution_daily AS e
                    INNER JOIN analytics.dim_date AS d
                        ON d.date_key = e.date_key
                    WHERE e.client_key = @ClientKey
                      AND e.campaign_key = @CampaignKey
                      AND (@SubPortfolioId IS NULL OR e.portfolio_key = @SubPortfolioId)
                      AND (
                          @BusinessUnit IS NULL
                          OR EXISTS
                          (
                              SELECT 1
                              FROM analytics.dim_portfolio AS business_unit_portfolio
                              WHERE business_unit_portfolio.portfolio_key = e.portfolio_key
                                AND business_unit_portfolio.source_business_unit = @BusinessUnit
                          )
                      )
                      AND d.calendar_date >= @DateFrom
                      AND d.calendar_date < @DateToExclusive
                      AND NOT EXISTS
                      (
                          SELECT 1
                          FROM analytics.fact_portfolio_daily AS real_snapshot
                          WHERE real_snapshot.client_key = e.client_key
                            AND real_snapshot.campaign_key = e.campaign_key
                            AND real_snapshot.date_key = e.date_key
                            AND (@SubPortfolioId IS NULL OR real_snapshot.portfolio_key = @SubPortfolioId)
                            AND (
                                @BusinessUnit IS NULL
                                OR EXISTS
                                (
                                    SELECT 1
                                    FROM analytics.dim_portfolio AS real_snapshot_business_unit
                                    WHERE real_snapshot_business_unit.portfolio_key = real_snapshot.portfolio_key
                                      AND real_snapshot_business_unit.source_business_unit = @BusinessUnit
                                )
                            )
                            AND real_snapshot.has_source_snapshot = 1
                      )
                    GROUP BY d.calendar_date
                ) AS candidate
                ORDER BY
                    candidate.HasRealSnapshot DESC,
                    candidate.SnapshotDate DESC
            ) AS snapshot
        ),
        FlowMetrics AS
        (
            SELECT
                COALESCE(SUM(v.management_events_day), 0) AS ManagementCount,
                COALESCE(SUM(v.recovered_amount_day), 0) AS RecoveredAmount,
                MAX(v.loaded_at) AS FlowLoadedAtUtc
            FROM analytics.v_portfolio_daily_metrics AS v
            WHERE v.client_key = @ClientKey
              AND v.campaign_key = @CampaignKey
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
              AND v.calendar_date >= @DateFrom
              AND v.calendar_date < @DateToExclusive
        ),
        ContactPerDebtor AS
        (
            SELECT
                f.portfolio_key,
                f.source_debtor_id,
                MAX(CONVERT(TINYINT, f.had_direct_contact)) AS had_direct_contact,
                MAX(CONVERT(TINYINT, f.had_indirect_contact)) AS had_indirect_contact,
                MAX(CONVERT(TINYINT, f.had_no_contact)) AS had_no_contact,
                MAX(f.loaded_at) AS loaded_at
            FROM analytics.fact_debtor_contact_daily AS f
            INNER JOIN analytics.dim_date AS d
                ON d.date_key = f.date_key
            WHERE f.client_key = @ClientKey
              AND f.campaign_key = @CampaignKey
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
              AND d.calendar_date >= @DateFrom
              AND d.calendar_date < @DateToExclusive
            GROUP BY
                f.portfolio_key,
                f.source_debtor_id
        ),
        ContactMetrics AS
        (
            SELECT
                COALESCE(SUM(CASE WHEN had_direct_contact = 1 THEN 1 ELSE 0 END), 0)
                    AS DirectContactClients,
                COALESCE(SUM(
                    CASE
                        WHEN had_direct_contact = 1
                          OR had_indirect_contact = 1
                          OR had_no_contact = 1
                            THEN 1
                        ELSE 0
                    END
                ), 0) AS ClassifiableClients,
                MAX(loaded_at) AS ContactLoadedAtUtc
            FROM ContactPerDebtor
        ),
        PromisePerDebtor AS
        (
            SELECT
                p.portfolio_key,
                p.source_debtor_id,
                COUNT_BIG(*) AS promise_count,
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
            FROM analytics.fact_promise AS p
            WHERE p.client_key = @ClientKey
              AND p.campaign_key = @CampaignKey
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
              AND p.is_valid_promise = 1
              AND p.management_at >= @DateFrom
              AND p.management_at < @DateToExclusive
            GROUP BY
                p.portfolio_key,
                p.source_debtor_id
        ),
        PromiseMetrics AS
        (
            SELECT
                COALESCE(SUM(promise_count), 0) AS PromiseCount,
                COUNT_BIG(*) AS ValidPromiseClients,
                COALESCE(SUM(fulfillment_paid_amount), 0) AS FulfillmentPaidAmount,
                COALESCE(SUM(fulfillment_promise_amount), 0) AS FulfillmentPromiseAmount,
                MAX(loaded_at) AS PromiseLoadedAtUtc
            FROM PromisePerDebtor
        ),
        PaymentDebtors AS
        (
            SELECT
                f.portfolio_key,
                f.source_debtor_id,
                MAX(f.loaded_at) AS loaded_at
            FROM analytics.fact_debtor_payment_daily AS f
            INNER JOIN analytics.dim_date AS d
                ON d.date_key = f.date_key
            WHERE f.client_key = @ClientKey
              AND f.campaign_key = @CampaignKey
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
              AND d.calendar_date >= @DateFrom
              AND d.calendar_date < @DateToExclusive
            GROUP BY
                f.portfolio_key,
                f.source_debtor_id
        ),
        PaymentMetrics AS
        (
            SELECT
                COUNT_BIG(*) AS PaymentCount,
                MAX(loaded_at) AS PaymentLoadedAtUtc
            FROM PaymentDebtors
        )
        SELECT
            sm.SnapshotDate,
            CONVERT(BIGINT, sm.AssignedPortfolio) AS AssignedPortfolio,
            CONVERT(BIGINT, sm.ManagedPortfolio) AS ManagedPortfolio,
            CONVERT(BIGINT, sm.PendingPortfolio) AS PendingPortfolio,
            CONVERT(BIGINT, fm.ManagementCount) AS ManagementCount,
            CAST(
                1.0 * fm.ManagementCount
                / NULLIF(sm.ManagedPortfolio, 0)
                AS DECIMAL(18,6)
            ) AS ManagementIntensity,
            CONVERT(DECIMAL(19,4), fm.RecoveredAmount) AS RecoveredAmount,
            CAST(
                1.0 * sm.ContactedPortfolio
                / NULLIF(sm.AssignedPortfolio, 0)
                AS DECIMAL(18,6)
            ) AS ContactabilityRate,
            CAST(
                1.0 * cm.DirectContactClients
                / NULLIF(cm.ClassifiableClients, 0)
                AS DECIMAL(18,6)
            ) AS RpcRate,
            CAST(
                1.0 * pm.ValidPromiseClients
                / NULLIF(cm.DirectContactClients, 0)
                AS DECIMAL(18,6)
            ) AS CloseRate,
            CONVERT(BIGINT, pm.PromiseCount) AS PromiseCount,
            CAST(
                pm.FulfillmentPaidAmount
                / NULLIF(pm.FulfillmentPromiseAmount, 0)
                AS DECIMAL(18,6)
            ) AS PromiseFulfillmentRate,
            CONVERT(BIGINT, paym.PaymentCount) AS PaymentCount,
            updated_at.UpdatedAtUtc,
            freshness.OperationAsOfLocal,
            freshness.PortfolioBaseRefreshedAtUtc,
            freshness.RefreshedAtUtc
        FROM SnapshotMetrics AS sm
        CROSS JOIN FlowMetrics AS fm
        CROSS JOIN ContactMetrics AS cm
        CROSS JOIN PromiseMetrics AS pm
        CROSS JOIN PaymentMetrics AS paym
        CROSS APPLY
        (
            SELECT MAX(v.loaded_at) AS UpdatedAtUtc
            FROM
            (
                VALUES
                    (sm.SnapshotLoadedAtUtc),
                    (fm.FlowLoadedAtUtc),
                    (cm.ContactLoadedAtUtc),
                    (pm.PromiseLoadedAtUtc),
                    (paym.PaymentLoadedAtUtc)
            ) AS v(loaded_at)
        ) AS updated_at
        OUTER APPLY
        (
            SELECT
                COALESCE(
                    MAX(CASE
                        WHEN w.source_code = 'CLARO_INTRADAY_UPSTREAM'
                            THEN w.last_source_datetime
                    END),
                    MIN(CASE
                        WHEN w.source_code IN
                        (
                            'GESTION_COB2_LIVE',
                            'CLARO_ADVISOR_DAILY'
                        )
                            THEN w.last_source_datetime
                    END)
                ) AS OperationAsOfLocal,
                MAX(CASE
                    WHEN w.source_code = 'CLARO_PORTFOLIO_SNAPSHOT'
                        THEN w.last_success_at
                END) AS PortfolioBaseRefreshedAtUtc,
                COALESCE(
                    MAX(CASE
                        WHEN w.source_code = 'CLARO_INTRADAY_UPSTREAM'
                            THEN w.last_success_at
                    END),
                    MAX(CASE
                        WHEN w.source_code IN
                        (
                            'GESTION_COB2_LIVE',
                            'CLARO_ADVISOR_DAILY'
                        )
                            THEN w.last_success_at
                    END)
                ) AS RefreshedAtUtc
            FROM etl.watermark AS w
            WHERE w.source_code IN
            (
                'CLARO_INTRADAY_UPSTREAM',
                'GESTION_COB2_LIVE',
                'CLARO_ADVISOR_DAILY',
                'CLARO_PORTFOLIO_SNAPSHOT'
            )
        ) AS freshness
        OPTION (RECOMPILE);
        """;
}
