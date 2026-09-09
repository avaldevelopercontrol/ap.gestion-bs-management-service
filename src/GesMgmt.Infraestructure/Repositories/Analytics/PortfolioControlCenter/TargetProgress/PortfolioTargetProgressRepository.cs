using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence;

namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal sealed class PortfolioTargetProgressRepository(
    AnalyticsDbContext context)
    : IPortfolioTargetProgressRepository
{
    public Task<PortfolioTargetProgressContext?> ResolveContextAsync(
        int crmClientId,
        string? campaignCode,
        long? subPortfolioId,
        string? businessUnit,
        bool includeClientLevelTarget,
        CancellationToken cancellationToken) =>
        PortfolioTargetProgressEfQuery.ResolveContextAsync(
            context,
            crmClientId,
            campaignCode,
            subPortfolioId,
            businessUnit,
            includeClientLevelTarget,
            cancellationToken);

    public Task<PortfolioTargetProgressDbRow?> GetTargetProgressAsync(
        int clientKey,
        int campaignKey,
        string? businessUnit,
        DateOnly dateTo,
        CancellationToken cancellationToken) =>
        PortfolioTargetProgressEfQuery.GetAsync(
            context,
            clientKey,
            campaignKey,
            businessUnit,
            dateTo,
            cancellationToken);
}
