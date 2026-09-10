using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;

namespace GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;

public interface IPortfolioOverviewRepository
{
    Task<PortfolioOverviewContext?> ResolveContextAsync(
        int crmClientId,
        string? campaignCode,
        long? subPortfolioId,
        string? businessUnit,
        CancellationToken cancellationToken);

    Task<PortfolioOverviewDbRows> GetOverviewAsync(
        int clientKey,
        int campaignKey,
        long? subPortfolioId,
        string? businessUnit,
        bool includeClientLevelTarget,
        PortfolioSummaryRange range,
        CancellationToken cancellationToken);
}
