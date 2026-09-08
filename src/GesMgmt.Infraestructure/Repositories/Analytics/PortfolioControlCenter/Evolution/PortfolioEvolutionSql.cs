using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence.Analytics;
using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal static class PortfolioEvolutionSql
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

        SELECT /* pcc:evolution-context */ TOP (1)
            c.client_key AS ClientKey,
            cp.campaign_key AS CampaignKey,
            cp.campaign_code AS CampaignCode,
            cp.campaign_name AS CampaignName,
            cp.start_date AS StartDate,
            cp.end_date AS EndDate,
            evolution_data.LatestEvolutionDate
        FROM analytics.dim_client AS c
        INNER JOIN analytics.dim_campaign AS cp
            ON cp.client_key = c.client_key
        OUTER APPLY
        (
            SELECT MAX(e.calendar_date) AS LatestEvolutionDate
            FROM analytics.v_portfolio_evolution_daily AS e
            WHERE e.client_key = cp.client_key
              AND e.campaign_key = cp.campaign_key
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
        ) AS evolution_data
        OUTER APPLY
        (
            SELECT MAX(v.calendar_date) AS LatestPortfolioDataDate
            FROM analytics.v_portfolio_daily_metrics AS v
            WHERE v.client_key = cp.client_key
              AND v.campaign_key = cp.campaign_key
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
                  AND
                  (
                      @BusinessUnit IS NULL
                      OR evolution_data.LatestEvolutionDate IS NOT NULL
                  )
              )
              OR
              (
                  @SubPortfolioId IS NOT NULL
                  AND portfolio_data.LatestPortfolioDataDate IS NOT NULL
              )
          )
        ORDER BY
            CASE WHEN evolution_data.LatestEvolutionDate IS NULL THEN 1 ELSE 0 END,
            cp.start_date DESC,
            cp.campaign_key DESC;
        """;

    public const string Evolution = """
        SELECT /* pcc:evolution:campaign */
            e.calendar_date AS Period,
            CONVERT(BIGINT, e.assigned_clients) AS AssignedPortfolio,
            CONVERT(BIGINT, e.managed_clients) AS ManagedPortfolio,
            CONVERT(BIGINT, e.pending_clients) AS PendingPortfolio,
            CONVERT(DECIMAL(19,4), e.recovered_amount_to_date) AS RecoveredAmount,
            e.loaded_at AS LoadedAtUtc
        FROM analytics.v_campaign_evolution_daily AS e
        WHERE @SubPortfolioId IS NULL
          AND @BusinessUnit IS NULL
          AND e.client_key = @ClientKey
          AND e.campaign_key = @CampaignKey
          AND e.calendar_date >= @DateFrom
          AND e.calendar_date < @DateToExclusive

        UNION ALL

        SELECT /* pcc:evolution:business-unit */
            e.calendar_date AS Period,
            CONVERT(BIGINT, SUM(CONVERT(BIGINT, e.assigned_clients))) AS AssignedPortfolio,
            CONVERT(BIGINT, SUM(CONVERT(BIGINT, e.managed_clients))) AS ManagedPortfolio,
            CONVERT(BIGINT, SUM(CONVERT(BIGINT, e.pending_clients))) AS PendingPortfolio,
            CONVERT(DECIMAL(19,4), SUM(e.recovered_amount_to_date)) AS RecoveredAmount,
            MAX(e.loaded_at) AS LoadedAtUtc
        FROM analytics.v_portfolio_evolution_daily AS e
        WHERE @SubPortfolioId IS NULL
          AND @BusinessUnit IS NOT NULL
          AND e.client_key = @ClientKey
          AND e.campaign_key = @CampaignKey
          AND EXISTS
          (
              SELECT 1
              FROM analytics.dim_portfolio AS business_unit_portfolio
              WHERE business_unit_portfolio.portfolio_key = e.portfolio_key
                AND business_unit_portfolio.source_business_unit = @BusinessUnit
          )
          AND e.calendar_date >= @DateFrom
          AND e.calendar_date < @DateToExclusive
        GROUP BY e.calendar_date

        UNION ALL

        SELECT /* pcc:evolution:portfolio */
            e.calendar_date AS Period,
            CONVERT(BIGINT, e.assigned_clients) AS AssignedPortfolio,
            CONVERT(BIGINT, e.managed_clients) AS ManagedPortfolio,
            CONVERT(BIGINT, e.pending_clients) AS PendingPortfolio,
            CONVERT(DECIMAL(19,4), e.recovered_amount_to_date) AS RecoveredAmount,
            e.loaded_at AS LoadedAtUtc
        FROM analytics.v_portfolio_evolution_daily AS e
        WHERE @SubPortfolioId IS NOT NULL
          AND e.client_key = @ClientKey
          AND e.campaign_key = @CampaignKey
          AND e.portfolio_key = @SubPortfolioId
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
          AND e.calendar_date >= @DateFrom
          AND e.calendar_date < @DateToExclusive

        ORDER BY Period
        OPTION (RECOMPILE);
        """;
}
