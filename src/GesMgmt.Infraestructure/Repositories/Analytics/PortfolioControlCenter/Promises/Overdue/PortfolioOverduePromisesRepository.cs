using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence.Analytics;
using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal sealed class PortfolioOverduePromisesRepository(
    IAnalyticsQueryExecutor queryExecutor,
    AnalyticsDatabaseOptions databaseOptions)
    : IPortfolioOverduePromisesRepository
{
    private readonly int _commandTimeoutSeconds =
        databaseOptions.CommandTimeoutSeconds;

    public Task<PortfolioOverduePromisesQueryResult> GetAsync(
        int clientKey,
        int campaignKey,
        long? subPortfolioId,
        string? businessUnit,
        int page,
        int pageSize,
        CancellationToken cancellationToken) =>
        GetAsync(
            clientKey,
            campaignKey,
            subPortfolioId,
            businessUnit,
            page,
            pageSize,
            null,
            "overdueDays",
            "desc",
            cancellationToken);

    public async Task<PortfolioOverduePromisesQueryResult> GetAsync(
        int clientKey,
        int campaignKey,
        long? subPortfolioId,
        string? businessUnit,
        int page,
        int pageSize,
        string? aging,
        string sortBy,
        string sortDirection,
        CancellationToken cancellationToken)
    {
        var resultSets = await queryExecutor.QueryTwoAsync<
            PortfolioOverduePromisesMetadataDbRow,
            PortfolioOverduePromiseDbRow>(
            PortfolioOverduePromisesSql.Query,
            new
            {
                ClientKey = clientKey,
                CampaignKey = campaignKey,
                SubPortfolioId = subPortfolioId,
                BusinessUnit = businessUnit,
                Offset = ((long)page - 1) * pageSize,
                PageSize = pageSize,
                Aging = aging,
                SortBy = sortBy,
                SortDirection = sortDirection
            },
            _commandTimeoutSeconds,
            cancellationToken);

        var summaryMetadata = resultSets.First.Single(
            row => string.Equals(row.RowType, "summary", StringComparison.Ordinal));
        var filteredMetadata = resultSets.First.SingleOrDefault(
            row => string.Equals(row.RowType, "filtered", StringComparison.Ordinal))
            ?? summaryMetadata;

        var summary = new PortfolioOverduePromisesSummaryDbRow
        {
            OverdueCount = summaryMetadata.PromiseCount,
            OverdueAmount = summaryMetadata.PromiseAmount,
            OutstandingAmount = summaryMetadata.OutstandingAmount,
            AsOfDate = summaryMetadata.AsOfDate,
            UpdatedAtUtc = summaryMetadata.UpdatedAtUtc
        };

        var agingBuckets = resultSets.First
            .Where(row =>
                string.Equals(row.RowType, "aging", StringComparison.Ordinal)
                && row.AgingKey is not null)
            .Select(row => new PortfolioOverduePromisesAgingDbRow
            {
                AgingKey = row.AgingKey!,
                PromiseCount = row.PromiseCount,
                PromiseAmount = row.PromiseAmount,
                OutstandingAmount = row.OutstandingAmount
            })
            .ToArray();

        var advisors = resultSets.First
            .Where(row =>
                string.Equals(row.RowType, "advisor", StringComparison.Ordinal)
                && row.AdvisorId.HasValue
                && !string.IsNullOrWhiteSpace(row.AdvisorName))
            .Select(row => new PortfolioOverdueAdvisorFilterDbRow
            {
                AdvisorId = row.AdvisorId!.Value,
                AdvisorName = row.AdvisorName!.Trim()
            })
            .OrderBy(row => row.AdvisorName, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var supervisors = resultSets.First
            .Where(row =>
                string.Equals(row.RowType, "supervisor", StringComparison.Ordinal)
                && row.SupervisorId.HasValue
                && !string.IsNullOrWhiteSpace(row.SupervisorName))
            .Select(row => new PortfolioOverdueSupervisorFilterDbRow
            {
                SupervisorId = row.SupervisorId!.Value,
                SupervisorName = row.SupervisorName!.Trim()
            })
            .OrderBy(row => row.SupervisorName, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new PortfolioOverduePromisesQueryResult(
            summary,
            agingBuckets,
            resultSets.Second,
            advisors,
            supervisors,
            PortfolioPromisesPagination.Create(
                page,
                pageSize,
                filteredMetadata.PromiseCount));
    }
}
