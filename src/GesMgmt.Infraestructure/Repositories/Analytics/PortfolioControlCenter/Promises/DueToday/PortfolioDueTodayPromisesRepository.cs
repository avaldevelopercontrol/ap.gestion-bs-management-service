using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal sealed class PortfolioDueTodayPromisesRepository(
    AnalyticsDbContext context)
    : IPortfolioDueTodayPromisesRepository
{
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
        var baseQuery = PortfolioPromiseDetailsEfQuery.ApplyScope(
            context,
            context.AnalyticsPromiseOperational
                .AsNoTracking()
                .Where(row =>
                    row.ClientKey == clientKey
                    && row.CampaignKey == campaignKey
                    && row.IsValidPromise
                    && row.IsDueToday),
            subPortfolioId,
            businessUnit);

        var metricQuery = baseQuery.Select(row => new DueTodayMetricProjection
        {
            DueDate = row.PromiseDueDate,
            PromiseAmount = row.PromiseAmount ?? 0m,
            PaidAmount = row.PaidAmount ?? 0m,
            OutstandingAmount = (row.PromiseAmount ?? 0m) > (row.PaidAmount ?? 0m)
                ? (row.PromiseAmount ?? 0m) - (row.PaidAmount ?? 0m)
                : 0m,
            StatusKey = (row.PaidAmount ?? 0m) <= 0m
                ? "pending"
                : (row.PaidAmount ?? 0m) < (row.PromiseAmount ?? 0m)
                    ? "partial"
                    : "covered",
            UpdatedAtUtc = row.LoadedAt
        });

        var summaryData = await metricQuery
            .GroupBy(_ => 1)
            .Select(group => new
            {
                PromiseCount = group.LongCount(),
                PromiseAmount = group.Sum(row => row.PromiseAmount),
                PaidAmount = group.Sum(row => row.PaidAmount),
                OutstandingAmount = group.Sum(row => row.OutstandingAmount),
                AsOfDate = group.Max(row => row.DueDate),
                UpdatedAtUtc = group.Max(row => row.UpdatedAtUtc)
            })
            .SingleOrDefaultAsync(cancellationToken);

        var summary = summaryData is null
            ? new PortfolioDueTodayPromisesSummaryDbRow()
            : new PortfolioDueTodayPromisesSummaryDbRow
            {
                DueTodayCount = summaryData.PromiseCount,
                DueTodayAmount = PortfolioPromiseDetailsEfQuery.RoundAmount(summaryData.PromiseAmount),
                PaidAmount = PortfolioPromiseDetailsEfQuery.RoundAmount(summaryData.PaidAmount),
                OutstandingAmount = PortfolioPromiseDetailsEfQuery.RoundAmount(summaryData.OutstandingAmount),
                AsOfDate = summaryData.AsOfDate,
                UpdatedAtUtc = summaryData.UpdatedAtUtc
            };

        var statusRows = await metricQuery
            .GroupBy(row => row.StatusKey)
            .Select(group => new
            {
                StatusKey = group.Key,
                PromiseCount = group.LongCount(),
                PromiseAmount = group.Sum(row => row.PromiseAmount),
                PaidAmount = group.Sum(row => row.PaidAmount),
                OutstandingAmount = group.Sum(row => row.OutstandingAmount)
            })
            .ToListAsync(cancellationToken);

        var statusBuckets = statusRows
            .Select(row => new PortfolioDueTodayPromisesStatusDbRow
            {
                StatusKey = row.StatusKey,
                PromiseCount = row.PromiseCount,
                PromiseAmount = PortfolioPromiseDetailsEfQuery.RoundAmount(row.PromiseAmount),
                PaidAmount = PortfolioPromiseDetailsEfQuery.RoundAmount(row.PaidAmount),
                OutstandingAmount = PortfolioPromiseDetailsEfQuery.RoundAmount(row.OutstandingAmount)
            })
            .ToArray();

        var filteredCount = status is null
            ? summary.DueTodayCount
            : await metricQuery.LongCountAsync(
                row => row.StatusKey == status,
                cancellationToken);

        var itemQuery = baseQuery.Select(row => new DueTodayItemProjection
        {
            PromiseId = row.PromiseFactKey,
            DebtorId = row.SourceDebtorId,
            DueDate = row.PromiseDueDate,
            PromiseAmount = row.PromiseAmount ?? 0m,
            PaidAmount = row.PaidAmount ?? 0m,
            OutstandingAmount = (row.PromiseAmount ?? 0m) > (row.PaidAmount ?? 0m)
                ? (row.PromiseAmount ?? 0m) - (row.PaidAmount ?? 0m)
                : 0m,
            LastPaymentDate = row.LastPaymentDate,
            StatusKey = (row.PaidAmount ?? 0m) <= 0m
                ? "pending"
                : (row.PaidAmount ?? 0m) < (row.PromiseAmount ?? 0m)
                    ? "partial"
                    : "covered",
            StatusLabel = (row.PaidAmount ?? 0m) <= 0m
                ? "Pendiente"
                : (row.PaidAmount ?? 0m) < (row.PromiseAmount ?? 0m)
                    ? "Pago parcial"
                    : "Cubierta",
            AdvisorId = row.AdvisorKey,
            AdvisorName = context.AnalyticsSupervisorPromiseOperational
                .AsNoTracking()
                .Where(attribution =>
                    attribution.ClientKey == row.ClientKey
                    && attribution.CampaignKey == row.CampaignKey
                    && attribution.PromiseFactKey == row.PromiseFactKey)
                .OrderBy(attribution => attribution.SupervisorKey == null ? 1 : 0)
                .ThenByDescending(attribution => attribution.SupervisorAdvisorKey)
                .ThenByDescending(attribution => attribution.LoadedAt)
                .Select(attribution => attribution.AdvisorName == null
                    || attribution.AdvisorName.Trim() == string.Empty
                        ? null
                        : attribution.AdvisorName.Trim())
                .FirstOrDefault(),
            SupervisorId = context.AnalyticsSupervisorPromiseOperational
                .AsNoTracking()
                .Where(attribution =>
                    attribution.ClientKey == row.ClientKey
                    && attribution.CampaignKey == row.CampaignKey
                    && attribution.PromiseFactKey == row.PromiseFactKey)
                .OrderBy(attribution => attribution.SupervisorKey == null ? 1 : 0)
                .ThenByDescending(attribution => attribution.SupervisorAdvisorKey)
                .ThenByDescending(attribution => attribution.LoadedAt)
                .Select(attribution => attribution.SupervisorKey)
                .FirstOrDefault(),
            SupervisorName = context.AnalyticsSupervisorPromiseOperational
                .AsNoTracking()
                .Where(attribution =>
                    attribution.ClientKey == row.ClientKey
                    && attribution.CampaignKey == row.CampaignKey
                    && attribution.PromiseFactKey == row.PromiseFactKey)
                .OrderBy(attribution => attribution.SupervisorKey == null ? 1 : 0)
                .ThenByDescending(attribution => attribution.SupervisorAdvisorKey)
                .ThenByDescending(attribution => attribution.LoadedAt)
                .Select(attribution => attribution.SupervisorName == null
                    || attribution.SupervisorName.Trim() == string.Empty
                        ? null
                        : attribution.SupervisorName.Trim())
                .FirstOrDefault(),
            UpdatedAtUtc = row.LoadedAt
        });

        if (status is not null)
        {
            itemQuery = itemQuery.Where(row => row.StatusKey == status);
        }

        var orderedItems = ApplySort(itemQuery, sortBy, sortDirection)
            .ThenBy(row => row.PromiseId);

        var itemRows = await orderedItems
            .Skip(PortfolioPromiseDetailsEfQuery.GetOffset(page, pageSize))
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = itemRows
            .Select(row => new PortfolioDueTodayPromiseDbRow
            {
                PromiseId = row.PromiseId,
                DebtorId = row.DebtorId,
                DueDate = row.DueDate,
                PromiseAmount = PortfolioPromiseDetailsEfQuery.RoundAmount(row.PromiseAmount),
                PaidAmount = PortfolioPromiseDetailsEfQuery.RoundAmount(row.PaidAmount),
                OutstandingAmount = PortfolioPromiseDetailsEfQuery.RoundAmount(row.OutstandingAmount),
                LastPaymentDate = row.LastPaymentDate,
                StatusKey = row.StatusKey,
                AdvisorId = row.AdvisorId,
                AdvisorName = PortfolioPromiseDetailsEfQuery.NormalizeName(row.AdvisorName),
                SupervisorId = row.SupervisorId,
                SupervisorName = PortfolioPromiseDetailsEfQuery.NormalizeName(row.SupervisorName),
                UpdatedAtUtc = row.UpdatedAtUtc
            })
            .ToArray();

        return new PortfolioDueTodayPromisesQueryResult(
            summary,
            statusBuckets,
            items,
            PortfolioPromisesPagination.Create(page, pageSize, filteredCount));
    }

    private static IOrderedQueryable<DueTodayItemProjection> ApplySort(
        IQueryable<DueTodayItemProjection> query,
        string sortBy,
        string sortDirection)
    {
        var ascending = string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase);

        return sortBy.ToLowerInvariant() switch
        {
            "debtorid" => ascending
                ? query.OrderBy(row => row.DebtorId)
                : query.OrderByDescending(row => row.DebtorId),
            "promiseamount" => ascending
                ? query.OrderBy(row => row.PromiseAmount)
                : query.OrderByDescending(row => row.PromiseAmount),
            "paidamount" => ascending
                ? query.OrderBy(row => row.PaidAmount)
                : query.OrderByDescending(row => row.PaidAmount),
            "statuslabel" => ascending
                ? query.OrderBy(row => row.StatusLabel)
                : query.OrderByDescending(row => row.StatusLabel),
            "lastpaymentdate" => ascending
                ? query.OrderBy(row => row.LastPaymentDate)
                : query.OrderByDescending(row => row.LastPaymentDate),
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

    private sealed class DueTodayMetricProjection
    {
        public DateTime? DueDate { get; init; }
        public decimal PromiseAmount { get; init; }
        public decimal PaidAmount { get; init; }
        public decimal OutstandingAmount { get; init; }
        public string StatusKey { get; init; } = string.Empty;
        public DateTime? UpdatedAtUtc { get; init; }
    }

    private sealed class DueTodayItemProjection
    {
        public long PromiseId { get; init; }
        public long DebtorId { get; init; }
        public DateTime? DueDate { get; init; }
        public decimal PromiseAmount { get; init; }
        public decimal PaidAmount { get; init; }
        public decimal OutstandingAmount { get; init; }
        public DateTime? LastPaymentDate { get; init; }
        public string StatusKey { get; init; } = string.Empty;
        public string StatusLabel { get; init; } = string.Empty;
        public int? AdvisorId { get; init; }
        public string? AdvisorName { get; init; }
        public int? SupervisorId { get; init; }
        public string? SupervisorName { get; init; }
        public DateTime? UpdatedAtUtc { get; init; }
    }
}
