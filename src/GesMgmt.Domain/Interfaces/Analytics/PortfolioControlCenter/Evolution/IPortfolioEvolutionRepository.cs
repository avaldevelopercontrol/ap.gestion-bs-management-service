using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
namespace GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;

public interface IPortfolioEvolutionRepository
{
    Task<PortfolioEvolutionContext?> ResolveContextAsync(
        int crmClientId,
        string? campaignCode,
        long? subPortfolioId,
        string? businessUnit,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<PortfolioEvolutionDbRow>> GetEvolutionAsync(
        int clientKey,
        int campaignKey,
        long? subPortfolioId,
        string? businessUnit,
        PortfolioEvolutionRange range,
        CancellationToken cancellationToken);
}
