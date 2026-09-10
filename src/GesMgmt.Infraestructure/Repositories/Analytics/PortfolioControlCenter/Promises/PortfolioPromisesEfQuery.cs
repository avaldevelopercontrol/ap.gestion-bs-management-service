using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal static class PortfolioPromisesEfQuery
{
    public static async Task<PortfolioPromisesContext?> ResolveContextAsync(
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

        var promiseRows = await ApplyPortfolioScope(
                context,
                context.AnalyticsPromiseOperational
                    .AsNoTracking()
                    .Where(row =>
                        row.ClientKey == clientKey.Value
                        && campaignKeys.Contains(row.CampaignKey)),
                subPortfolioId,
                businessUnit)
            .Select(row => new
            {
                row.CampaignKey,
                row.LoadedAt
            })
            .ToListAsync(cancellationToken);

        var promiseStats = promiseRows
            .GroupBy(row => row.CampaignKey)
            .ToDictionary(
                group => group.Key,
                group => new PromiseStats(
                    group.LongCount(),
                    group.Max(row => row.LoadedAt)));

        var latestPortfolioDates = await LoadLatestPortfolioDatesAsync(
            context,
            clientKey.Value,
            campaignKeys,
            subPortfolioId,
            businessUnit,
            cancellationToken);

        var selected = campaigns
            .Select(campaign => new SelectedCampaign(
                campaign,
                promiseStats.GetValueOrDefault(campaign.CampaignKey),
                latestPortfolioDates.GetValueOrDefault(campaign.CampaignKey)))
            .Where(item =>
                (subPortfolioId is null && businessUnit is null)
                || item.LatestPortfolioDataDate.HasValue)
            .OrderBy(item => item.PromiseStats?.PromiseCount > 0 ? 0 : 1)
            .ThenByDescending(item => item.PromiseStats?.LatestLoadedAt)
            .ThenByDescending(item => item.Campaign.StartDate)
            .ThenByDescending(item => item.Campaign.CampaignKey)
            .FirstOrDefault();

        if (selected is null)
        {
            return null;
        }

        return new PortfolioPromisesContext(
            clientKey.Value,
            selected.Campaign.CampaignKey,
            selected.Campaign.CampaignCode,
            selected.Campaign.CampaignName);
    }

    public static async Task<PortfolioPromisesDbRow> GetOperationalAsync(
        AnalyticsDbContext context,
        int clientKey,
        int campaignKey,
        long? subPortfolioId,
        string? businessUnit,
        CancellationToken cancellationToken)
    {
        var rows = await ApplyPortfolioScope(
                context,
                context.AnalyticsPromiseOperational
                    .AsNoTracking()
                    .Where(row =>
                        row.ClientKey == clientKey
                        && row.CampaignKey == campaignKey
                        && row.IsValidPromise),
                subPortfolioId,
                businessUnit)
            .ToListAsync(cancellationToken);

        var dueToday = rows.Where(row => row.IsDueToday).ToArray();
        var fulfillmentRows = rows
            .Where(row => row.IsFulfilledOrPartial || row.IsBroken)
            .ToArray();

        var fulfillmentPaidAmount = rows
            .Where(row => row.IsFulfilledOrPartial)
            .Sum(row => row.PaidAmount ?? 0m);

        var fulfillmentPromiseAmount = fulfillmentRows
            .Sum(row => row.PromiseAmount ?? 0m);

        return new PortfolioPromisesDbRow
        {
            DueTodayCount = dueToday.LongLength,
            DueTodayAmount = RoundAmount(
                dueToday.Sum(row => row.PromiseAmount ?? 0m)),
            OverdueCount = rows.LongCount(row => row.IsBroken),
            FulfillmentRate = Divide(
                fulfillmentPaidAmount,
                fulfillmentPromiseAmount),
            UpdatedAtUtc = rows.Count == 0
                ? null
                : rows.Max(row => row.LoadedAt)
        };
    }

    private static IQueryable<AnalyticsPromiseOperational> ApplyPortfolioScope(
        AnalyticsDbContext context,
        IQueryable<AnalyticsPromiseOperational> query,
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

    private static async Task<Dictionary<int, DateTime?>> LoadLatestPortfolioDatesAsync(
        AnalyticsDbContext context,
        int clientKey,
        IReadOnlyCollection<int> campaignKeys,
        long? subPortfolioId,
        string? businessUnit,
        CancellationToken cancellationToken)
    {
        var query = context.AnalyticsPortfolioDailyMetrics
            .AsNoTracking()
            .Where(row =>
                row.ClientKey == clientKey
                && campaignKeys.Contains(row.CampaignKey));

        if (subPortfolioId.HasValue)
        {
            query = query.Where(row => row.PortfolioKey == subPortfolioId.Value);
        }

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

    private static decimal? Divide(decimal numerator, decimal denominator)
    {
        if (denominator == 0m)
        {
            return null;
        }

        return decimal.Round(
            numerator / denominator,
            6,
            MidpointRounding.AwayFromZero);
    }

    private static decimal RoundAmount(decimal value) =>
        decimal.Round(value, 4, MidpointRounding.AwayFromZero);

    private static string? NormalizeBusinessUnit(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private sealed record PromiseStats(
        long PromiseCount,
        DateTime? LatestLoadedAt);

    private sealed record SelectedCampaign(
        AnalyticsCampaignDimension Campaign,
        PromiseStats? PromiseStats,
        DateTime? LatestPortfolioDataDate);
}
