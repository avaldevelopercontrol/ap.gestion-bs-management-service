using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

namespace GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;

public interface IPortfolioPromisesService
{
    Task<PortfolioOperationResult<PortfolioPromisesResponse>> GetAsync(
        string? campaign,
        string? subPortfolioId,
        string? dateFrom,
        string? dateTo,
        string? businessUnit,
        int? crmClientId,
        CancellationToken cancellationToken);
}
