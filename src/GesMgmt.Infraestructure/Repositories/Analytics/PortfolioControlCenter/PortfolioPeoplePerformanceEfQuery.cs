using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal static class PortfolioPeoplePerformanceEfQuery
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

    public static async Task<IReadOnlyList<PortfolioAdvisorPerformanceDbRow>?> GetAdvisorsAsync(
        AnalyticsDbContext context,
        int crmClientId,
        PortfolioAdvisorPerformanceRequest request,
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

        var campaignKeys = await GetCampaignKeysAsync(
            context,
            clientKey.Value,
            request.Campaign,
            cancellationToken);

        if (campaignKeys.Length == 0)
        {
            return [];
        }

        var activity = BuildActivityQuery(
            context,
            clientKey.Value,
            campaignKeys,
            request.SubPortfolioId,
            request.BusinessUnit,
            request.SupervisorId,
            request.DateFrom,
            request.DateTo);

        var managementRows = await activity
            .GroupBy(row => row.AdvisorKey)
            .Select(group => new AdvisorManagementMetrics(
                group.Key,
                group.Max(row => row.AdvisorName),
                group.Min(row => row.CalendarDate),
                group.Max(row => row.CalendarDate),
                group.Sum(row => (long)(row.ManagementEvents ?? 0)),
                group.Sum(row => row.RecoveredAmount ?? 0m),
                group.Max(row => row.LoadedAt)))
            .ToListAsync(cancellationToken);

        if (managementRows.Count == 0)
        {
            return [];
        }

        var effectiveDateFrom = managementRows.Min(row => row.DateFrom);
        var effectiveDateTo = managementRows.Max(row => row.DateTo);

        var supervisorAssignments = await activity
            .Where(row => row.SupervisorKey.HasValue)
            .Select(row => new
            {
                row.AdvisorKey,
                SupervisorKey = row.SupervisorKey!.Value,
                row.SupervisorName
            })
            .Distinct()
            .ToListAsync(cancellationToken);

        var contactRows = await BuildContactQuery(
                context,
                clientKey.Value,
                campaignKeys,
                request.SubPortfolioId,
                request.BusinessUnit,
                request.SupervisorId,
                effectiveDateFrom,
                effectiveDateTo)
            .Where(row => row.AdvisorKey.HasValue)
            .GroupBy(row => new
            {
                AdvisorKey = row.AdvisorKey!.Value,
                row.CampaignKey,
                row.PortfolioKey,
                row.SourceDebtorId
            })
            .Select(group => new DebtorContactMetrics(
                group.Key.AdvisorKey,
                group.Any(row => row.HadDirectContact),
                group.Any(row => row.HadIndirectContact),
                group.Any(row => row.HadNoContact),
                group.Max(row => row.LoadedAt)))
            .ToListAsync(cancellationToken);

        var promiseRows = await BuildPromiseQuery(
                context,
                clientKey.Value,
                campaignKeys,
                request.SubPortfolioId,
                request.BusinessUnit,
                request.SupervisorId,
                effectiveDateFrom,
                effectiveDateTo)
            .Where(row => row.AdvisorKey.HasValue && row.IsValidPromise)
            .GroupBy(row => new
            {
                AdvisorKey = row.AdvisorKey!.Value,
                row.CampaignKey,
                row.PortfolioKey,
                row.SourceDebtorId
            })
            .Select(group => new DebtorPromiseMetrics(
                group.Key.AdvisorKey,
                group.LongCount(),
                0m,
                0m,
                group.Max(row => row.LoadedAt)))
            .ToListAsync(cancellationToken);

        var paymentRows = await BuildPaymentQuery(
                context,
                clientKey.Value,
                campaignKeys,
                request.SubPortfolioId,
                request.BusinessUnit,
                request.SupervisorId,
                effectiveDateFrom,
                effectiveDateTo)
            .Where(row => row.AdvisorKey.HasValue)
            .GroupBy(row => new
            {
                AdvisorKey = row.AdvisorKey!.Value,
                row.CampaignKey,
                row.PortfolioKey,
                row.SourceDebtorId
            })
            .Select(group => new DebtorPaymentMetrics(
                group.Key.AdvisorKey,
                group.Max(row => row.LoadedAt)))
            .ToListAsync(cancellationToken);

        var currentSupervisors = await context.AnalyticsAdvisorSupervisorCurrent
            .AsNoTracking()
            .Where(row => row.ClientKey == clientKey.Value)
            .ToListAsync(cancellationToken);

        var contactByAdvisor = contactRows
            .GroupBy(row => row.ActorKey)
            .ToDictionary(
                group => group.Key,
                group => new ContactMetrics(
                    group.LongCount(row => row.HadDirectContact),
                    group.LongCount(row =>
                        row.HadDirectContact
                        || row.HadIndirectContact
                        || row.HadNoContact),
                    MaxNullable(group.Select(row => row.LoadedAt))));

        var promiseByAdvisor = promiseRows
            .GroupBy(row => row.ActorKey)
            .ToDictionary(
                group => group.Key,
                group => new PromiseMetrics(
                    group.Sum(row => row.PromiseCount),
                    group.LongCount(),
                    0m,
                    0m,
                    MaxNullable(group.Select(row => row.LoadedAt))));

        var paymentByAdvisor = paymentRows
            .GroupBy(row => row.ActorKey)
            .ToDictionary(
                group => group.Key,
                group => new PaymentMetrics(
                    group.LongCount(),
                    MaxNullable(group.Select(row => row.LoadedAt))));

        var supervisorByAdvisor = supervisorAssignments
            .GroupBy(row => row.AdvisorKey)
            .ToDictionary(group => group.Key, group =>
            {
                var supervisorKeys = group
                    .Select(row => row.SupervisorKey)
                    .Distinct()
                    .ToArray();

                if (supervisorKeys.Length != 1)
                {
                    return new SupervisorAssignment(null, null);
                }

                var supervisorKey = supervisorKeys[0];
                var supervisorName = group
                    .Where(row => row.SupervisorKey == supervisorKey)
                    .Select(row => NormalizeName(row.SupervisorName))
                    .Where(name => name is not null)
                    .OrderByDescending(name => name, StringComparer.Ordinal)
                    .FirstOrDefault();

                return new SupervisorAssignment(supervisorKey, supervisorName);
            });

        var currentSupervisorByAdvisor = currentSupervisors
            .GroupBy(row => row.AdvisorKey)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(row => new SupervisorAssignment(
                        row.SupervisorKey,
                        NormalizeName(row.SupervisorName)))
                    .First());

        return managementRows
            .Select(management =>
            {
                contactByAdvisor.TryGetValue(management.AdvisorKey, out var contact);
                promiseByAdvisor.TryGetValue(management.AdvisorKey, out var promise);
                paymentByAdvisor.TryGetValue(management.AdvisorKey, out var payment);
                supervisorByAdvisor.TryGetValue(management.AdvisorKey, out var periodSupervisor);
                currentSupervisorByAdvisor.TryGetValue(management.AdvisorKey, out var currentSupervisor);

                return new PortfolioAdvisorPerformanceDbRow
                {
                    AdvisorId = management.AdvisorKey,
                    AdvisorName = management.AdvisorName,
                    PeriodSupervisorId = periodSupervisor?.SupervisorKey,
                    PeriodSupervisorName = periodSupervisor?.SupervisorName,
                    CurrentSupervisorId = currentSupervisor?.SupervisorKey,
                    CurrentSupervisorName = currentSupervisor?.SupervisorName,
                    DateFrom = effectiveDateFrom,
                    DateTo = effectiveDateTo,
                    ManagementCount = management.ManagementCount,
                    RpcRate = Divide(
                        contact?.DirectContactClients ?? 0,
                        contact?.ClassifiableClients ?? 0),
                    CloseRate = Divide(
                        promise?.ValidPromiseClients ?? 0,
                        contact?.DirectContactClients ?? 0),
                    PromiseCount = promise?.PromiseCount ?? 0,
                    PaymentCount = payment?.PaymentCount ?? 0,
                    AttributableRecoveredAmount = Round(management.RecoveredAmount, 4),
                    UpdatedAtUtc = MaxNullable(
                    [
                        management.LoadedAt,
                        contact?.LoadedAt,
                        promise?.LoadedAt,
                        payment?.LoadedAt
                    ])
                };
            })
            .OrderBy(row => row.AdvisorName, StringComparer.Ordinal)
            .ThenBy(row => row.AdvisorId)
            .ToArray();
    }

    public static async Task<IReadOnlyList<PortfolioSupervisorPerformanceDbRow>?> GetSupervisorsAsync(
        AnalyticsDbContext context,
        int crmClientId,
        PortfolioSupervisorPerformanceRequest request,
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

        var campaignKeys = await GetCampaignKeysAsync(
            context,
            clientKey.Value,
            request.Campaign,
            cancellationToken);

        if (campaignKeys.Length == 0)
        {
            return [];
        }

        var activity = BuildActivityQuery(
                context,
                clientKey.Value,
                campaignKeys,
                request.SubPortfolioId,
                request.BusinessUnit,
                request.SupervisorId,
                request.DateFrom,
                request.DateTo)
            .Where(row => row.SupervisorKey.HasValue);

        var managementRows = await activity
            .GroupBy(row => row.SupervisorKey!.Value)
            .Select(group => new SupervisorManagementMetrics(
                group.Key,
                group.Max(row => row.SupervisorName) ?? string.Empty,
                group.Min(row => row.CalendarDate),
                group.Max(row => row.CalendarDate),
                group.Sum(row => (long)(row.ManagementEvents ?? 0)),
                group.Sum(row => row.RecoveredAmount ?? 0m),
                group.Max(row => row.LoadedAt)))
            .ToListAsync(cancellationToken);

        if (managementRows.Count == 0)
        {
            return [];
        }

        var effectiveDateFrom = managementRows.Min(row => row.DateFrom);
        var effectiveDateTo = managementRows.Max(row => row.DateTo);

        var advisorCountBySupervisor = await activity
            .Select(row => new
            {
                SupervisorKey = row.SupervisorKey!.Value,
                row.AdvisorKey
            })
            .Distinct()
            .GroupBy(row => row.SupervisorKey)
            .Select(group => new
            {
                SupervisorKey = group.Key,
                AdvisorCount = group.LongCount()
            })
            .ToDictionaryAsync(
                row => row.SupervisorKey,
                row => row.AdvisorCount,
                cancellationToken);

        var contactRows = await BuildContactQuery(
                context,
                clientKey.Value,
                campaignKeys,
                request.SubPortfolioId,
                request.BusinessUnit,
                request.SupervisorId,
                effectiveDateFrom,
                effectiveDateTo)
            .Where(row => row.SupervisorKey.HasValue)
            .GroupBy(row => new
            {
                SupervisorKey = row.SupervisorKey!.Value,
                row.CampaignKey,
                row.PortfolioKey,
                row.SourceDebtorId
            })
            .Select(group => new DebtorContactMetrics(
                group.Key.SupervisorKey,
                group.Any(row => row.HadDirectContact),
                group.Any(row => row.HadIndirectContact),
                group.Any(row => row.HadNoContact),
                group.Max(row => row.LoadedAt)))
            .ToListAsync(cancellationToken);

        var promiseRows = await BuildPromiseQuery(
                context,
                clientKey.Value,
                campaignKeys,
                request.SubPortfolioId,
                request.BusinessUnit,
                request.SupervisorId,
                effectiveDateFrom,
                effectiveDateTo)
            .Where(row => row.SupervisorKey.HasValue && row.IsValidPromise)
            .GroupBy(row => new
            {
                SupervisorKey = row.SupervisorKey!.Value,
                row.CampaignKey,
                row.PortfolioKey,
                row.SourceDebtorId
            })
            .Select(group => new DebtorPromiseMetrics(
                group.Key.SupervisorKey,
                group.LongCount(),
                group.Sum(row => FulfilledStatuses.Contains(row.StatusCode)
                    ? row.PaidAmount ?? 0m
                    : 0m),
                group.Sum(row => FulfillmentStatuses.Contains(row.StatusCode)
                    ? row.PromiseAmount ?? 0m
                    : 0m),
                group.Max(row => row.LoadedAt)))
            .ToListAsync(cancellationToken);

        var paymentRows = await BuildPaymentQuery(
                context,
                clientKey.Value,
                campaignKeys,
                request.SubPortfolioId,
                request.BusinessUnit,
                request.SupervisorId,
                effectiveDateFrom,
                effectiveDateTo)
            .Where(row => row.SupervisorKey.HasValue)
            .GroupBy(row => new
            {
                SupervisorKey = row.SupervisorKey!.Value,
                row.CampaignKey,
                row.PortfolioKey,
                row.SourceDebtorId
            })
            .Select(group => new DebtorPaymentMetrics(
                group.Key.SupervisorKey,
                group.Max(row => row.LoadedAt)))
            .ToListAsync(cancellationToken);

        var contactBySupervisor = contactRows
            .GroupBy(row => row.ActorKey)
            .ToDictionary(
                group => group.Key,
                group => new ContactMetrics(
                    group.LongCount(row => row.HadDirectContact),
                    group.LongCount(row =>
                        row.HadDirectContact
                        || row.HadIndirectContact
                        || row.HadNoContact),
                    MaxNullable(group.Select(row => row.LoadedAt))));

        var promiseBySupervisor = promiseRows
            .GroupBy(row => row.ActorKey)
            .ToDictionary(
                group => group.Key,
                group => new PromiseMetrics(
                    group.Sum(row => row.PromiseCount),
                    group.LongCount(),
                    group.Sum(row => row.FulfillmentPaidAmount),
                    group.Sum(row => row.FulfillmentPromiseAmount),
                    MaxNullable(group.Select(row => row.LoadedAt))));

        var paymentBySupervisor = paymentRows
            .GroupBy(row => row.ActorKey)
            .ToDictionary(
                group => group.Key,
                group => new PaymentMetrics(
                    group.LongCount(),
                    MaxNullable(group.Select(row => row.LoadedAt))));

        return managementRows
            .Select(management =>
            {
                contactBySupervisor.TryGetValue(management.SupervisorKey, out var contact);
                promiseBySupervisor.TryGetValue(management.SupervisorKey, out var promise);
                paymentBySupervisor.TryGetValue(management.SupervisorKey, out var payment);

                return new PortfolioSupervisorPerformanceDbRow
                {
                    SupervisorId = management.SupervisorKey,
                    SupervisorName = management.SupervisorName,
                    DateFrom = effectiveDateFrom,
                    DateTo = effectiveDateTo,
                    AdvisorCount = advisorCountBySupervisor.GetValueOrDefault(
                        management.SupervisorKey),
                    ManagementCount = management.ManagementCount,
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
                    AttributableRecoveredAmount = Round(management.RecoveredAmount, 4),
                    UpdatedAtUtc = MaxNullable(
                    [
                        management.LoadedAt,
                        contact?.LoadedAt,
                        promise?.LoadedAt,
                        payment?.LoadedAt
                    ])
                };
            })
            .OrderBy(row => row.SupervisorName, StringComparer.Ordinal)
            .ThenBy(row => row.SupervisorId)
            .ToArray();
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

        var availableBusinessUnits = await (
            from fact in context.AnalyticsPortfolioDailyFacts.AsNoTracking()
            join portfolio in context.AnalyticsPortfolios.AsNoTracking()
                on fact.PortfolioKey equals portfolio.PortfolioKey
            where fact.ClientKey == clientKey.Value
            select portfolio.SourceBusinessUnit)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (businessUnit is null)
        {
            var distinctScopes = availableBusinessUnits
                .Select(NormalizeBusinessUnit)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(2)
                .Count();

            return distinctScopes > 1 ? null : clientKey;
        }

        return availableBusinessUnits.Any(unit =>
            string.Equals(unit, businessUnit, StringComparison.Ordinal))
            ? clientKey
            : null;
    }

    private static Task<int[]> GetCampaignKeysAsync(
        AnalyticsDbContext context,
        int clientKey,
        string? campaignCode,
        CancellationToken cancellationToken) =>
        context.AnalyticsCampaigns
            .AsNoTracking()
            .Where(campaign =>
                campaign.ClientKey == clientKey
                && (campaignCode == null || campaign.CampaignCode == campaignCode))
            .Select(campaign => campaign.CampaignKey)
            .ToArrayAsync(cancellationToken);

    private static IQueryable<AnalyticsSupervisorAdvisorDailyAttribution> BuildActivityQuery(
        AnalyticsDbContext context,
        int clientKey,
        int[] campaignKeys,
        long? subPortfolioId,
        string? businessUnit,
        int? supervisorId,
        DateOnly? dateFrom,
        DateOnly? dateTo)
    {
        var query = context.AnalyticsSupervisorAdvisorDailyAttributions
            .AsNoTracking()
            .Where(row =>
                row.ClientKey == clientKey
                && campaignKeys.Contains(row.CampaignKey));

        if (subPortfolioId.HasValue)
        {
            query = query.Where(row => row.PortfolioKey == subPortfolioId.Value);
        }

        if (businessUnit is not null)
        {
            query = query.Where(row => context.AnalyticsPortfolios.Any(portfolio =>
                portfolio.PortfolioKey == row.PortfolioKey
                && portfolio.SourceBusinessUnit == businessUnit));
        }

        if (supervisorId.HasValue)
        {
            query = query.Where(row => row.SupervisorKey == supervisorId.Value);
        }

        if (dateFrom.HasValue)
        {
            var from = dateFrom.Value.ToDateTime(TimeOnly.MinValue);
            query = query.Where(row => row.CalendarDate >= from);
        }

        if (dateTo.HasValue)
        {
            var to = dateTo.Value.ToDateTime(TimeOnly.MinValue);
            query = query.Where(row => row.CalendarDate <= to);
        }

        return query;
    }

    private static IQueryable<AnalyticsSupervisorDebtorContactDaily> BuildContactQuery(
        AnalyticsDbContext context,
        int clientKey,
        int[] campaignKeys,
        long? subPortfolioId,
        string? businessUnit,
        int? supervisorId,
        DateTime dateFrom,
        DateTime dateTo)
    {
        var query = context.AnalyticsSupervisorDebtorContactDaily
            .AsNoTracking()
            .Where(row =>
                row.ClientKey == clientKey
                && campaignKeys.Contains(row.CampaignKey)
                && row.CalendarDate >= dateFrom
                && row.CalendarDate <= dateTo);

        if (subPortfolioId.HasValue)
        {
            query = query.Where(row => row.PortfolioKey == subPortfolioId.Value);
        }

        if (businessUnit is not null)
        {
            query = query.Where(row => context.AnalyticsPortfolios.Any(portfolio =>
                portfolio.PortfolioKey == row.PortfolioKey
                && portfolio.SourceBusinessUnit == businessUnit));
        }

        if (supervisorId.HasValue)
        {
            query = query.Where(row => row.SupervisorKey == supervisorId.Value);
        }

        return query;
    }

    private static IQueryable<AnalyticsSupervisorPromiseOperational> BuildPromiseQuery(
        AnalyticsDbContext context,
        int clientKey,
        int[] campaignKeys,
        long? subPortfolioId,
        string? businessUnit,
        int? supervisorId,
        DateTime dateFrom,
        DateTime dateTo)
    {
        var dateToExclusive = dateTo.AddDays(1);
        var query = context.AnalyticsSupervisorPromiseOperational
            .AsNoTracking()
            .Where(row =>
                row.ClientKey == clientKey
                && campaignKeys.Contains(row.CampaignKey)
                && row.ManagementAt >= dateFrom
                && row.ManagementAt < dateToExclusive);

        if (subPortfolioId.HasValue)
        {
            query = query.Where(row => row.PortfolioKey == subPortfolioId.Value);
        }

        if (businessUnit is not null)
        {
            query = query.Where(row => context.AnalyticsPortfolios.Any(portfolio =>
                portfolio.PortfolioKey == row.PortfolioKey
                && portfolio.SourceBusinessUnit == businessUnit));
        }

        if (supervisorId.HasValue)
        {
            query = query.Where(row => row.SupervisorKey == supervisorId.Value);
        }

        return query;
    }

    private static IQueryable<AnalyticsSupervisorDebtorPaymentDaily> BuildPaymentQuery(
        AnalyticsDbContext context,
        int clientKey,
        int[] campaignKeys,
        long? subPortfolioId,
        string? businessUnit,
        int? supervisorId,
        DateTime dateFrom,
        DateTime dateTo)
    {
        var query = context.AnalyticsSupervisorDebtorPaymentDaily
            .AsNoTracking()
            .Where(row =>
                row.ClientKey == clientKey
                && campaignKeys.Contains(row.CampaignKey)
                && row.CalendarDate >= dateFrom
                && row.CalendarDate <= dateTo);

        if (subPortfolioId.HasValue)
        {
            query = query.Where(row => row.PortfolioKey == subPortfolioId.Value);
        }

        if (businessUnit is not null)
        {
            query = query.Where(row => context.AnalyticsPortfolios.Any(portfolio =>
                portfolio.PortfolioKey == row.PortfolioKey
                && portfolio.SourceBusinessUnit == businessUnit));
        }

        if (supervisorId.HasValue)
        {
            query = query.Where(row => row.SupervisorKey == supervisorId.Value);
        }

        return query;
    }

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

    private static DateTime? MaxNullable(IEnumerable<DateTime?> values) =>
        values.Where(value => value.HasValue)
            .Select(value => value!.Value)
            .DefaultIfEmpty()
            .Max() is var max && max != default
                ? max
                : null;

    private static string? NormalizeBusinessUnit(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string? NormalizeName(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private sealed record AdvisorManagementMetrics(
        int AdvisorKey,
        string AdvisorName,
        DateTime DateFrom,
        DateTime DateTo,
        long ManagementCount,
        decimal RecoveredAmount,
        DateTime? LoadedAt);

    private sealed record SupervisorManagementMetrics(
        int SupervisorKey,
        string SupervisorName,
        DateTime DateFrom,
        DateTime DateTo,
        long ManagementCount,
        decimal RecoveredAmount,
        DateTime? LoadedAt);

    private sealed record DebtorContactMetrics(
        int ActorKey,
        bool HadDirectContact,
        bool HadIndirectContact,
        bool HadNoContact,
        DateTime? LoadedAt);

    private sealed record DebtorPromiseMetrics(
        int ActorKey,
        long PromiseCount,
        decimal FulfillmentPaidAmount,
        decimal FulfillmentPromiseAmount,
        DateTime? LoadedAt);

    private sealed record DebtorPaymentMetrics(
        int ActorKey,
        DateTime? LoadedAt);

    private sealed record ContactMetrics(
        long DirectContactClients,
        long ClassifiableClients,
        DateTime? LoadedAt);

    private sealed record PromiseMetrics(
        long PromiseCount,
        long ValidPromiseClients,
        decimal FulfillmentPaidAmount,
        decimal FulfillmentPromiseAmount,
        DateTime? LoadedAt);

    private sealed record PaymentMetrics(
        long PaymentCount,
        DateTime? LoadedAt);

    private sealed record SupervisorAssignment(
        int? SupervisorKey,
        string? SupervisorName);
}
