using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence;

namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal sealed class PortfolioEvolutionRepository(AnalyticsDbContext context)
    : IPortfolioEvolutionRepository
{
    public Task<PortfolioEvolutionContext?> ResolveContextAsync(
        int crmClientId,
        string? campaignCode,
        long? subPortfolioId,
        string? businessUnit,
        CancellationToken cancellationToken) =>
        PortfolioEvolutionEfQuery.ResolveContextAsync(
            context,
            crmClientId,
            campaignCode,
            subPortfolioId,
            businessUnit,
            cancellationToken);

    public Task<IReadOnlyList<PortfolioEvolutionDbRow>> GetEvolutionAsync(
        int clientKey,
        int campaignKey,
        long? subPortfolioId,
        string? businessUnit,
        PortfolioEvolutionRange range,
        CancellationToken cancellationToken) =>
        PortfolioEvolutionEfQuery.GetAsync(
            context,
            clientKey,
            campaignKey,
            subPortfolioId,
            businessUnit,
            range,
            cancellationToken);
}
