using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence.Analytics;
using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal static class PortfolioPromisesSql
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

        SELECT /* pcc:promises-context */ TOP (1)
            c.client_key AS ClientKey,
            cp.campaign_key AS CampaignKey,
            cp.campaign_code AS CampaignCode,
            cp.campaign_name AS CampaignName
        FROM analytics.dim_client AS c
        INNER JOIN analytics.dim_campaign AS cp
            ON cp.client_key = c.client_key
        OUTER APPLY
        (
            SELECT
                COUNT_BIG(p.promise_fact_key) AS PromiseCount,
                MAX(p.loaded_at) AS LatestPromiseLoadedAt
            FROM analytics.v_promise_operational AS p
            WHERE p.client_key = cp.client_key
              AND p.campaign_key = cp.campaign_key
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
        ) AS promise_data
        OUTER APPLY
        (
            SELECT MAX(v.calendar_date) AS LatestPortfolioDataDate
            FROM analytics.v_portfolio_daily_metrics AS v
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
        ) AS portfolio_data
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
              OR portfolio_data.LatestPortfolioDataDate IS NOT NULL
          )
        ORDER BY
            CASE WHEN promise_data.PromiseCount = 0 THEN 1 ELSE 0 END,
            promise_data.LatestPromiseLoadedAt DESC,
            cp.start_date DESC,
            cp.campaign_key DESC;
        """;

    public const string OperationalPromises = """
        WITH /* pcc:promises */ OperationalMetrics AS
        (
            SELECT
                COALESCE(SUM(
                    CASE
                        WHEN p.is_due_today = 1
                            THEN CONVERT(BIGINT, 1)
                        ELSE CONVERT(BIGINT, 0)
                    END
                ), 0) AS DueTodayCount,
                COALESCE(SUM(
                    CASE
                        WHEN p.is_due_today = 1
                            THEN p.promise_amount
                        ELSE CONVERT(DECIMAL(19,4), 0)
                    END
                ), 0) AS DueTodayAmount,
                COALESCE(SUM(
                    CASE
                        WHEN p.is_broken = 1
                            THEN CONVERT(BIGINT, 1)
                        ELSE CONVERT(BIGINT, 0)
                    END
                ), 0) AS OverdueCount,
                COALESCE(SUM(
                    CASE
                        WHEN p.is_fulfilled_or_partial = 1
                            THEN p.paid_amount
                        ELSE CONVERT(DECIMAL(19,4), 0)
                    END
                ), 0) AS FulfillmentPaidAmount,
                COALESCE(SUM(
                    CASE
                        WHEN p.is_fulfilled_or_partial = 1
                          OR p.is_broken = 1
                            THEN p.promise_amount
                        ELSE CONVERT(DECIMAL(19,4), 0)
                    END
                ), 0) AS FulfillmentPromiseAmount,
                MAX(p.loaded_at) AS UpdatedAtUtc
            FROM analytics.v_promise_operational AS p
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
        )
        SELECT
            CONVERT(BIGINT, DueTodayCount) AS DueTodayCount,
            CONVERT(DECIMAL(19,4), DueTodayAmount) AS DueTodayAmount,
            CONVERT(BIGINT, OverdueCount) AS OverdueCount,
            CAST(
                FulfillmentPaidAmount
                / NULLIF(FulfillmentPromiseAmount, 0)
                AS DECIMAL(18,6)
            ) AS FulfillmentRate,
            UpdatedAtUtc
        FROM OperationalMetrics
        OPTION (RECOMPILE);
        """;
}
