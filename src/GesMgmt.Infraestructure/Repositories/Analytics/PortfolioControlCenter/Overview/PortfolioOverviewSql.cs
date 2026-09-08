using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence.Analytics;
using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal static class PortfolioOverviewSql
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

        WITH /* pcc:overview-context */ SelectedSummaryContext AS
        (
            SELECT TOP (1)
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
                cp.campaign_key DESC
        )
        SELECT
            selected.ClientKey,
            selected.CampaignKey,
            selected.CampaignCode,
            selected.CampaignName,
            selected.StartDate,
            selected.EndDate,
            selected.LatestDataDate,
            CASE
                WHEN @SubPortfolioId IS NULL THEN CONVERT(BIT, 1)
                WHEN portfolio_data.LatestPortfolioDataDate IS NOT NULL
                    THEN CONVERT(BIT, 1)
                ELSE CONVERT(BIT, 0)
            END AS OperationalSubPortfolioAvailable
        FROM SelectedSummaryContext AS selected
        OUTER APPLY
        (
            SELECT MAX(v.calendar_date) AS LatestPortfolioDataDate
            FROM analytics.v_portfolio_daily_metrics AS v
            WHERE v.client_key = selected.ClientKey
              AND v.campaign_key = selected.CampaignKey
              AND @SubPortfolioId IS NOT NULL
              AND v.portfolio_key = @SubPortfolioId
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
        OPTION (RECOMPILE);
        """;

    private const string EmptyTargetProgress = """
        SELECT
            CAST(NULL AS DATE) AS AsOfDate,
            CAST(NULL AS DECIMAL(19,4)) AS MonthlyTargetAmount,
            CAST(NULL AS DECIMAL(19,4)) AS ExpectedToDateAmount,
            CAST(NULL AS DECIMAL(18,6)) AS TargetAchievementRate,
            CAST(NULL AS DECIMAL(18,6)) AS PaceAchievementRate,
            CAST(NULL AS DECIMAL(19,4)) AS GapAmount,
            CAST(NULL AS DECIMAL(18,6)) AS GapRate,
            CAST(NULL AS DATETIME2) AS UpdatedAtUtc
        WHERE 1 = 0;
        """;

    public static readonly string Query = $"""
        {PortfolioSummarySql.Summary}

        IF @SubPortfolioId IS NULL AND @IncludeClientLevelTarget = 1
        BEGIN
            {PortfolioTargetProgressSql.TargetProgress}
        END
        ELSE
        BEGIN
            {EmptyTargetProgress}
        END;

        {PortfolioPromisesSql.OperationalPromises}

        {PortfolioEvolutionSql.Evolution}
        """;
}
