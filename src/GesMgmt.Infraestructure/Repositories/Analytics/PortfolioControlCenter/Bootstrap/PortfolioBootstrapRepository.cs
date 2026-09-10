using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence;

namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal sealed class PortfolioBootstrapRepository(
    AnalyticsDbContext context)
    : IPortfolioBootstrapRepository
{
    public async Task<PortfolioBootstrapSource?> ResolveAsync(
        int crmClientId,
        string? campaignCode,
        long? subPortfolioId,
        string? businessUnit,
        CancellationToken cancellationToken)
    {
        var filterOptions = await PortfolioFilterOptionsEfQuery.ExecuteAsync(
            context,
            crmClientId,
            cancellationToken);

        if (filterOptions is null)
        {
            return null;
        }

        var overviewContext = await PortfolioOverviewContextEfQuery.ExecuteAsync(
            context,
            crmClientId,
            campaignCode,
            subPortfolioId,
            businessUnit,
            cancellationToken);

        return new PortfolioBootstrapSource(
            filterOptions,
            overviewContext);
    }
}
