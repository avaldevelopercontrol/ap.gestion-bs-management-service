using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
namespace GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;

public interface IPortfolioDueTodayPromisesRepository
{
    Task<PortfolioDueTodayPromisesQueryResult> GetAsync(
        int clientKey,
        int campaignKey,
        long? subPortfolioId,
        string? businessUnit,
        int page,
        int pageSize,
        string? status,
        string sortBy,
        string sortDirection,
        CancellationToken cancellationToken);
}
