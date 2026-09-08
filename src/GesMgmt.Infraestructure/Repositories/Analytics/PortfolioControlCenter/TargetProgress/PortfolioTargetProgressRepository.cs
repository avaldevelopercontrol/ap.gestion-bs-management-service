using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence.Analytics;
using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal sealed class PortfolioTargetProgressRepository(
    IAnalyticsQueryExecutor queryExecutor,
    AnalyticsDatabaseOptions databaseOptions)
    : IPortfolioTargetProgressRepository
{
    private readonly int _commandTimeoutSeconds =
        databaseOptions.CommandTimeoutSeconds;

    public async Task<PortfolioTargetProgressContext?> ResolveContextAsync(
        int crmClientId,
        string? campaignCode,
        long? subPortfolioId,
        string? businessUnit,
        bool includeClientLevelTarget,
        CancellationToken cancellationToken)
    {
        var row = await queryExecutor.QuerySingleOrDefaultAsync<ContextDbRow>(
            PortfolioTargetProgressSql.ResolveContext,
            new
            {
                CrmClientId = crmClientId,
                CampaignCode = campaignCode,
                SubPortfolioId = subPortfolioId,
                BusinessUnit = businessUnit,
                IncludeClientLevelTarget = includeClientLevelTarget
            },
            _commandTimeoutSeconds,
            cancellationToken);

        if (row is null)
        {
            return null;
        }

        return new PortfolioTargetProgressContext(
            row.ClientKey,
            row.CampaignKey,
            row.CampaignCode,
            row.CampaignName,
            DateOnly.FromDateTime(row.StartDate),
            DateOnly.FromDateTime(row.EndDate),
            row.LatestProgressDate.HasValue
                ? DateOnly.FromDateTime(row.LatestProgressDate.Value)
                : null);
    }

    public Task<PortfolioTargetProgressDbRow?> GetTargetProgressAsync(
        int clientKey,
        int campaignKey,
        string? businessUnit,
        DateOnly dateTo,
        CancellationToken cancellationToken) =>
        queryExecutor.QuerySingleOrDefaultAsync<PortfolioTargetProgressDbRow>(
            PortfolioTargetProgressSql.TargetProgress,
            new
            {
                ClientKey = clientKey,
                CampaignKey = campaignKey,
                BusinessUnit = businessUnit,
                DateTo = dateTo.ToDateTime(TimeOnly.MinValue)
            },
            _commandTimeoutSeconds,
            cancellationToken);

    private sealed record ContextDbRow
    {
        public int ClientKey { get; init; }
        public int CampaignKey { get; init; }
        public required string CampaignCode { get; init; }
        public required string CampaignName { get; init; }
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public DateTime? LatestProgressDate { get; init; }
    }
}
