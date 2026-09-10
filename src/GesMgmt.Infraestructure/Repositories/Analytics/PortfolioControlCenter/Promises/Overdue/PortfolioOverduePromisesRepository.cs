using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal sealed class PortfolioOverduePromisesRepository(
    AnalyticsDbContext context)
    : IPortfolioOverduePromisesRepository
{
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
        var baseQuery = PortfolioPromiseDetailsEfQuery.ApplyScope(
            context,
            context.AnalyticsSupervisorPromiseOperational
                .AsNoTracking()
                .Where(row =>
                    row.ClientKey == clientKey
                    && row.CampaignKey == campaignKey
                    && row.IsValidPromise
                    && row.StatusCode == "BROKEN"),
            subPortfolioId,
            businessUnit);

        var totalCount = await baseQuery.LongCountAsync(cancellationToken);
        if (totalCount == 0)
        {
            return new PortfolioOverduePromisesQueryResult(
                new PortfolioOverduePromisesSummaryDbRow(),
                [],
                [],
                [],
                [],
                PortfolioPromisesPagination.Create(page, pageSize, 0));
        }

        var maxLoadedAt = await baseQuery
            .MaxAsync(row => row.LoadedAt, cancellationToken);
        var asOfDate = maxLoadedAt?.Date ?? DateTime.UtcNow.Date;

        var itemQuery = baseQuery.Select(row => new OverdueItemProjection
        {
            PromiseId = row.PromiseFactKey,
            DebtorId = row.SourceDebtorId,
            DueDate = row.PromiseDueDate,
            OverdueDays = row.PromiseDueDate == null
                ? null
                : EF.Functions.DateDiffDay(row.PromiseDueDate.Value, asOfDate) < 1
                    ? 1
                    : EF.Functions.DateDiffDay(row.PromiseDueDate.Value, asOfDate),
            PromiseAmount = row.PromiseAmount ?? 0m,
            PaidAmount = row.PaidAmount ?? 0m,
            OutstandingAmount = (row.PromiseAmount ?? 0m) > (row.PaidAmount ?? 0m)
                ? (row.PromiseAmount ?? 0m) - (row.PaidAmount ?? 0m)
                : 0m,
            AdvisorId = row.AdvisorKey,
            AdvisorName = row.AdvisorName == null || row.AdvisorName.Trim() == string.Empty
                ? null
                : row.AdvisorName.Trim(),
            SupervisorId = row.SupervisorKey,
            SupervisorName = row.SupervisorName == null || row.SupervisorName.Trim() == string.Empty
                ? null
                : row.SupervisorName.Trim(),
            AgingKey = row.PromiseDueDate == null
                ? "unclassified"
                : EF.Functions.DateDiffDay(row.PromiseDueDate.Value, asOfDate) >= 1
                    && EF.Functions.DateDiffDay(row.PromiseDueDate.Value, asOfDate) <= 3
                        ? "1-3"
                        : EF.Functions.DateDiffDay(row.PromiseDueDate.Value, asOfDate) >= 4
                            && EF.Functions.DateDiffDay(row.PromiseDueDate.Value, asOfDate) <= 7
                                ? "4-7"
                                : "8-plus",
            UpdatedAtUtc = row.LoadedAt
        });

        var summaryData = await itemQuery
            .GroupBy(_ => 1)
            .Select(group => new
            {
                PromiseCount = group.LongCount(),
                PromiseAmount = group.Sum(row => row.PromiseAmount),
                OutstandingAmount = group.Sum(row => row.OutstandingAmount),
                UpdatedAtUtc = group.Max(row => row.UpdatedAtUtc)
            })
            .SingleAsync(cancellationToken);

        var summary = new PortfolioOverduePromisesSummaryDbRow
        {
            OverdueCount = summaryData.PromiseCount,
            OverdueAmount = PortfolioPromiseDetailsEfQuery.RoundAmount(summaryData.PromiseAmount),
            OutstandingAmount = PortfolioPromiseDetailsEfQuery.RoundAmount(summaryData.OutstandingAmount),
            AsOfDate = asOfDate,
            UpdatedAtUtc = summaryData.UpdatedAtUtc
        };

        var agingRows = await itemQuery
            .GroupBy(row => row.AgingKey)
            .Select(group => new
            {
                AgingKey = group.Key,
                PromiseCount = group.LongCount(),
                PromiseAmount = group.Sum(row => row.PromiseAmount),
                OutstandingAmount = group.Sum(row => row.OutstandingAmount)
            })
            .ToListAsync(cancellationToken);

        var agingBuckets = agingRows
            .Select(row => new PortfolioOverduePromisesAgingDbRow
            {
                AgingKey = row.AgingKey,
                PromiseCount = row.PromiseCount,
                PromiseAmount = PortfolioPromiseDetailsEfQuery.RoundAmount(row.PromiseAmount),
                OutstandingAmount = PortfolioPromiseDetailsEfQuery.RoundAmount(row.OutstandingAmount)
            })
            .ToArray();

        var advisors = await itemQuery
            .Where(row => row.AdvisorId.HasValue && row.AdvisorName != null)
            .GroupBy(row => row.AdvisorId!.Value)
            .Select(group => new PortfolioOverdueAdvisorFilterDbRow
            {
                AdvisorId = group.Key,
                AdvisorName = group.Max(row => row.AdvisorName)!
            })
            .OrderBy(row => row.AdvisorName)
            .ToArrayAsync(cancellationToken);

        var supervisors = await itemQuery
            .Where(row => row.SupervisorId.HasValue && row.SupervisorName != null)
            .GroupBy(row => row.SupervisorId!.Value)
            .Select(group => new PortfolioOverdueSupervisorFilterDbRow
            {
                SupervisorId = group.Key,
                SupervisorName = group.Max(row => row.SupervisorName)!
            })
            .OrderBy(row => row.SupervisorName)
            .ToArrayAsync(cancellationToken);

        if (aging is not null)
        {
            itemQuery = itemQuery.Where(row => row.AgingKey == aging);
        }

        var filteredCount = aging is null
            ? totalCount
            : await itemQuery.LongCountAsync(cancellationToken);

        var orderedItems = ApplySort(itemQuery, sortBy, sortDirection)
            .ThenBy(row => row.PromiseId);

        var itemRows = await orderedItems
            .Skip(PortfolioPromiseDetailsEfQuery.GetOffset(page, pageSize))
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = itemRows
            .Select(row => new PortfolioOverduePromiseDbRow
            {
                PromiseId = row.PromiseId,
                DebtorId = row.DebtorId,
                DueDate = row.DueDate,
                OverdueDays = row.OverdueDays,
                PromiseAmount = PortfolioPromiseDetailsEfQuery.RoundAmount(row.PromiseAmount),
                PaidAmount = PortfolioPromiseDetailsEfQuery.RoundAmount(row.PaidAmount),
                OutstandingAmount = PortfolioPromiseDetailsEfQuery.RoundAmount(row.OutstandingAmount),
                AdvisorId = row.AdvisorId,
                AdvisorName = PortfolioPromiseDetailsEfQuery.NormalizeName(row.AdvisorName),
                SupervisorId = row.SupervisorId,
                SupervisorName = PortfolioPromiseDetailsEfQuery.NormalizeName(row.SupervisorName),
                AgingKey = row.AgingKey,
                AsOfDate = asOfDate,
                UpdatedAtUtc = row.UpdatedAtUtc
            })
            .ToArray();

        return new PortfolioOverduePromisesQueryResult(
            summary,
            agingBuckets,
            items,
            advisors,
            supervisors,
            PortfolioPromisesPagination.Create(page, pageSize, filteredCount));
    }

    private static IOrderedQueryable<OverdueItemProjection> ApplySort(
        IQueryable<OverdueItemProjection> query,
        string sortBy,
        string sortDirection)
    {
        var ascending = string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase);

        return sortBy.ToLowerInvariant() switch
        {
            "debtorid" => ascending
                ? query.OrderBy(row => row.DebtorId)
                : query.OrderByDescending(row => row.DebtorId),
            "duedate" => ascending
                ? query.OrderBy(row => row.DueDate)
                : query.OrderByDescending(row => row.DueDate),
            "overduedays" => ascending
                ? query.OrderBy(row => row.OverdueDays)
                : query.OrderByDescending(row => row.OverdueDays),
            "promiseamount" => ascending
                ? query.OrderBy(row => row.PromiseAmount)
                : query.OrderByDescending(row => row.PromiseAmount),
            "paidamount" => ascending
                ? query.OrderBy(row => row.PaidAmount)
                : query.OrderByDescending(row => row.PaidAmount),
            "advisorname" => ascending
                ? query.OrderBy(row => row.AdvisorName)
                : query.OrderByDescending(row => row.AdvisorName),
            "supervisorname" => ascending
                ? query.OrderBy(row => row.SupervisorName)
                : query.OrderByDescending(row => row.SupervisorName),
            _ => ascending
                ? query.OrderBy(row => row.OutstandingAmount)
                : query.OrderByDescending(row => row.OutstandingAmount)
        };
    }

    private sealed class OverdueItemProjection
    {
        public long PromiseId { get; init; }
        public long DebtorId { get; init; }
        public DateTime? DueDate { get; init; }
        public int? OverdueDays { get; init; }
        public decimal PromiseAmount { get; init; }
        public decimal PaidAmount { get; init; }
        public decimal OutstandingAmount { get; init; }
        public int? AdvisorId { get; init; }
        public string? AdvisorName { get; init; }
        public int? SupervisorId { get; init; }
        public string? SupervisorName { get; init; }
        public string AgingKey { get; init; } = string.Empty;
        public DateTime? UpdatedAtUtc { get; init; }
    }
}
