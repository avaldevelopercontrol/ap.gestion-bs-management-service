using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
namespace GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;

public interface IPortfolioAdvisorPerformanceRepository
{
    Task<IReadOnlyList<PortfolioAdvisorPerformanceDbRow>?> GetAdvisorPerformanceAsync(
        int crmClientId,
        PortfolioAdvisorPerformanceRequest request,
        CancellationToken cancellationToken);
}
