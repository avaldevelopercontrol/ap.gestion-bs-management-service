using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal static class PortfolioSummaryEfQuery
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

    private static readonly string[] FreshnessSourceCodes =
    [
        "CLARO_INTRADAY_UPSTREAM",
        "GESTION_COB2_LIVE",
        "CLARO_ADVISOR_DAILY",
        "CLARO_PORTFOLIO_SNAPSHOT"
    ];

    public static async Task<PortfolioSummaryDbRow> ExecuteAsync(
        AnalyticsDbContext context,
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

        var snapshot = await GetSnapshotAsync(
            context,
            clientKey,
            campaignKey,
            subPortfolioId,
            businessUnit,
            dateFrom,
            dateToExclusive,
            cancellationToken);

        var flow = await GetFlowAsync(
            context,
            clientKey,
            campaignKey,
            subPortfolioId,
            businessUnit,
            dateFrom,
            dateToExclusive,
            cancellationToken);

        var contact = await GetContactMetricsAsync(
            context,
            clientKey,
            campaignKey,
            subPortfolioId,
            businessUnit,
            dateFrom,
            dateToExclusive,
            cancellationToken);

        var promise = await GetPromiseMetricsAsync(
            context,
            clientKey,
            campaignKey,
            subPortfolioId,
            businessUnit,
            dateFrom,
            dateToExclusive,
            cancellationToken);

        var payment = await GetPaymentMetricsAsync(
            context,
            clientKey,
            campaignKey,
            subPortfolioId,
            businessUnit,
            dateFrom,
            dateToExclusive,
            cancellationToken);

        var freshness = await GetFreshnessAsync(context, cancellationToken);

        var updatedAtUtc = new DateTime?[]
        {
            snapshot.LoadedAtUtc,
            flow.LoadedAtUtc,
            contact.LoadedAtUtc,
            promise.LoadedAtUtc,
            payment.LoadedAtUtc
        }.Max();

        return new PortfolioSummaryDbRow
        {
            SnapshotDate = snapshot.SnapshotDate,
            AssignedPortfolio = snapshot.AssignedPortfolio,
            ManagedPortfolio = snapshot.ManagedPortfolio,
            PendingPortfolio = snapshot.PendingPortfolio,
            ManagementCount = flow.ManagementCount,
            ManagementIntensity = Divide(flow.ManagementCount, snapshot.ManagedPortfolio),
            RecoveredAmount = Round(flow.RecoveredAmount, 4),
            ContactabilityRate = snapshot.ContactedPortfolio.HasValue
                ? Divide(snapshot.ContactedPortfolio.Value, snapshot.AssignedPortfolio)
                : null,
            RpcRate = Divide(contact.DirectContactClients, contact.ClassifiableClients),
            CloseRate = Divide(promise.ValidPromiseClients, contact.DirectContactClients),
            PromiseCount = promise.PromiseCount,
            PromiseFulfillmentRate = Divide(
                promise.FulfillmentPaidAmount,
                promise.FulfillmentPromiseAmount),
            PaymentCount = payment.PaymentCount,
            UpdatedAtUtc = updatedAtUtc,
            OperationAsOfLocal = freshness.OperationAsOfLocal,
            PortfolioBaseRefreshedAtUtc = freshness.PortfolioBaseRefreshedAtUtc,
            RefreshedAtUtc = freshness.RefreshedAtUtc
        };
    }

    private static async Task<SnapshotMetrics> GetSnapshotAsync(
        AnalyticsDbContext context,
        int clientKey,
        int campaignKey,
        long? subPortfolioId,
        string? businessUnit,
        DateTime dateFrom,
        DateTime dateToExclusive,
        CancellationToken cancellationToken)
    {
        var realSnapshot = await (
            from fact in context.AnalyticsPortfolioDailyFacts.AsNoTracking()
            join date in context.AnalyticsDates.AsNoTracking()
                on fact.DateKey equals date.DateKey
            where fact.ClientKey == clientKey
                && fact.CampaignKey == campaignKey
                && fact.HasSourceSnapshot
                && date.CalendarDate >= dateFrom
                && date.CalendarDate < dateToExclusive
                && (!subPortfolioId.HasValue || fact.PortfolioKey == subPortfolioId.Value)
                && (businessUnit == null || context.AnalyticsPortfolios.Any(
                    portfolio =>
                        portfolio.PortfolioKey == fact.PortfolioKey
                        && portfolio.SourceBusinessUnit == businessUnit))
            group fact by date.CalendarDate
            into grouped
            orderby grouped.Key descending
            select new SnapshotMetrics(
                grouped.Key,
                (long)(grouped.Sum(row => row.AssignedClientsSnapshot) ?? 0),
                (long)(grouped.Sum(row => row.ManagedClientsSnapshot) ?? 0),
                (long)(grouped.Sum(row => row.PendingClientsSnapshot) ?? 0),
                grouped.Any(row => row.ContactedClientsSnapshot.HasValue)
                    ? (long?)grouped.Sum(row => row.ContactedClientsSnapshot)
                    : null,
                grouped.Max(row => row.LoadedAt)))
            .FirstOrDefaultAsync(cancellationToken);

        if (realSnapshot is not null)
        {
            return realSnapshot;
        }

        var fallbackSnapshot = await (
            from fact in context.AnalyticsPortfolioEvolutionDailyFacts.AsNoTracking()
            join date in context.AnalyticsDates.AsNoTracking()
                on fact.DateKey equals date.DateKey
            where fact.ClientKey == clientKey
                && fact.CampaignKey == campaignKey
                && date.CalendarDate >= dateFrom
                && date.CalendarDate < dateToExclusive
                && (!subPortfolioId.HasValue || fact.PortfolioKey == subPortfolioId.Value)
                && (businessUnit == null || context.AnalyticsPortfolios.Any(
                    portfolio =>
                        portfolio.PortfolioKey == fact.PortfolioKey
                        && portfolio.SourceBusinessUnit == businessUnit))
            group fact by date.CalendarDate
            into grouped
            orderby grouped.Key descending
            select new SnapshotMetrics(
                grouped.Key,
                (long)(grouped.Sum(row => row.AssignedClients) ?? 0),
                (long)(grouped.Sum(row => row.ManagedClients) ?? 0),
                (long)(grouped.Sum(row => row.PendingClients) ?? 0),
                null,
                grouped.Max(row => row.LoadedAt)))
            .FirstOrDefaultAsync(cancellationToken);

        return fallbackSnapshot ?? SnapshotMetrics.Empty;
    }

    private static async Task<FlowMetrics> GetFlowAsync(
        AnalyticsDbContext context,
        int clientKey,
        int campaignKey,
        long? subPortfolioId,
        string? businessUnit,
        DateTime dateFrom,
        DateTime dateToExclusive,
        CancellationToken cancellationToken)
    {
        var metrics = context.AnalyticsPortfolioDailyMetrics
            .AsNoTracking()
            .Where(row =>
                row.ClientKey == clientKey
                && row.CampaignKey == campaignKey
                && row.CalendarDate >= dateFrom
                && row.CalendarDate < dateToExclusive
                && (!subPortfolioId.HasValue || row.PortfolioKey == subPortfolioId.Value)
                && (businessUnit == null || context.AnalyticsPortfolios.Any(
                    portfolio =>
                        portfolio.PortfolioKey == row.PortfolioKey
                        && portfolio.SourceBusinessUnit == businessUnit)));

        return await metrics
            .GroupBy(_ => 1)
            .Select(grouped => new FlowMetrics(
                (long)(grouped.Sum(row => row.ManagementEventsDay) ?? 0),
                grouped.Sum(row => row.RecoveredAmountDay) ?? 0m,
                grouped.Max(row => row.LoadedAt)))
            .SingleOrDefaultAsync(cancellationToken)
            ?? FlowMetrics.Empty;
    }

    private static async Task<ContactMetrics> GetContactMetricsAsync(
        AnalyticsDbContext context,
        int clientKey,
        int campaignKey,
        long? subPortfolioId,
        string? businessUnit,
        DateTime dateFrom,
        DateTime dateToExclusive,
        CancellationToken cancellationToken)
    {
        var contacts =
            from fact in context.AnalyticsDebtorContactDailyFacts.AsNoTracking()
            join date in context.AnalyticsDates.AsNoTracking()
                on fact.DateKey equals date.DateKey
            where fact.ClientKey == clientKey
                && fact.CampaignKey == campaignKey
                && date.CalendarDate >= dateFrom
                && date.CalendarDate < dateToExclusive
                && (!subPortfolioId.HasValue || fact.PortfolioKey == subPortfolioId.Value)
                && (businessUnit == null || context.AnalyticsPortfolios.Any(
                    portfolio =>
                        portfolio.PortfolioKey == fact.PortfolioKey
                        && portfolio.SourceBusinessUnit == businessUnit))
            select fact;

        var directContactClients = await contacts
            .Where(row => row.HadDirectContact)
            .Select(row => new { row.PortfolioKey, row.SourceDebtorId })
            .Distinct()
            .LongCountAsync(cancellationToken);

        var classifiableClients = await contacts
            .Where(row =>
                row.HadDirectContact
                || row.HadIndirectContact
                || row.HadNoContact)
            .Select(row => new { row.PortfolioKey, row.SourceDebtorId })
            .Distinct()
            .LongCountAsync(cancellationToken);

        var loadedAtUtc = await contacts
            .Select(row => row.LoadedAt)
            .MaxAsync(cancellationToken);

        return new ContactMetrics(
            directContactClients,
            classifiableClients,
            loadedAtUtc);
    }

    private static async Task<PromiseMetrics> GetPromiseMetricsAsync(
        AnalyticsDbContext context,
        int clientKey,
        int campaignKey,
        long? subPortfolioId,
        string? businessUnit,
        DateTime dateFrom,
        DateTime dateToExclusive,
        CancellationToken cancellationToken)
    {
        var promises = context.AnalyticsPromises
            .AsNoTracking()
            .Where(row =>
                row.ClientKey == clientKey
                && row.CampaignKey == campaignKey
                && row.IsValidPromise
                && row.ManagementAt >= dateFrom
                && row.ManagementAt < dateToExclusive
                && (!subPortfolioId.HasValue || row.PortfolioKey == subPortfolioId.Value)
                && (businessUnit == null || context.AnalyticsPortfolios.Any(
                    portfolio =>
                        portfolio.PortfolioKey == row.PortfolioKey
                        && portfolio.SourceBusinessUnit == businessUnit)));

        var aggregate = await promises
            .GroupBy(_ => 1)
            .Select(grouped => new
            {
                PromiseCount = grouped.LongCount(),
                FulfillmentPaidAmount = grouped.Sum(row =>
                    FulfilledStatuses.Contains(row.StatusCode)
                        ? row.PaidAmount ?? 0m
                        : 0m),
                FulfillmentPromiseAmount = grouped.Sum(row =>
                    FulfillmentStatuses.Contains(row.StatusCode)
                        ? row.PromiseAmount ?? 0m
                        : 0m),
                LoadedAtUtc = grouped.Max(row => row.LoadedAt)
            })
            .SingleOrDefaultAsync(cancellationToken);

        var validPromiseClients = await promises
            .Select(row => new { row.PortfolioKey, row.SourceDebtorId })
            .Distinct()
            .LongCountAsync(cancellationToken);

        return aggregate is null
            ? new PromiseMetrics(0L, validPromiseClients, 0m, 0m, null)
            : new PromiseMetrics(
                aggregate.PromiseCount,
                validPromiseClients,
                aggregate.FulfillmentPaidAmount,
                aggregate.FulfillmentPromiseAmount,
                aggregate.LoadedAtUtc);
    }

    private static async Task<PaymentMetrics> GetPaymentMetricsAsync(
        AnalyticsDbContext context,
        int clientKey,
        int campaignKey,
        long? subPortfolioId,
        string? businessUnit,
        DateTime dateFrom,
        DateTime dateToExclusive,
        CancellationToken cancellationToken)
    {
        var payments =
            from fact in context.AnalyticsDebtorPaymentDailyFacts.AsNoTracking()
            join date in context.AnalyticsDates.AsNoTracking()
                on fact.DateKey equals date.DateKey
            where fact.ClientKey == clientKey
                && fact.CampaignKey == campaignKey
                && date.CalendarDate >= dateFrom
                && date.CalendarDate < dateToExclusive
                && (!subPortfolioId.HasValue || fact.PortfolioKey == subPortfolioId.Value)
                && (businessUnit == null || context.AnalyticsPortfolios.Any(
                    portfolio =>
                        portfolio.PortfolioKey == fact.PortfolioKey
                        && portfolio.SourceBusinessUnit == businessUnit))
            select fact;

        var paymentCount = await payments
            .Select(row => new { row.PortfolioKey, row.SourceDebtorId })
            .Distinct()
            .LongCountAsync(cancellationToken);

        var loadedAtUtc = await payments
            .Select(row => row.LoadedAt)
            .MaxAsync(cancellationToken);

        return new PaymentMetrics(paymentCount, loadedAtUtc);
    }

    private static async Task<FreshnessMetrics> GetFreshnessAsync(
        AnalyticsDbContext context,
        CancellationToken cancellationToken)
    {
        var watermarks = await context.AnalyticsWatermarks
            .AsNoTracking()
            .Where(row => FreshnessSourceCodes.Contains(row.SourceCode))
            .ToListAsync(cancellationToken);

        var intraday = watermarks
            .Where(row => row.SourceCode == "CLARO_INTRADAY_UPSTREAM")
            .ToArray();

        var liveAndAdvisor = watermarks
            .Where(row =>
                row.SourceCode == "GESTION_COB2_LIVE"
                || row.SourceCode == "CLARO_ADVISOR_DAILY")
            .ToArray();

        var operationAsOfLocal = intraday
            .Select(row => row.LastSourceDateTime)
            .Max()
            ?? liveAndAdvisor
                .Select(row => row.LastSourceDateTime)
                .Min();

        var portfolioBaseRefreshedAtUtc = watermarks
            .Where(row => row.SourceCode == "CLARO_PORTFOLIO_SNAPSHOT")
            .Select(row => row.LastSuccessAt)
            .Max();

        var refreshedAtUtc = intraday
            .Select(row => row.LastSuccessAt)
            .Max()
            ?? liveAndAdvisor
                .Select(row => row.LastSuccessAt)
                .Max();

        return new FreshnessMetrics(
            operationAsOfLocal,
            portfolioBaseRefreshedAtUtc,
            refreshedAtUtc);
    }

    private static decimal? Divide(long numerator, long denominator)
    {
        if (denominator == 0)
        {
            return null;
        }

        return Round((decimal)numerator / denominator, 6);
    }

    private static decimal? Divide(decimal numerator, decimal denominator)
    {
        if (denominator == 0m)
        {
            return null;
        }

        return Round(numerator / denominator, 6);
    }

    private static decimal Round(decimal value, int decimals) =>
        decimal.Round(value, decimals, MidpointRounding.AwayFromZero);

    private sealed record SnapshotMetrics(
        DateTime? SnapshotDate,
        long AssignedPortfolio,
        long ManagedPortfolio,
        long PendingPortfolio,
        long? ContactedPortfolio,
        DateTime? LoadedAtUtc)
    {
        public static SnapshotMetrics Empty { get; } =
            new(null, 0L, 0L, 0L, null, null);
    }

    private sealed record FlowMetrics(
        long ManagementCount,
        decimal RecoveredAmount,
        DateTime? LoadedAtUtc)
    {
        public static FlowMetrics Empty { get; } = new(0L, 0m, null);
    }

    private sealed record ContactMetrics(
        long DirectContactClients,
        long ClassifiableClients,
        DateTime? LoadedAtUtc);

    private sealed record PromiseMetrics(
        long PromiseCount,
        long ValidPromiseClients,
        decimal FulfillmentPaidAmount,
        decimal FulfillmentPromiseAmount,
        DateTime? LoadedAtUtc);

    private sealed record PaymentMetrics(
        long PaymentCount,
        DateTime? LoadedAtUtc);

    private sealed record FreshnessMetrics(
        DateTime? OperationAsOfLocal,
        DateTime? PortfolioBaseRefreshedAtUtc,
        DateTime? RefreshedAtUtc);
}
