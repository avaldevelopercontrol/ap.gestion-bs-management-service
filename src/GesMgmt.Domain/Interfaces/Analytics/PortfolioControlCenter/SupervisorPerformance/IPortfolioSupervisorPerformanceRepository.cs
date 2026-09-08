using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
namespace GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;

public interface IPortfolioSupervisorPerformanceRepository
{
    Task<IReadOnlyList<PortfolioSupervisorPerformanceDbRow>?> GetSupervisorPerformanceAsync(
        int crmClientId,
        PortfolioSupervisorPerformanceRequest request,
        CancellationToken cancellationToken);
}
