using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
namespace GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;

public interface IPortfolioTargetProgressRepository
{
    Task<PortfolioTargetProgressContext?> ResolveContextAsync(
        int crmClientId,
        string? campaignCode,
        long? subPortfolioId,
        string? businessUnit,
        bool includeClientLevelTarget,
        CancellationToken cancellationToken);

    Task<PortfolioTargetProgressDbRow?> GetTargetProgressAsync(
        int clientKey,
        int campaignKey,
        string? businessUnit,
        DateOnly dateTo,
        CancellationToken cancellationToken);
}
