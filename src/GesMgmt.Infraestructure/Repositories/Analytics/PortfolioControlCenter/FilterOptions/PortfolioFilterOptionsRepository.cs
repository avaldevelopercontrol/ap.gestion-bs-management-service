using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence;

namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal sealed class PortfolioFilterOptionsRepository(
    AnalyticsDbContext context)
    : IPortfolioFilterOptionsRepository
{
    public Task<PortfolioFilterOptionsDbResult?> GetFilterOptionsAsync(
        int crmClientId,
        CancellationToken cancellationToken) =>
        PortfolioFilterOptionsEfQuery.ExecuteAsync(
            context,
            crmClientId,
            cancellationToken);
}
