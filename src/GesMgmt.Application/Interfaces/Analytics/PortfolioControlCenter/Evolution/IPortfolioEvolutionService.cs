using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

namespace GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;

public interface IPortfolioEvolutionService
{
    Task<PortfolioOperationResult<PortfolioEvolutionResponse>> GetAsync(
        string? campaign,
        string? subPortfolioId,
        string? dateFrom,
        string? dateTo,
        string? businessUnit,
        int? crmClientId,
        CancellationToken cancellationToken);
}
