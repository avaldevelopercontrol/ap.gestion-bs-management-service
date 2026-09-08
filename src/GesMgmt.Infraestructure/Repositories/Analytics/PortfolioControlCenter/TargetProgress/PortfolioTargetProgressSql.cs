using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence.Analytics;
using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal static class PortfolioTargetProgressSql
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

        SELECT /* pcc:target-progress-context */ TOP (1)
            c.client_key AS ClientKey,
            cp.campaign_key AS CampaignKey,
            cp.campaign_code AS CampaignCode,
            cp.campaign_name AS CampaignName,
            cp.start_date AS StartDate,
            cp.end_date AS EndDate,
            CASE
                WHEN @SubPortfolioId IS NULL
                     AND @IncludeClientLevelTarget = 1
                    THEN target_data.LatestProgressDate
                ELSE portfolio_data.LatestPortfolioDataDate
            END AS LatestProgressDate
        FROM analytics.dim_client AS c
        INNER JOIN analytics.dim_campaign AS cp
            ON cp.client_key = c.client_key
        OUTER APPLY
        (
            SELECT MAX(t.calendar_date) AS LatestProgressDate
            FROM analytics.v_campaign_target_progress AS t
            WHERE @IncludeClientLevelTarget = 1
              AND t.client_key = cp.client_key
              AND t.campaign_key = cp.campaign_key
        ) AS target_data
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
              (
                  @SubPortfolioId IS NULL
                  AND @IncludeClientLevelTarget = 1
              )
              OR portfolio_data.LatestPortfolioDataDate IS NOT NULL
          )
        ORDER BY
            CASE
                WHEN @SubPortfolioId IS NULL
                     AND @IncludeClientLevelTarget = 1
                     AND target_data.LatestProgressDate IS NULL THEN 1
                WHEN (@SubPortfolioId IS NOT NULL OR @IncludeClientLevelTarget = 0)
                     AND portfolio_data.LatestPortfolioDataDate IS NULL THEN 1
                ELSE 0
            END,
            CASE
                WHEN @SubPortfolioId IS NULL
                     AND @IncludeClientLevelTarget = 1
                    THEN target_data.LatestProgressDate
                ELSE portfolio_data.LatestPortfolioDataDate
            END DESC,
            cp.start_date DESC,
            cp.campaign_key DESC;
        """;

    public const string TargetProgress = """
        WITH TargetProgressSource AS
        (
            SELECT /* pcc:target-progress */ TOP (1)
                t.calendar_date AS AsOfDate,
                CONVERT(DECIMAL(19,4), t.target_recovered_amount)
                    AS MonthlyTargetAmount,
                CONVERT(DECIMAL(19,4), t.expected_recovered_to_date)
                    AS ExpectedToDateAmount,
                t.target_source_as_of_at AS UpdatedAtUtc
            FROM analytics.v_campaign_target_progress AS t
            WHERE t.client_key = @ClientKey
              AND t.campaign_key = @CampaignKey
              AND t.calendar_date <= @DateTo
            ORDER BY t.calendar_date DESC
        ),
        ScopedRecovery AS
        (
            SELECT
                CONVERT(DECIMAL(19,4),
                    COALESCE(SUM(v.recovered_amount_day), 0)) AS RecoveredAmount
            FROM analytics.v_portfolio_daily_metrics AS v
            CROSS JOIN TargetProgressSource AS target
            WHERE v.client_key = @ClientKey
              AND v.campaign_key = @CampaignKey
              AND v.calendar_date <= target.AsOfDate
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
        )
        SELECT
            target.AsOfDate,
            target.MonthlyTargetAmount,
            target.ExpectedToDateAmount,
            CAST(
                recovery.RecoveredAmount
                / NULLIF(target.MonthlyTargetAmount, 0)
                AS DECIMAL(18,6)
            ) AS TargetAchievementRate,
            CAST(
                recovery.RecoveredAmount
                / NULLIF(target.ExpectedToDateAmount, 0)
                AS DECIMAL(18,6)
            ) AS PaceAchievementRate,
            CONVERT(DECIMAL(19,4),
                recovery.RecoveredAmount - target.ExpectedToDateAmount)
                AS GapAmount,
            CAST(
                (recovery.RecoveredAmount - target.ExpectedToDateAmount)
                / NULLIF(target.ExpectedToDateAmount, 0)
                AS DECIMAL(18,6)
            ) AS GapRate,
            target.UpdatedAtUtc
        FROM TargetProgressSource AS target
        CROSS JOIN ScopedRecovery AS recovery;
        """;
}
