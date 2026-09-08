using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
namespace GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;

public interface IPortfolioOverduePromisesRepository
{
    Task<PortfolioOverduePromisesQueryResult> GetAsync(
        int clientKey,
        int campaignKey,
        long? subPortfolioId,
        string? businessUnit,
        int page,
        int pageSize,
        string? aging,
        string sortBy,
        string sortDirection,
        CancellationToken cancellationToken);
}
