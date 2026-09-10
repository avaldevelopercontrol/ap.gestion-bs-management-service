using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
namespace GesMgmt.Application.Services.Analytics.PortfolioControlCenter;

internal static class PortfolioOverviewResponseMapper
{
    public static PortfolioOverviewResponse Map(
        PortfolioSummaryContext context,
        PortfolioSummaryRange range,
        PortfolioOverviewDbRows rows)
    {
        var targetContext = new PortfolioTargetProgressContext(
            context.ClientKey,
            context.CampaignKey,
            context.CampaignCode,
            context.CampaignName,
            context.StartDate,
            context.EndDate,
            context.LatestDataDate);

        var promisesContext = new PortfolioPromisesContext(
            context.ClientKey,
            context.CampaignKey,
            context.CampaignCode,
            context.CampaignName);

        var evolutionContext = new PortfolioEvolutionContext(
            context.ClientKey,
            context.CampaignKey,
            context.CampaignCode,
            context.CampaignName,
            context.StartDate,
            context.EndDate,
            context.LatestDataDate);

        return new PortfolioOverviewResponse(
            PortfolioSummaryResponseMapper.Map(
                context,
                range,
                rows.Summary),
            PortfolioTargetProgressResponseMapper.Map(
                targetContext,
                range.DateTo,
                rows.TargetProgress),
            PortfolioPromisesResponseMapper.Map(
                promisesContext,
                rows.Promises),
            PortfolioEvolutionResponseMapper.Map(
                evolutionContext,
                new PortfolioEvolutionRange(
                    range.DateFrom,
                    range.DateTo),
                rows.Evolution));
    }
}
