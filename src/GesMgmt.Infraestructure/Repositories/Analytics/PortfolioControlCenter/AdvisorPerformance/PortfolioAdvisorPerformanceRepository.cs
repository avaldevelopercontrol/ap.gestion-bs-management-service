using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence;

namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal sealed class PortfolioAdvisorPerformanceRepository(AnalyticsDbContext context)
    : IPortfolioAdvisorPerformanceRepository
{
    public Task<IReadOnlyList<PortfolioAdvisorPerformanceDbRow>?> GetAdvisorPerformanceAsync(
        int crmClientId,
        PortfolioAdvisorPerformanceRequest request,
        CancellationToken cancellationToken) =>
        PortfolioPeoplePerformanceEfQuery.GetAdvisorsAsync(
            context,
            crmClientId,
            request,
            cancellationToken);
}
