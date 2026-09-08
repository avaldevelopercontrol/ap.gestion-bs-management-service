using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
namespace GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;

public interface IPortfolioCampaignPerformanceRepository
{
    Task<IReadOnlyList<PortfolioCampaignPerformanceDbRow>?> GetCampaignPerformanceAsync(
        int crmClientId,
        PortfolioCampaignPerformanceRequest request,
        CancellationToken cancellationToken);
}
