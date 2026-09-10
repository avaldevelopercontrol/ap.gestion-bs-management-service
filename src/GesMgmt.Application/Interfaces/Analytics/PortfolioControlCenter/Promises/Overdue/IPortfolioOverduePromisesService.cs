using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

namespace GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;

public interface IPortfolioOverduePromisesService
{
    Task<PortfolioOperationResult<PortfolioOverduePromisesResponse>> GetAsync(
        string? campaign,
        string? subPortfolioId,
        string? page,
        string? pageSize,
        string? aging,
        string? sortBy,
        string? sortDirection,
        string? businessUnit,
        int? crmClientId,
        CancellationToken cancellationToken);
}
