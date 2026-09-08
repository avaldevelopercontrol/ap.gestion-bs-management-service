using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence.Analytics;
using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal sealed class PortfolioPromisesRepository(
    IAnalyticsQueryExecutor queryExecutor,
    AnalyticsDatabaseOptions databaseOptions)
    : IPortfolioPromisesRepository
{
    private readonly int _commandTimeoutSeconds =
        databaseOptions.CommandTimeoutSeconds;

    public async Task<PortfolioPromisesContext?> ResolveContextAsync(
        int crmClientId,
        string? campaignCode,
        long? subPortfolioId,
        string? businessUnit,
        CancellationToken cancellationToken)
    {
        var row = await queryExecutor.QuerySingleOrDefaultAsync<ContextDbRow>(
            PortfolioPromisesSql.ResolveContext,
            new
            {
                CrmClientId = crmClientId,
                CampaignCode = campaignCode,
                SubPortfolioId = subPortfolioId,
                BusinessUnit = businessUnit
            },
            _commandTimeoutSeconds,
            cancellationToken);

        if (row is null)
        {
            return null;
        }

        return new PortfolioPromisesContext(
            row.ClientKey,
            row.CampaignKey,
            row.CampaignCode,
            row.CampaignName);
    }

    public Task<PortfolioPromisesDbRow> GetOperationalPromisesAsync(
        int clientKey,
        int campaignKey,
        long? subPortfolioId,
        string? businessUnit,
        CancellationToken cancellationToken) =>
        queryExecutor.QuerySingleAsync<PortfolioPromisesDbRow>(
            PortfolioPromisesSql.OperationalPromises,
            new
            {
                ClientKey = clientKey,
                CampaignKey = campaignKey,
                SubPortfolioId = subPortfolioId,
                BusinessUnit = businessUnit
            },
            _commandTimeoutSeconds,
            cancellationToken);

    private sealed record ContextDbRow
    {
        public int ClientKey { get; init; }
        public int CampaignKey { get; init; }
        public required string CampaignCode { get; init; }
        public required string CampaignName { get; init; }
    }
}
