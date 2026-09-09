using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence;

namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal sealed class PortfolioSummaryRepository(AnalyticsDbContext context)
    : IPortfolioSummaryRepository
{
    public async Task<PortfolioSummaryContext?> ResolveContextAsync(
        int crmClientId,
        string? campaignCode,
        long? subPortfolioId,
        string? businessUnit,
        CancellationToken cancellationToken)
    {
        var overviewContext = await PortfolioOverviewContextEfQuery.ExecuteAsync(
            context,
            crmClientId,
            campaignCode,
            subPortfolioId,
            businessUnit,
            cancellationToken);

        return overviewContext?.Summary;
    }

    public Task<PortfolioSummaryDbRow> GetSummaryAsync(
        int clientKey,
        int campaignKey,
        long? subPortfolioId,
        string? businessUnit,
        PortfolioSummaryRange range,
        CancellationToken cancellationToken) =>
        PortfolioSummaryEfQuery.ExecuteAsync(
            context,
            clientKey,
            campaignKey,
            subPortfolioId,
            businessUnit,
            range,
            cancellationToken);
}
