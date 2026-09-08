using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence.Analytics;
using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal sealed class PortfolioOverviewRepository(
    IAnalyticsQueryExecutor queryExecutor,
    AnalyticsDatabaseOptions databaseOptions)
    : IPortfolioOverviewRepository
{
    private readonly int _commandTimeoutSeconds =
        databaseOptions.CommandTimeoutSeconds;

    public async Task<PortfolioOverviewContext?> ResolveContextAsync(
        int crmClientId,
        string? campaignCode,
        long? subPortfolioId,
        string? businessUnit,
        CancellationToken cancellationToken)
    {
        var row = await queryExecutor.QuerySingleOrDefaultAsync<ContextDbRow>(
            PortfolioOverviewSql.ResolveContext,
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

        return new PortfolioOverviewContext(
            new PortfolioSummaryContext(
                row.ClientKey,
                row.CampaignKey,
                row.CampaignCode,
                row.CampaignName,
                DateOnly.FromDateTime(row.StartDate),
                DateOnly.FromDateTime(row.EndDate),
                row.LatestDataDate.HasValue
                    ? DateOnly.FromDateTime(row.LatestDataDate.Value)
                    : null),
            row.OperationalSubPortfolioAvailable);
    }

    public async Task<PortfolioOverviewDbRows> GetOverviewAsync(
        int clientKey,
        int campaignKey,
        long? subPortfolioId,
        string? businessUnit,
        bool includeClientLevelTarget,
        PortfolioSummaryRange range,
        CancellationToken cancellationToken)
    {
        var dateFrom = range.DateFrom.ToDateTime(TimeOnly.MinValue);
        var dateTo = range.DateTo.ToDateTime(TimeOnly.MinValue);
        var dateToExclusive = range.DateTo
            .AddDays(1)
            .ToDateTime(TimeOnly.MinValue);

        var results = await queryExecutor.QueryFourAsync<
            PortfolioSummaryDbRow,
            PortfolioTargetProgressDbRow,
            PortfolioPromisesDbRow,
            PortfolioEvolutionDbRow>(
                PortfolioOverviewSql.Query,
                new
                {
                    ClientKey = clientKey,
                    CampaignKey = campaignKey,
                    SubPortfolioId = subPortfolioId,
                    BusinessUnit = businessUnit,
                    IncludeClientLevelTarget = includeClientLevelTarget,
                    DateFrom = dateFrom,
                    DateTo = dateTo,
                    DateToExclusive = dateToExclusive
                },
                _commandTimeoutSeconds,
                cancellationToken);

        return new PortfolioOverviewDbRows(
            results.First.Single(),
            results.Second.SingleOrDefault(),
            results.Third.Single(),
            results.Fourth);
    }

    private sealed record ContextDbRow
    {
        public int ClientKey { get; init; }
        public int CampaignKey { get; init; }
        public required string CampaignCode { get; init; }
        public required string CampaignName { get; init; }
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public DateTime? LatestDataDate { get; init; }
        public bool OperationalSubPortfolioAvailable { get; init; }
    }
}
