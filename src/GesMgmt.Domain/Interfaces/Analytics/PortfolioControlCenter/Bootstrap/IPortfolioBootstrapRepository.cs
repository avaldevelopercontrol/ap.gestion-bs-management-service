using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;

namespace GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;

public interface IPortfolioBootstrapRepository
{
    Task<PortfolioBootstrapSource?> ResolveAsync(
        int crmClientId,
        string? campaignCode,
        long? subPortfolioId,
        string? businessUnit,
        CancellationToken cancellationToken);
}
