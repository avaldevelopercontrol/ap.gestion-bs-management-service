using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence.Analytics;
using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal sealed class PortfolioSummaryRepository(
    IAnalyticsQueryExecutor queryExecutor,
    AnalyticsDatabaseOptions databaseOptions)
    : IPortfolioSummaryRepository
{
    private readonly int _commandTimeoutSeconds =
        databaseOptions.CommandTimeoutSeconds;

    public async Task<PortfolioSummaryContext?> ResolveContextAsync(
        int crmClientId,
        string? campaignCode,
        long? subPortfolioId,
        string? businessUnit,
        CancellationToken cancellationToken)
    {
        var row = await queryExecutor.QuerySingleOrDefaultAsync<ContextDbRow>(
            PortfolioSummarySql.ResolveContext,
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

        return new PortfolioSummaryContext(
            row.ClientKey,
            row.CampaignKey,
            row.CampaignCode,
            row.CampaignName,
            DateOnly.FromDateTime(row.StartDate),
            DateOnly.FromDateTime(row.EndDate),
            row.LatestDataDate.HasValue
                ? DateOnly.FromDateTime(row.LatestDataDate.Value)
                : null);
    }

    public Task<PortfolioSummaryDbRow> GetSummaryAsync(
        int clientKey,
        int campaignKey,
        long? subPortfolioId,
        string? businessUnit,
        PortfolioSummaryRange range,
        CancellationToken cancellationToken)
    {
        var dateFrom = range.DateFrom.ToDateTime(TimeOnly.MinValue);
        var dateToExclusive = range.DateTo
            .AddDays(1)
            .ToDateTime(TimeOnly.MinValue);

        return queryExecutor.QuerySingleAsync<PortfolioSummaryDbRow>(
            PortfolioSummarySql.Summary,
            new
            {
                ClientKey = clientKey,
                CampaignKey = campaignKey,
                SubPortfolioId = subPortfolioId,
                BusinessUnit = businessUnit,
                DateFrom = dateFrom,
                DateToExclusive = dateToExclusive
            },
            _commandTimeoutSeconds,
            cancellationToken);
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
    }
}
