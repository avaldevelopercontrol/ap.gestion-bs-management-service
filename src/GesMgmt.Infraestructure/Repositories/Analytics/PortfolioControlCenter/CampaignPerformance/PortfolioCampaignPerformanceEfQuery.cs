using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal static class PortfolioCampaignPerformanceEfQuery
{
    private static readonly string[] FulfilledStatuses =
    [
        "FULFILLED",
        "PARTIAL",
        "FULFILLED_OUT_OF_RANGE"
    ];

    private static readonly string[] FulfillmentStatuses =
    [
        "FULFILLED",
        "PARTIAL",
        "FULFILLED_OUT_OF_RANGE",
        "BROKEN"
    ];

    public static async Task<IReadOnlyList<PortfolioCampaignPerformanceDbRow>?> GetAsync(
        AnalyticsDbContext context,
        int crmClientId,
        PortfolioCampaignPerformanceRequest request,
        CancellationToken cancellationToken)
    {
        var clientKey = await ResolveClientKeyAsync(
            context,
            crmClientId,
            request.BusinessUnit,
            cancellationToken);

        if (!clientKey.HasValue)
        {
            return null;
        }

        var eligibleCampaigns = BuildEligibleCampaigns(
            context,
            clientKey.Value,
            request);

        var ranges = await eligibleCampaigns
            .OrderByDescending(row => row.DateTo)
            .ThenByDescending(row => row.CampaignCode)
            .ToListAsync(cancellationToken);

        if (ranges.Count == 0)
        {
            return [];
        }

        var snapshotDates = await BuildSnapshotRows(
                context,
                clientKey.Value,
                request,
                eligibleCampaigns)
            .GroupBy(row => row.CampaignKey)
            .Select(group => new CampaignSnapshotDate(
                group.Key,
                group.Max(row => row.CalendarDate)))
            .ToDictionaryAsync(
                row => row.CampaignKey,
                row => row.SnapshotDate,
                cancellationToken);

        if (snapshotDates.Count == 0)
        {
            return [];
        }

        var snapshotMetrics = await LoadSnapshotMetricsAsync(
            context,
            clientKey.Value,
            request,
            snapshotDates,
            cancellationToken);

        var flowMetrics = await LoadFlowMetricsAsync(
            context,
            clientKey.Value,
            request,
            eligibleCampaigns,
            cancellationToken);

        var contactMetrics = await LoadContactMetricsAsync(
            context,
            clientKey.Value,
            request,
            eligibleCampaigns,
            cancellationToken);

        var promiseMetrics = await LoadPromiseMetricsAsync(
            context,
            clientKey.Value,
            request,
            eligibleCampaigns,
            cancellationToken);

        var paymentMetrics = await LoadPaymentMetricsAsync(
            context,
            clientKey.Value,
            request,
            eligibleCampaigns,
            cancellationToken);

        var includeClientLevelTarget =
            PortfolioBusinessUnitPolicy.CanUseClientLevelTarget(
                crmClientId,
                request.BusinessUnit);

        var targetMetrics = includeClientLevelTarget
            ? await LoadTargetMetricsAsync(
                context,
                clientKey.Value,
                eligibleCampaigns,
                cancellationToken)
            : new Dictionary<int, CampaignTargetMetrics>();

        var result = new List<PortfolioCampaignPerformanceDbRow>(ranges.Count);

        foreach (var range in ranges)
        {
            if (!snapshotMetrics.TryGetValue(range.CampaignKey, out var snapshot))
            {
                continue;
            }

            flowMetrics.TryGetValue(range.CampaignKey, out var flow);
            contactMetrics.TryGetValue(range.CampaignKey, out var contact);
            promiseMetrics.TryGetValue(range.CampaignKey, out var promise);
            paymentMetrics.TryGetValue(range.CampaignKey, out var payment);
            targetMetrics.TryGetValue(range.CampaignKey, out var target);

            result.Add(new PortfolioCampaignPerformanceDbRow
            {
                CampaignCode = range.CampaignCode,
                CampaignName = range.CampaignName,
                DateFrom = range.DateFrom,
                DateTo = range.DateTo,
                SnapshotDate = snapshot.SnapshotDate,
                AssignedPortfolio = snapshot.AssignedPortfolio,
                ManagedPortfolio = snapshot.ManagedPortfolio,
                PendingPortfolio = snapshot.PendingPortfolio,
                ProgressRate = Divide(
                    snapshot.ManagedPortfolio,
                    snapshot.AssignedPortfolio),
                ManagementCount = flow?.ManagementCount ?? 0,
                ContactabilityRate = Divide(
                    snapshot.ContactedPortfolio,
                    snapshot.AssignedPortfolio),
                RpcRate = Divide(
                    contact?.DirectContactClients ?? 0,
                    contact?.ClassifiableClients ?? 0),
                CloseRate = Divide(
                    promise?.ValidPromiseClients ?? 0,
                    contact?.DirectContactClients ?? 0),
                PromiseCount = promise?.PromiseCount ?? 0,
                PromiseFulfillmentRate = Divide(
                    promise?.FulfillmentPaidAmount ?? 0m,
                    promise?.FulfillmentPromiseAmount ?? 0m),
                PaymentCount = payment?.PaymentCount ?? 0,
                RecoveredAmount = Round(flow?.RecoveredAmount ?? 0m, 4),
                TargetAmount = request.SubPortfolioId is null && includeClientLevelTarget
                    ? target?.TargetAmount
                    : null,
                UpdatedAtUtc = MaxNullable(
                [
                    snapshot.LoadedAt,
                    flow?.LoadedAt,
                    contact?.LoadedAt,
                    promise?.LoadedAt,
                    payment?.LoadedAt,
                    target?.LoadedAt
                ])
            });
        }

        return result;
    }

    private static IQueryable<CampaignRange> BuildEligibleCampaigns(
        AnalyticsDbContext context,
        int clientKey,
        PortfolioCampaignPerformanceRequest request)
    {
        var requestedDateFrom = request.DateFrom?.ToDateTime(TimeOnly.MinValue);
        var requestedDateTo = request.DateTo?.ToDateTime(TimeOnly.MinValue);

        var selectedCampaigns =
            from campaign in context.AnalyticsCampaigns.AsNoTracking()
            join fact in context.AnalyticsPortfolioDailyFacts.AsNoTracking()
                on new { campaign.ClientKey, campaign.CampaignKey }
                equals new { fact.ClientKey, fact.CampaignKey }
            join date in context.AnalyticsDates.AsNoTracking()
                on fact.DateKey equals date.DateKey
            where campaign.ClientKey == clientKey
                  && (request.Campaign == null
                      || campaign.CampaignCode == request.Campaign)
                  && (!request.SubPortfolioId.HasValue
                      || fact.PortfolioKey == request.SubPortfolioId.Value)
                  && (request.BusinessUnit == null
                      || context.AnalyticsPortfolios.Any(portfolio =>
                          portfolio.PortfolioKey == fact.PortfolioKey
                          && portfolio.SourceBusinessUnit == request.BusinessUnit))
            group date by new
            {
                campaign.CampaignKey,
                campaign.CampaignCode,
                campaign.CampaignName
            }
            into grouped
            select new
            {
                grouped.Key.CampaignKey,
                grouped.Key.CampaignCode,
                grouped.Key.CampaignName,
                AvailableDateFrom = grouped.Min(row => row.CalendarDate),
                AvailableDateTo = grouped.Max(row => row.CalendarDate)
            };

        return selectedCampaigns
            .Select(row => new CampaignRange
            {
                CampaignKey = row.CampaignKey,
                CampaignCode = row.CampaignCode,
                CampaignName = row.CampaignName,
                DateFrom = !requestedDateFrom.HasValue
                    || requestedDateFrom.Value < row.AvailableDateFrom
                        ? row.AvailableDateFrom
                        : requestedDateFrom.Value,
                DateTo = !requestedDateTo.HasValue
                    || requestedDateTo.Value > row.AvailableDateTo
                        ? row.AvailableDateTo
                        : requestedDateTo.Value
            })
            .Where(row => row.DateFrom <= row.DateTo);
    }

    private static IQueryable<CampaignMetricRow> BuildSnapshotRows(
        AnalyticsDbContext context,
        int clientKey,
        PortfolioCampaignPerformanceRequest request,
        IQueryable<CampaignRange> eligibleCampaigns) =>
        from range in eligibleCampaigns
        join metric in context.AnalyticsPortfolioDailyMetrics.AsNoTracking()
            on range.CampaignKey equals metric.CampaignKey
        where metric.ClientKey == clientKey
              && metric.HasSourceSnapshot
              && metric.CalendarDate >= range.DateFrom
              && metric.CalendarDate <= range.DateTo
              && (!request.SubPortfolioId.HasValue
                  || metric.PortfolioKey == request.SubPortfolioId.Value)
              && (request.BusinessUnit == null
                  || context.AnalyticsPortfolios.Any(portfolio =>
                      portfolio.PortfolioKey == metric.PortfolioKey
                      && portfolio.SourceBusinessUnit == request.BusinessUnit))
        select new CampaignMetricRow
        {
            CampaignKey = metric.CampaignKey,
            PortfolioKey = metric.PortfolioKey,
            CalendarDate = metric.CalendarDate,
            AssignedClientsSnapshot = metric.AssignedClientsSnapshot,
            ManagedClientsSnapshot = metric.ManagedClientsSnapshot,
            PendingClientsSnapshot = metric.PendingClientsSnapshot,
            ContactedClientsSnapshot = metric.ContactedClientsSnapshot,
            ManagementEventsDay = metric.ManagementEventsDay,
            RecoveredAmountDay = metric.RecoveredAmountDay,
            LoadedAt = metric.LoadedAt
        };

    private static async Task<Dictionary<int, CampaignSnapshotMetrics>> LoadSnapshotMetricsAsync(
        AnalyticsDbContext context,
        int clientKey,
        PortfolioCampaignPerformanceRequest request,
        IReadOnlyDictionary<int, DateTime> snapshotDates,
        CancellationToken cancellationToken)
    {
        var campaignKeys = snapshotDates.Keys.ToArray();
        var rows = await context.AnalyticsPortfolioDailyMetrics
            .AsNoTracking()
            .Where(metric =>
                metric.ClientKey == clientKey
                && campaignKeys.Contains(metric.CampaignKey)
                && metric.HasSourceSnapshot
                && (!request.SubPortfolioId.HasValue
                    || metric.PortfolioKey == request.SubPortfolioId.Value)
                && (request.BusinessUnit == null
                    || context.AnalyticsPortfolios.Any(portfolio =>
                        portfolio.PortfolioKey == metric.PortfolioKey
                        && portfolio.SourceBusinessUnit == request.BusinessUnit)))
            .Select(metric => new CampaignMetricRow
            {
                CampaignKey = metric.CampaignKey,
                PortfolioKey = metric.PortfolioKey,
                CalendarDate = metric.CalendarDate,
                AssignedClientsSnapshot = metric.AssignedClientsSnapshot,
                ManagedClientsSnapshot = metric.ManagedClientsSnapshot,
                PendingClientsSnapshot = metric.PendingClientsSnapshot,
                ContactedClientsSnapshot = metric.ContactedClientsSnapshot,
                LoadedAt = metric.LoadedAt
            })
            .ToListAsync(cancellationToken);

        return rows
            .Where(row => snapshotDates.TryGetValue(row.CampaignKey, out var snapshotDate)
                && row.CalendarDate == snapshotDate)
            .GroupBy(row => row.CampaignKey)
            .ToDictionary(
                group => group.Key,
                group => new CampaignSnapshotMetrics(
                    snapshotDates[group.Key],
                    group.Sum(row => (long)(row.AssignedClientsSnapshot ?? 0)),
                    group.Sum(row => (long)(row.ManagedClientsSnapshot ?? 0)),
                    group.Sum(row => (long)(row.PendingClientsSnapshot ?? 0)),
                    group.Sum(row => (long)(row.ContactedClientsSnapshot ?? 0)),
                    group.Select(row => row.LoadedAt).Max()));
    }

    private static async Task<Dictionary<int, CampaignFlowMetrics>> LoadFlowMetricsAsync(
        AnalyticsDbContext context,
        int clientKey,
        PortfolioCampaignPerformanceRequest request,
        IQueryable<CampaignRange> eligibleCampaigns,
        CancellationToken cancellationToken)
    {
        var query =
            from range in eligibleCampaigns
            join metric in context.AnalyticsPortfolioDailyMetrics.AsNoTracking()
                on range.CampaignKey equals metric.CampaignKey
            where metric.ClientKey == clientKey
                  && metric.CalendarDate >= range.DateFrom
                  && metric.CalendarDate <= range.DateTo
                  && (!request.SubPortfolioId.HasValue
                      || metric.PortfolioKey == request.SubPortfolioId.Value)
                  && (request.BusinessUnit == null
                      || context.AnalyticsPortfolios.Any(portfolio =>
                          portfolio.PortfolioKey == metric.PortfolioKey
                          && portfolio.SourceBusinessUnit == request.BusinessUnit))
            group metric by range.CampaignKey
            into grouped
            select new CampaignFlowMetrics(
                grouped.Key,
                grouped.Sum(row => (long)(row.ManagementEventsDay ?? 0)),
                grouped.Sum(row => row.RecoveredAmountDay ?? 0m),
                grouped.Max(row => row.LoadedAt));

        return await query.ToDictionaryAsync(
            row => row.CampaignKey,
            cancellationToken);
    }

    private static async Task<Dictionary<int, CampaignContactMetrics>> LoadContactMetricsAsync(
        AnalyticsDbContext context,
        int clientKey,
        PortfolioCampaignPerformanceRequest request,
        IQueryable<CampaignRange> eligibleCampaigns,
        CancellationToken cancellationToken)
    {
        var contacts =
            from range in eligibleCampaigns
            join fact in context.AnalyticsDebtorContactDailyFacts.AsNoTracking()
                on range.CampaignKey equals fact.CampaignKey
            join date in context.AnalyticsDates.AsNoTracking()
                on fact.DateKey equals date.DateKey
            where fact.ClientKey == clientKey
                  && date.CalendarDate >= range.DateFrom
                  && date.CalendarDate <= range.DateTo
                  && (!request.SubPortfolioId.HasValue
                      || fact.PortfolioKey == request.SubPortfolioId.Value)
                  && (request.BusinessUnit == null
                      || context.AnalyticsPortfolios.Any(portfolio =>
                          portfolio.PortfolioKey == fact.PortfolioKey
                          && portfolio.SourceBusinessUnit == request.BusinessUnit))
            select new
            {
                range.CampaignKey,
                fact.PortfolioKey,
                fact.SourceDebtorId,
                fact.HadDirectContact,
                fact.HadIndirectContact,
                fact.HadNoContact,
                fact.LoadedAt
            };

        var directCounts = await contacts
            .Where(row => row.HadDirectContact)
            .Select(row => new
            {
                row.CampaignKey,
                row.PortfolioKey,
                row.SourceDebtorId
            })
            .Distinct()
            .GroupBy(row => row.CampaignKey)
            .Select(group => new CampaignCountMetric(
                group.Key,
                group.LongCount()))
            .ToDictionaryAsync(row => row.CampaignKey, cancellationToken);

        var classifiableCounts = await contacts
            .Where(row => row.HadDirectContact || row.HadIndirectContact || row.HadNoContact)
            .Select(row => new
            {
                row.CampaignKey,
                row.PortfolioKey,
                row.SourceDebtorId
            })
            .Distinct()
            .GroupBy(row => row.CampaignKey)
            .Select(group => new CampaignCountMetric(
                group.Key,
                group.LongCount()))
            .ToDictionaryAsync(row => row.CampaignKey, cancellationToken);

        var loadedAt = await contacts
            .GroupBy(row => row.CampaignKey)
            .Select(group => new CampaignLoadedAt(
                group.Key,
                group.Max(row => row.LoadedAt)))
            .ToDictionaryAsync(row => row.CampaignKey, cancellationToken);

        return loadedAt.Keys
            .Union(directCounts.Keys)
            .Union(classifiableCounts.Keys)
            .ToDictionary(
                campaignKey => campaignKey,
                campaignKey => new CampaignContactMetrics(
                    directCounts.GetValueOrDefault(campaignKey)?.Count ?? 0,
                    classifiableCounts.GetValueOrDefault(campaignKey)?.Count ?? 0,
                    loadedAt.GetValueOrDefault(campaignKey)?.LoadedAt));
    }

    private static async Task<Dictionary<int, CampaignPromiseMetrics>> LoadPromiseMetricsAsync(
        AnalyticsDbContext context,
        int clientKey,
        PortfolioCampaignPerformanceRequest request,
        IQueryable<CampaignRange> eligibleCampaigns,
        CancellationToken cancellationToken)
    {
        var promises =
            from range in eligibleCampaigns
            join promise in context.AnalyticsPromises.AsNoTracking()
                on range.CampaignKey equals promise.CampaignKey
            where promise.ClientKey == clientKey
                  && promise.IsValidPromise
                  && promise.ManagementAt >= range.DateFrom
                  && promise.ManagementAt < range.DateTo.AddDays(1)
                  && (!request.SubPortfolioId.HasValue
                      || promise.PortfolioKey == request.SubPortfolioId.Value)
                  && (request.BusinessUnit == null
                      || context.AnalyticsPortfolios.Any(portfolio =>
                          portfolio.PortfolioKey == promise.PortfolioKey
                          && portfolio.SourceBusinessUnit == request.BusinessUnit))
            select new
            {
                range.CampaignKey,
                promise.PortfolioKey,
                promise.SourceDebtorId,
                promise.StatusCode,
                promise.PaidAmount,
                promise.PromiseAmount,
                promise.LoadedAt
            };

        var totals = await promises
            .GroupBy(row => row.CampaignKey)
            .Select(group => new CampaignPromiseTotals(
                group.Key,
                group.LongCount(),
                group.Sum(row => FulfilledStatuses.Contains(row.StatusCode)
                    ? row.PaidAmount ?? 0m
                    : 0m),
                group.Sum(row => FulfillmentStatuses.Contains(row.StatusCode)
                    ? row.PromiseAmount ?? 0m
                    : 0m),
                group.Max(row => row.LoadedAt)))
            .ToDictionaryAsync(row => row.CampaignKey, cancellationToken);

        var debtorCounts = await promises
            .Select(row => new
            {
                row.CampaignKey,
                row.PortfolioKey,
                row.SourceDebtorId
            })
            .Distinct()
            .GroupBy(row => row.CampaignKey)
            .Select(group => new CampaignCountMetric(
                group.Key,
                group.LongCount()))
            .ToDictionaryAsync(row => row.CampaignKey, cancellationToken);

        return totals.Keys
            .Union(debtorCounts.Keys)
            .ToDictionary(
                campaignKey => campaignKey,
                campaignKey =>
                {
                    var totalsRow = totals.GetValueOrDefault(campaignKey);
                    return new CampaignPromiseMetrics(
                        totalsRow?.PromiseCount ?? 0,
                        debtorCounts.GetValueOrDefault(campaignKey)?.Count ?? 0,
                        totalsRow?.FulfillmentPaidAmount ?? 0m,
                        totalsRow?.FulfillmentPromiseAmount ?? 0m,
                        totalsRow?.LoadedAt);
                });
    }

    private static async Task<Dictionary<int, CampaignPaymentMetrics>> LoadPaymentMetricsAsync(
        AnalyticsDbContext context,
        int clientKey,
        PortfolioCampaignPerformanceRequest request,
        IQueryable<CampaignRange> eligibleCampaigns,
        CancellationToken cancellationToken)
    {
        var payments =
            from range in eligibleCampaigns
            join fact in context.AnalyticsDebtorPaymentDailyFacts.AsNoTracking()
                on range.CampaignKey equals fact.CampaignKey
            join date in context.AnalyticsDates.AsNoTracking()
                on fact.DateKey equals date.DateKey
            where fact.ClientKey == clientKey
                  && date.CalendarDate >= range.DateFrom
                  && date.CalendarDate <= range.DateTo
                  && (!request.SubPortfolioId.HasValue
                      || fact.PortfolioKey == request.SubPortfolioId.Value)
                  && (request.BusinessUnit == null
                      || context.AnalyticsPortfolios.Any(portfolio =>
                          portfolio.PortfolioKey == fact.PortfolioKey
                          && portfolio.SourceBusinessUnit == request.BusinessUnit))
            select new
            {
                range.CampaignKey,
                fact.PortfolioKey,
                fact.SourceDebtorId,
                fact.LoadedAt
            };

        var counts = await payments
            .Select(row => new
            {
                row.CampaignKey,
                row.PortfolioKey,
                row.SourceDebtorId
            })
            .Distinct()
            .GroupBy(row => row.CampaignKey)
            .Select(group => new CampaignCountMetric(
                group.Key,
                group.LongCount()))
            .ToDictionaryAsync(row => row.CampaignKey, cancellationToken);

        var loadedAt = await payments
            .GroupBy(row => row.CampaignKey)
            .Select(group => new CampaignLoadedAt(
                group.Key,
                group.Max(row => row.LoadedAt)))
            .ToDictionaryAsync(row => row.CampaignKey, cancellationToken);

        return counts.Keys
            .Union(loadedAt.Keys)
            .ToDictionary(
                campaignKey => campaignKey,
                campaignKey => new CampaignPaymentMetrics(
                    counts.GetValueOrDefault(campaignKey)?.Count ?? 0,
                    loadedAt.GetValueOrDefault(campaignKey)?.LoadedAt));
    }

    private static async Task<Dictionary<int, CampaignTargetMetrics>> LoadTargetMetricsAsync(
        AnalyticsDbContext context,
        int clientKey,
        IQueryable<CampaignRange> eligibleCampaigns,
        CancellationToken cancellationToken)
    {
        var targets =
            from range in eligibleCampaigns
            join target in context.AnalyticsTargetMonthlyFacts.AsNoTracking()
                on range.CampaignKey equals target.CampaignKey
            where target.ClientKey == clientKey
                  && target.PortfolioKey == null
            group target by range.CampaignKey
            into grouped
            select new CampaignTargetMetrics(
                grouped.Key,
                grouped.Max(row => row.TargetRecoveredAmount),
                grouped.Max(row => row.SourceAsOfAt));

        return await targets.ToDictionaryAsync(
            row => row.CampaignKey,
            cancellationToken);
    }

    private static async Task<int?> ResolveClientKeyAsync(
        AnalyticsDbContext context,
        int crmClientId,
        string? businessUnit,
        CancellationToken cancellationToken)
    {
        var clientKey = await context.AnalyticsClients
            .AsNoTracking()
            .Where(client => client.CrmClientId == crmClientId)
            .Select(client => (int?)client.ClientKey)
            .SingleOrDefaultAsync(cancellationToken);

        if (!clientKey.HasValue)
        {
            return null;
        }

        if (businessUnit is null)
        {
            var businessUnits = await (
                from fact in context.AnalyticsPortfolioDailyFacts.AsNoTracking()
                join portfolio in context.AnalyticsPortfolios.AsNoTracking()
                    on fact.PortfolioKey equals portfolio.PortfolioKey
                where fact.ClientKey == clientKey.Value
                select portfolio.SourceBusinessUnit)
                .Distinct()
                .ToListAsync(cancellationToken);

            var normalizedScopes = businessUnits
                .Select(NormalizeBusinessUnit)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(2)
                .Count();

            return normalizedScopes > 1 ? null : clientKey;
        }

        var hasBusinessUnit = await (
            from fact in context.AnalyticsPortfolioDailyFacts.AsNoTracking()
            join portfolio in context.AnalyticsPortfolios.AsNoTracking()
                on fact.PortfolioKey equals portfolio.PortfolioKey
            where fact.ClientKey == clientKey.Value
                  && portfolio.SourceBusinessUnit == businessUnit
            select fact)
            .AnyAsync(cancellationToken);

        return hasBusinessUnit ? clientKey : null;
    }

    private static string? NormalizeBusinessUnit(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();

    private static decimal? Divide(long numerator, long denominator) =>
        denominator == 0
            ? null
            : Round((decimal)numerator / denominator, 6);

    private static decimal? Divide(decimal numerator, decimal denominator) =>
        denominator == 0m
            ? null
            : Round(numerator / denominator, 6);

    private static decimal Round(decimal value, int decimals) =>
        decimal.Round(value, decimals, MidpointRounding.AwayFromZero);

    private static DateTime? MaxNullable(IEnumerable<DateTime?> values)
    {
        var populated = values
            .Where(value => value.HasValue)
            .Select(value => value!.Value)
            .ToArray();

        return populated.Length == 0
            ? null
            : populated.Max();
    }

    private sealed class CampaignRange
    {
        public int CampaignKey { get; init; }
        public string CampaignCode { get; init; } = string.Empty;
        public string CampaignName { get; init; } = string.Empty;
        public DateTime DateFrom { get; init; }
        public DateTime DateTo { get; init; }
    }

    private sealed class CampaignMetricRow
    {
        public int CampaignKey { get; init; }
        public long PortfolioKey { get; init; }
        public DateTime CalendarDate { get; init; }
        public int? AssignedClientsSnapshot { get; init; }
        public int? ManagedClientsSnapshot { get; init; }
        public int? PendingClientsSnapshot { get; init; }
        public int? ContactedClientsSnapshot { get; init; }
        public int? ManagementEventsDay { get; init; }
        public decimal? RecoveredAmountDay { get; init; }
        public DateTime? LoadedAt { get; init; }
    }

    private sealed record CampaignSnapshotDate(int CampaignKey, DateTime SnapshotDate);

    private sealed record CampaignSnapshotMetrics(
        DateTime SnapshotDate,
        long AssignedPortfolio,
        long ManagedPortfolio,
        long PendingPortfolio,
        long ContactedPortfolio,
        DateTime? LoadedAt);

    private sealed record CampaignFlowMetrics(
        int CampaignKey,
        long ManagementCount,
        decimal RecoveredAmount,
        DateTime? LoadedAt);

    private sealed record CampaignContactMetrics(
        long DirectContactClients,
        long ClassifiableClients,
        DateTime? LoadedAt);

    private sealed record CampaignPromiseMetrics(
        long PromiseCount,
        long ValidPromiseClients,
        decimal FulfillmentPaidAmount,
        decimal FulfillmentPromiseAmount,
        DateTime? LoadedAt);

    private sealed record CampaignPaymentMetrics(
        long PaymentCount,
        DateTime? LoadedAt);

    private sealed record CampaignTargetMetrics(
        int CampaignKey,
        decimal? TargetAmount,
        DateTime? LoadedAt);

    private sealed record CampaignPromiseTotals(
        int CampaignKey,
        long PromiseCount,
        decimal FulfillmentPaidAmount,
        decimal FulfillmentPromiseAmount,
        DateTime? LoadedAt);

    private sealed record CampaignCountMetric(int CampaignKey, long Count);

    private sealed record CampaignLoadedAt(int CampaignKey, DateTime? LoadedAt);
}
