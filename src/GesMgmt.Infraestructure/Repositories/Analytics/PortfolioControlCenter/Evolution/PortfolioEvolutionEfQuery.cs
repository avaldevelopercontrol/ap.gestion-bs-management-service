using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal static class PortfolioEvolutionEfQuery
{
    public static async Task<PortfolioEvolutionContext?> ResolveContextAsync(
        AnalyticsDbContext context,
        int crmClientId,
        string? campaignCode,
        long? subPortfolioId,
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

        if (businessUnit is null
            && await HasAmbiguousBusinessUnitScopeAsync(
                context,
                clientKey.Value,
                cancellationToken))
        {
            return null;
        }

        var candidateCampaigns = context.AnalyticsCampaigns
            .AsNoTracking()
            .Where(campaign => campaign.ClientKey == clientKey.Value);

        if (campaignCode is not null)
        {
            candidateCampaigns = candidateCampaigns.Where(
                campaign => campaign.CampaignCode == campaignCode);
        }
        else if (businessUnit is not null)
        {
            var latestCampaignKey = await context.AnalyticsCampaigns
                .AsNoTracking()
                .Where(campaign =>
                    campaign.ClientKey == clientKey.Value
                    && context.AnalyticsPortfolioDailyFacts.Any(fact =>
                        fact.ClientKey == campaign.ClientKey
                        && fact.CampaignKey == campaign.CampaignKey))
                .OrderByDescending(campaign => campaign.StartDate)
                .ThenByDescending(campaign => campaign.CampaignKey)
                .Select(campaign => (int?)campaign.CampaignKey)
                .FirstOrDefaultAsync(cancellationToken);

            if (!latestCampaignKey.HasValue)
            {
                return null;
            }

            candidateCampaigns = candidateCampaigns.Where(
                campaign => campaign.CampaignKey == latestCampaignKey.Value);
        }

        var campaigns = await candidateCampaigns.ToListAsync(cancellationToken);
        if (campaigns.Count == 0)
        {
            return null;
        }

        var campaignKeys = campaigns
            .Select(campaign => campaign.CampaignKey)
            .ToArray();

        var latestEvolutionDates = await LoadLatestEvolutionDatesAsync(
            context,
            clientKey.Value,
            campaignKeys,
            subPortfolioId,
            businessUnit,
            cancellationToken);

        var latestPortfolioDates = subPortfolioId.HasValue
            ? await LoadLatestPortfolioDatesAsync(
                context,
                clientKey.Value,
                campaignKeys,
                subPortfolioId.Value,
                businessUnit,
                cancellationToken)
            : new Dictionary<int, DateTime?>();

        var selected = campaigns
            .Select(campaign => new SelectedCampaign(
                campaign,
                latestEvolutionDates.GetValueOrDefault(campaign.CampaignKey),
                latestPortfolioDates.GetValueOrDefault(campaign.CampaignKey)))
            .Where(item =>
                subPortfolioId.HasValue
                    ? item.LatestPortfolioDataDate.HasValue
                    : businessUnit is null || item.LatestEvolutionDate.HasValue)
            .OrderBy(item => item.LatestEvolutionDate.HasValue ? 0 : 1)
            .ThenByDescending(item => item.Campaign.StartDate)
            .ThenByDescending(item => item.Campaign.CampaignKey)
            .FirstOrDefault();

        if (selected is null)
        {
            return null;
        }

        return new PortfolioEvolutionContext(
            clientKey.Value,
            selected.Campaign.CampaignKey,
            selected.Campaign.CampaignCode,
            selected.Campaign.CampaignName,
            DateOnly.FromDateTime(selected.Campaign.StartDate),
            DateOnly.FromDateTime(selected.Campaign.EndDate),
            selected.LatestEvolutionDate.HasValue
                ? DateOnly.FromDateTime(selected.LatestEvolutionDate.Value)
                : null);
    }

    public static async Task<IReadOnlyList<PortfolioEvolutionDbRow>> GetAsync(
        AnalyticsDbContext context,
        int clientKey,
        int campaignKey,
        long? subPortfolioId,
        string? businessUnit,
        PortfolioEvolutionRange range,
        CancellationToken cancellationToken)
    {
        var dateFrom = range.DateFrom.ToDateTime(TimeOnly.MinValue);
        var dateToExclusive = range.DateTo
            .AddDays(1)
            .ToDateTime(TimeOnly.MinValue);

        if (subPortfolioId is null && businessUnit is null)
        {
            var rows = await context.AnalyticsCampaignEvolutionDaily
                .AsNoTracking()
                .Where(row =>
                    row.ClientKey == clientKey
                    && row.CampaignKey == campaignKey
                    && row.CalendarDate >= dateFrom
                    && row.CalendarDate < dateToExclusive)
                .OrderBy(row => row.CalendarDate)
                .ToListAsync(cancellationToken);

            return rows
                .Select(row => new PortfolioEvolutionDbRow
                {
                    Period = row.CalendarDate,
                    AssignedPortfolio = row.AssignedClients ?? 0,
                    ManagedPortfolio = row.ManagedClients ?? 0,
                    PendingPortfolio = row.PendingClients ?? 0,
                    RecoveredAmount = RoundAmount(row.RecoveredAmountToDate ?? 0m),
                    LoadedAtUtc = row.LoadedAt
                })
                .ToArray();
        }

        var scopedRows = await ApplyPortfolioScope(
                context,
                context.AnalyticsPortfolioEvolutionDaily
                    .AsNoTracking()
                    .Where(row =>
                        row.ClientKey == clientKey
                        && row.CampaignKey == campaignKey
                        && row.CalendarDate >= dateFrom
                        && row.CalendarDate < dateToExclusive),
                subPortfolioId,
                businessUnit)
            .ToListAsync(cancellationToken);

        if (subPortfolioId.HasValue)
        {
            return scopedRows
                .OrderBy(row => row.CalendarDate)
                .Select(row => new PortfolioEvolutionDbRow
                {
                    Period = row.CalendarDate,
                    AssignedPortfolio = row.AssignedClients ?? 0,
                    ManagedPortfolio = row.ManagedClients ?? 0,
                    PendingPortfolio = row.PendingClients ?? 0,
                    RecoveredAmount = RoundAmount(row.RecoveredAmountToDate ?? 0m),
                    LoadedAtUtc = row.LoadedAt
                })
                .ToArray();
        }

        return scopedRows
            .GroupBy(row => row.CalendarDate)
            .OrderBy(group => group.Key)
            .Select(group => new PortfolioEvolutionDbRow
            {
                Period = group.Key,
                AssignedPortfolio = group.Sum(row => (long)(row.AssignedClients ?? 0)),
                ManagedPortfolio = group.Sum(row => (long)(row.ManagedClients ?? 0)),
                PendingPortfolio = group.Sum(row => (long)(row.PendingClients ?? 0)),
                RecoveredAmount = RoundAmount(
                    group.Sum(row => row.RecoveredAmountToDate ?? 0m)),
                LoadedAtUtc = group.Max(row => row.LoadedAt)
            })
            .ToArray();
    }

    private static IQueryable<AnalyticsPortfolioEvolutionDaily> ApplyPortfolioScope(
        AnalyticsDbContext context,
        IQueryable<AnalyticsPortfolioEvolutionDaily> query,
        long? subPortfolioId,
        string? businessUnit)
    {
        if (subPortfolioId.HasValue)
        {
            query = query.Where(row => row.PortfolioKey == subPortfolioId.Value);
        }

        if (businessUnit is null)
        {
            return query;
        }

        return from row in query
               join portfolio in context.AnalyticsPortfolios.AsNoTracking()
                   on row.PortfolioKey equals portfolio.PortfolioKey
               where portfolio.SourceBusinessUnit == businessUnit
               select row;
    }

    private static async Task<bool> HasAmbiguousBusinessUnitScopeAsync(
        AnalyticsDbContext context,
        int clientKey,
        CancellationToken cancellationToken)
    {
        var businessUnits = await (
            from fact in context.AnalyticsPortfolioDailyFacts.AsNoTracking()
            join portfolio in context.AnalyticsPortfolios.AsNoTracking()
                on fact.PortfolioKey equals portfolio.PortfolioKey
            where fact.ClientKey == clientKey
            select portfolio.SourceBusinessUnit)
            .Distinct()
            .ToListAsync(cancellationToken);

        return businessUnits
            .Select(NormalizeBusinessUnit)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(2)
            .Count() > 1;
    }

    private static async Task<Dictionary<int, DateTime?>> LoadLatestEvolutionDatesAsync(
        AnalyticsDbContext context,
        int clientKey,
        IReadOnlyCollection<int> campaignKeys,
        long? subPortfolioId,
        string? businessUnit,
        CancellationToken cancellationToken)
    {
        var query = context.AnalyticsPortfolioEvolutionDaily
            .AsNoTracking()
            .Where(row =>
                row.ClientKey == clientKey
                && campaignKeys.Contains(row.CampaignKey));

        query = ApplyPortfolioScope(context, query, subPortfolioId, businessUnit);

        return await query
            .GroupBy(row => row.CampaignKey)
            .Select(group => new
            {
                CampaignKey = group.Key,
                LatestDate = (DateTime?)group.Max(row => row.CalendarDate)
            })
            .ToDictionaryAsync(
                item => item.CampaignKey,
                item => item.LatestDate,
                cancellationToken);
    }

    private static async Task<Dictionary<int, DateTime?>> LoadLatestPortfolioDatesAsync(
        AnalyticsDbContext context,
        int clientKey,
        IReadOnlyCollection<int> campaignKeys,
        long subPortfolioId,
        string? businessUnit,
        CancellationToken cancellationToken)
    {
        var query = context.AnalyticsPortfolioDailyMetrics
            .AsNoTracking()
            .Where(row =>
                row.ClientKey == clientKey
                && campaignKeys.Contains(row.CampaignKey)
                && row.PortfolioKey == subPortfolioId);

        if (businessUnit is null)
        {
            return await query
                .GroupBy(row => row.CampaignKey)
                .Select(group => new
                {
                    CampaignKey = group.Key,
                    LatestDate = (DateTime?)group.Max(row => row.CalendarDate)
                })
                .ToDictionaryAsync(
                    item => item.CampaignKey,
                    item => item.LatestDate,
                    cancellationToken);
        }

        return await (
            from row in query
            join portfolio in context.AnalyticsPortfolios.AsNoTracking()
                on row.PortfolioKey equals portfolio.PortfolioKey
            where portfolio.SourceBusinessUnit == businessUnit
            group row by row.CampaignKey
            into grouped
            select new
            {
                CampaignKey = grouped.Key,
                LatestDate = (DateTime?)grouped.Max(row => row.CalendarDate)
            })
            .ToDictionaryAsync(
                item => item.CampaignKey,
                item => item.LatestDate,
                cancellationToken);
    }

    private static decimal RoundAmount(decimal value) =>
        decimal.Round(value, 4, MidpointRounding.AwayFromZero);

    private static string? NormalizeBusinessUnit(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private sealed record SelectedCampaign(
        AnalyticsCampaignDimension Campaign,
        DateTime? LatestEvolutionDate,
        DateTime? LatestPortfolioDataDate);
}
