using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence.Analytics;
using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal sealed class PortfolioDueTodayPromisesRepository(
    IAnalyticsQueryExecutor queryExecutor,
    AnalyticsDatabaseOptions databaseOptions)
    : IPortfolioDueTodayPromisesRepository
{
    private readonly int _commandTimeoutSeconds =
        databaseOptions.CommandTimeoutSeconds;

    public Task<PortfolioDueTodayPromisesQueryResult> GetAsync(
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
            "outstandingAmount",
            "desc",
            cancellationToken);

    public async Task<PortfolioDueTodayPromisesQueryResult> GetAsync(
        int clientKey,
        int campaignKey,
        long? subPortfolioId,
        string? businessUnit,
        int page,
        int pageSize,
        string? status,
        string sortBy,
        string sortDirection,
        CancellationToken cancellationToken)
    {
        var resultSets = await queryExecutor.QueryTwoAsync<
            PortfolioDueTodayPromisesMetadataDbRow,
            PortfolioDueTodayPromiseDbRow>(
            PortfolioDueTodayPromisesSql.Query,
            new
            {
                ClientKey = clientKey,
                CampaignKey = campaignKey,
                SubPortfolioId = subPortfolioId,
                BusinessUnit = businessUnit,
                Offset = ((long)page - 1) * pageSize,
                PageSize = pageSize,
                Status = status,
                SortBy = sortBy,
                SortDirection = sortDirection
            },
            _commandTimeoutSeconds,
            cancellationToken);

        var summaryMetadata = resultSets.First.Single(
            row => row.StatusKey is null);
        var filteredMetadata = resultSets.First.SingleOrDefault(
            row => string.Equals(row.StatusKey, "__filtered__", StringComparison.Ordinal))
            ?? summaryMetadata;

        var summary = new PortfolioDueTodayPromisesSummaryDbRow
        {
            DueTodayCount = summaryMetadata.PromiseCount,
            DueTodayAmount = summaryMetadata.PromiseAmount,
            PaidAmount = summaryMetadata.PaidAmount,
            OutstandingAmount = summaryMetadata.OutstandingAmount,
            AsOfDate = summaryMetadata.AsOfDate,
            UpdatedAtUtc = summaryMetadata.UpdatedAtUtc
        };

        var statusBuckets = resultSets.First
            .Where(row => row.StatusKey is not null && row.StatusKey != "__filtered__")
            .Select(row => new PortfolioDueTodayPromisesStatusDbRow
            {
                StatusKey = row.StatusKey!,
                PromiseCount = row.PromiseCount,
                PromiseAmount = row.PromiseAmount,
                PaidAmount = row.PaidAmount,
                OutstandingAmount = row.OutstandingAmount
            })
            .ToArray();

        return new PortfolioDueTodayPromisesQueryResult(
            summary,
            statusBuckets,
            resultSets.Second,
            PortfolioPromisesPagination.Create(
                page,
                pageSize,
                filteredMetadata.PromiseCount));
    }
}
