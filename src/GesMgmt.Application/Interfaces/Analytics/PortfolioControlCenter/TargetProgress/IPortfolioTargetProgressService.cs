using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

namespace GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;

public interface IPortfolioTargetProgressService
{
    Task<PortfolioOperationResult<PortfolioTargetProgressResponse>> GetAsync(
        string? campaign,
        string? subPortfolioId,
        string? dateTo,
        string? businessUnit,
        int? crmClientId,
        CancellationToken cancellationToken);
}
