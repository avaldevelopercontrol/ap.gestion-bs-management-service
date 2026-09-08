using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence.Analytics;
using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal static class PortfolioFilterOptionsSql
{
    public const string FilterOptions = """
        DECLARE @ClientKey INT =
        (
            SELECT c.client_key
            FROM analytics.dim_client AS c
            WHERE c.crm_client_id = @CrmClientId
        );

        SELECT @ClientKey AS ClientKey
        WHERE @ClientKey IS NOT NULL;

        WITH /* pcc:filter-options:campaigns */ CampaignData AS
        (
            SELECT
                s.client_key,
                s.campaign_key,
                s.calendar_date,
                s.loaded_at
            FROM analytics.v_campaign_daily_summary AS s
            WHERE s.client_key = @ClientKey

            UNION ALL

            SELECT
                e.client_key,
                e.campaign_key,
                e.calendar_date,
                e.loaded_at
            FROM analytics.v_campaign_evolution_daily AS e
            WHERE e.client_key = @ClientKey
        )
        SELECT
            cp.campaign_code AS CampaignCode,
            cp.campaign_name AS CampaignName,
            cp.start_date AS StartDate,
            cp.end_date AS EndDate,
            MIN(cd.calendar_date) AS AvailableDateFrom,
            MAX(cd.calendar_date) AS AvailableDateTo,
            MAX(cd.loaded_at) AS UpdatedAtUtc
        FROM analytics.dim_campaign AS cp
        INNER JOIN CampaignData AS cd
            ON cd.client_key = cp.client_key
           AND cd.campaign_key = cp.campaign_key
        WHERE cp.client_key = @ClientKey
        GROUP BY
            cp.campaign_code,
            cp.campaign_name,
            cp.start_date,
            cp.end_date
        ORDER BY cp.start_date DESC, cp.campaign_code DESC;

        WITH /* pcc:filter-options:subportfolio-contexts */ SubPortfolioContexts AS
        (
            SELECT
                f.portfolio_key AS SubPortfolioId,
                MAX(p.portfolio_name) AS ContextSubPortfolioName,
                MAX(NULLIF(LTRIM(RTRIM(p.source_business_unit)), '')) AS BusinessUnitCode,
                cp.campaign_code AS CampaignCode,
                MIN(d.calendar_date) AS AvailableDateFrom,
                MAX(d.calendar_date) AS AvailableDateTo,
                MAX(f.loaded_at) AS UpdatedAtUtc
            FROM analytics.fact_portfolio_daily AS f
            INNER JOIN analytics.dim_portfolio AS p
                ON p.portfolio_key = f.portfolio_key
            INNER JOIN analytics.dim_campaign AS cp
                ON cp.campaign_key = f.campaign_key
            INNER JOIN analytics.dim_date AS d
                ON d.date_key = f.date_key
            WHERE f.client_key = @ClientKey
            GROUP BY
                f.portfolio_key,
                cp.campaign_code
        ),
        SubPortfolioContextsWithParent AS
        (
            SELECT
                context.SubPortfolioId,
                MAX(context.ContextSubPortfolioName) OVER
                (
                    PARTITION BY context.SubPortfolioId
                ) AS SubPortfolioName,
                context.BusinessUnitCode,
                context.CampaignCode,
                context.AvailableDateFrom,
                context.AvailableDateTo,
                context.UpdatedAtUtc,
                MAX(context.UpdatedAtUtc) OVER
                (
                    PARTITION BY context.SubPortfolioId
                ) AS SubPortfolioUpdatedAtUtc
            FROM SubPortfolioContexts AS context
        )
        SELECT
            context.SubPortfolioId,
            context.SubPortfolioName,
            context.BusinessUnitCode,
            context.CampaignCode,
            context.AvailableDateFrom,
            context.AvailableDateTo,
            context.UpdatedAtUtc,
            context.SubPortfolioUpdatedAtUtc,
            DENSE_RANK() OVER
            (
                ORDER BY context.SubPortfolioName, context.SubPortfolioId
            ) AS SubPortfolioSortOrder
        FROM SubPortfolioContextsWithParent AS context
        ORDER BY
            context.CampaignCode DESC,
            context.SubPortfolioId;

        WITH /* pcc:filter-options:supervisor-contexts */ SupervisorContexts AS
        (
            SELECT
                a.supervisor_key AS SupervisorId,
                MAX(a.supervisor_name) AS ContextSupervisorName,
                a.portfolio_key AS SubPortfolioId,
                cp.campaign_code AS CampaignCode,
                MIN(a.calendar_date) AS AvailableDateFrom,
                MAX(a.calendar_date) AS AvailableDateTo,
                MAX(a.loaded_at) AS UpdatedAtUtc
            FROM analytics.v_supervisor_advisor_daily_attribution AS a
            INNER JOIN analytics.dim_campaign AS cp
                ON cp.campaign_key = a.campaign_key
            WHERE a.client_key = @ClientKey
              AND a.supervisor_key IS NOT NULL
            GROUP BY
                a.supervisor_key,
                a.portfolio_key,
                cp.campaign_code
        ),
        SupervisorContextsWithParent AS
        (
            SELECT
                context.SupervisorId,
                MAX(context.ContextSupervisorName) OVER
                (
                    PARTITION BY context.SupervisorId
                ) AS SupervisorName,
                context.SubPortfolioId,
                context.CampaignCode,
                context.AvailableDateFrom,
                context.AvailableDateTo,
                context.UpdatedAtUtc,
                MAX(context.UpdatedAtUtc) OVER
                (
                    PARTITION BY context.SupervisorId
                ) AS SupervisorUpdatedAtUtc
            FROM SupervisorContexts AS context
        )
        SELECT
            context.SupervisorId,
            context.SupervisorName,
            context.SubPortfolioId,
            context.CampaignCode,
            context.AvailableDateFrom,
            context.AvailableDateTo,
            context.UpdatedAtUtc,
            context.SupervisorUpdatedAtUtc,
            DENSE_RANK() OVER
            (
                ORDER BY context.SupervisorName, context.SupervisorId
            ) AS SupervisorSortOrder
        FROM SupervisorContextsWithParent AS context
        ORDER BY
            context.CampaignCode DESC,
            context.SubPortfolioId,
            context.SupervisorId;
        """;
}
