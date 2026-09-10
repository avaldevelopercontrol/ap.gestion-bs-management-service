using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence;

namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal sealed class PortfolioSupervisorPerformanceRepository(AnalyticsDbContext context)
    : IPortfolioSupervisorPerformanceRepository
{
    public Task<IReadOnlyList<PortfolioSupervisorPerformanceDbRow>?> GetSupervisorPerformanceAsync(
        int crmClientId,
        PortfolioSupervisorPerformanceRequest request,
        CancellationToken cancellationToken) =>
        PortfolioPeoplePerformanceEfQuery.GetSupervisorsAsync(
            context,
            crmClientId,
            request,
            cancellationToken);
}
