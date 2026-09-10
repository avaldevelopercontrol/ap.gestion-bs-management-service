using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
namespace GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;

public interface IPortfolioFilterOptionsRepository
{
    Task<PortfolioFilterOptionsDbResult?> GetFilterOptionsAsync(
        int crmClientId,
        CancellationToken cancellationToken);
}
