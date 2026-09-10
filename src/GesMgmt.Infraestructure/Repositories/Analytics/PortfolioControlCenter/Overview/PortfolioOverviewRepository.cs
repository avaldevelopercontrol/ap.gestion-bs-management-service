using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence;

namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal sealed class PortfolioOverviewRepository(AnalyticsDbContext context)
    : IPortfolioOverviewRepository
{
    public Task<PortfolioOverviewContext?> ResolveContextAsync(
        int crmClientId,
        string? campaignCode,
        long? subPortfolioId,
        string? businessUnit,
        CancellationToken cancellationToken) =>
        PortfolioOverviewContextEfQuery.ExecuteAsync(
            context,
            crmClientId,
            campaignCode,
            subPortfolioId,
            businessUnit,
            cancellationToken);

    public async Task<PortfolioOverviewDbRows> GetOverviewAsync(
        int clientKey,
        int campaignKey,
        long? subPortfolioId,
        string? businessUnit,
        bool includeClientLevelTarget,
        PortfolioSummaryRange range,
        CancellationToken cancellationToken)
    {
        var summary = await PortfolioSummaryEfQuery.ExecuteAsync(
            context,
            clientKey,
            campaignKey,
            subPortfolioId,
            businessUnit,
            range,
            cancellationToken);

        PortfolioTargetProgressDbRow? targetProgress = null;
        if (subPortfolioId is null && includeClientLevelTarget)
        {
            targetProgress = await PortfolioTargetProgressEfQuery.GetAsync(
                context,
                clientKey,
                campaignKey,
                businessUnit,
                range.DateTo,
                cancellationToken);
        }

        var promises = await PortfolioPromisesEfQuery.GetOperationalAsync(
            context,
            clientKey,
            campaignKey,
            subPortfolioId,
            businessUnit,
            cancellationToken);

        var evolution = await PortfolioEvolutionEfQuery.GetAsync(
            context,
            clientKey,
            campaignKey,
            subPortfolioId,
            businessUnit,
            new PortfolioEvolutionRange(range.DateFrom, range.DateTo),
            cancellationToken);

        return new PortfolioOverviewDbRows(
            summary,
            targetProgress,
            promises,
            evolution);
    }
}
