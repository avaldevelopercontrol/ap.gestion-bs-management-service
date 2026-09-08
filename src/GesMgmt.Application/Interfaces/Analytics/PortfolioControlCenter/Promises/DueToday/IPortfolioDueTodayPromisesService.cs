using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

namespace GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;

public interface IPortfolioDueTodayPromisesService
{
    Task<PortfolioOperationResult<PortfolioDueTodayPromisesResponse>> GetAsync(
        string? campaign,
        string? subPortfolioId,
        string? page,
        string? pageSize,
        string? status,
        string? sortBy,
        string? sortDirection,
        string? businessUnit,
        int? crmClientId,
        CancellationToken cancellationToken);
}
