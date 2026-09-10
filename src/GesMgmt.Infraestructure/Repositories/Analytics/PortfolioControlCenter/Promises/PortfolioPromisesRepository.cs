using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence;

namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal sealed class PortfolioPromisesRepository(AnalyticsDbContext context)
    : IPortfolioPromisesRepository
{
    public Task<PortfolioPromisesContext?> ResolveContextAsync(
        int crmClientId,
        string? campaignCode,
        long? subPortfolioId,
        string? businessUnit,
        CancellationToken cancellationToken) =>
        PortfolioPromisesEfQuery.ResolveContextAsync(
            context,
            crmClientId,
            campaignCode,
            subPortfolioId,
            businessUnit,
            cancellationToken);

    public Task<PortfolioPromisesDbRow> GetOperationalPromisesAsync(
        int clientKey,
        int campaignKey,
        long? subPortfolioId,
        string? businessUnit,
        CancellationToken cancellationToken) =>
        PortfolioPromisesEfQuery.GetOperationalAsync(
            context,
            clientKey,
            campaignKey,
            subPortfolioId,
            businessUnit,
            cancellationToken);
}
