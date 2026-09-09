using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence;

namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal sealed class PortfolioCampaignPerformanceRepository(
    AnalyticsDbContext context)
    : IPortfolioCampaignPerformanceRepository
{
    public Task<IReadOnlyList<PortfolioCampaignPerformanceDbRow>?> GetCampaignPerformanceAsync(
        int crmClientId,
        PortfolioCampaignPerformanceRequest request,
        CancellationToken cancellationToken) =>
        PortfolioCampaignPerformanceEfQuery.GetAsync(
            context,
            crmClientId,
            request,
            cancellationToken);
}
