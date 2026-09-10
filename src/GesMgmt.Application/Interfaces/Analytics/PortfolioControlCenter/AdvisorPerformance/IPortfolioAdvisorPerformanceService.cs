using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

namespace GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;

public interface IPortfolioAdvisorPerformanceService
{
    Task<PortfolioOperationResult<PortfolioAdvisorPerformanceResponse>> GetAsync(
        string? campaign,
        string? subPortfolioId,
        string? supervisorId,
        string? dateFrom,
        string? dateTo,
        string? businessUnit,
        int? crmClientId,
        CancellationToken cancellationToken);
}
