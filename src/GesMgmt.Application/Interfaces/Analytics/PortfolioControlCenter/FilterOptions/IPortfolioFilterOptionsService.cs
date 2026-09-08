using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

namespace GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;

public interface IPortfolioFilterOptionsService
{
    Task<PortfolioOperationResult<PortfolioFilterOptionsResponse>> GetAsync(
        string? businessUnit,
        int? crmClientId,
        CancellationToken cancellationToken);
}
