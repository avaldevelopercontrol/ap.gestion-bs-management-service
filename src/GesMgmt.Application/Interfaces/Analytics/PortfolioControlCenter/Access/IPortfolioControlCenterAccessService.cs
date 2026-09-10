using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
namespace GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;

public interface IPortfolioControlCenterAccessService
{
    Task<PortfolioControlCenterClientAccess> ResolveClientAsync(
        int? requestedCrmClientId,
        CancellationToken cancellationToken);
}
