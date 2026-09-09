using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal static class PortfolioTargetProgressEfQuery
{
    public static async Task<PortfolioTargetProgressContext?> ResolveContextAsync(
        AnalyticsDbContext context,
        int crmClientId,
        string? campaignCode,
        long? subPortfolioId,
        string? businessUnit,
        bool includeClientLevelTarget,
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

        var latestTargetDates = includeClientLevelTarget
            ? await LoadLatestTargetDatesAsync(
                context,
                clientKey.Value,
                campaignKeys,
                cancellationToken)
            : new Dictionary<int, DateTime?>();

        var latestPortfolioDates = await LoadLatestPortfolioDatesAsync(
            context,
            clientKey.Value,
            campaignKeys,
            subPortfolioId,
            businessUnit,
            cancellationToken);

        var useClientLevelTarget = subPortfolioId is null && includeClientLevelTarget;

        var selected = campaigns
            .Select(campaign => new SelectedCampaign(
                campaign,
                useClientLevelTarget
                    ? latestTargetDates.GetValueOrDefault(campaign.CampaignKey)
                    : latestPortfolioDates.GetValueOrDefault(campaign.CampaignKey)))
            .Where(item => useClientLevelTarget || item.LatestProgressDate.HasValue)
            .OrderBy(item => item.LatestProgressDate.HasValue ? 0 : 1)
            .ThenByDescending(item => item.LatestProgressDate)
            .ThenByDescending(item => item.Campaign.StartDate)
            .ThenByDescending(item => item.Campaign.CampaignKey)
            .FirstOrDefault();

        if (selected is null)
        {
            return null;
        }

        return new PortfolioTargetProgressContext(
            clientKey.Value,
            selected.Campaign.CampaignKey,
            selected.Campaign.CampaignCode,
            selected.Campaign.CampaignName,
            DateOnly.FromDateTime(selected.Campaign.StartDate),
            DateOnly.FromDateTime(selected.Campaign.EndDate),
            selected.LatestProgressDate.HasValue
                ? DateOnly.FromDateTime(selected.LatestProgressDate.Value)
                : null);
    }

    public static async Task<PortfolioTargetProgressDbRow?> GetAsync(
        AnalyticsDbContext context,
        int clientKey,
        int campaignKey,
        string? businessUnit,
        DateOnly dateTo,
        CancellationToken cancellationToken)
    {
        var dateToValue = dateTo.ToDateTime(TimeOnly.MinValue);

        var target = await context.AnalyticsCampaignTargetProgress
            .AsNoTracking()
            .Where(row =>
                row.ClientKey == clientKey
                && row.CampaignKey == campaignKey
                && row.CalendarDate <= dateToValue)
            .OrderByDescending(row => row.CalendarDate)
            .FirstOrDefaultAsync(cancellationToken);

        if (target is null)
        {
            return null;
        }

        var recoveredAmount = await LoadRecoveredAmountAsync(
            context,
            clientKey,
            campaignKey,
            businessUnit,
            target.CalendarDate,
            cancellationToken);

        var monthlyTargetAmount = RoundAmount(target.TargetRecoveredAmount);
        var expectedToDateAmount = RoundAmount(target.ExpectedRecoveredToDate);
        var gapAmount = expectedToDateAmount.HasValue
            ? RoundAmount(recoveredAmount - expectedToDateAmount.Value)
            : null;

        return new PortfolioTargetProgressDbRow
        {
            AsOfDate = target.CalendarDate,
            MonthlyTargetAmount = monthlyTargetAmount,
            ExpectedToDateAmount = expectedToDateAmount,
            TargetAchievementRate = Divide(recoveredAmount, monthlyTargetAmount),
            PaceAchievementRate = Divide(recoveredAmount, expectedToDateAmount),
            GapAmount = gapAmount,
            GapRate = gapAmount.HasValue
                ? Divide(gapAmount.Value, expectedToDateAmount)
                : null,
            UpdatedAtUtc = target.TargetSourceAsOfAt
        };
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

    private static async Task<Dictionary<int, DateTime?>> LoadLatestTargetDatesAsync(
        AnalyticsDbContext context,
        int clientKey,
        IReadOnlyCollection<int> campaignKeys,
        CancellationToken cancellationToken) =>
        await context.AnalyticsCampaignTargetProgress
            .AsNoTracking()
            .Where(row =>
                row.ClientKey == clientKey
                && campaignKeys.Contains(row.CampaignKey))
            .GroupBy(row => row.CampaignKey)
            .Select(group => new
            {
                CampaignKey = group.Key,
                LatestProgressDate = (DateTime?)group.Max(row => row.CalendarDate)
            })
            .ToDictionaryAsync(
                item => item.CampaignKey,
                item => item.LatestProgressDate,
                cancellationToken);

    private static async Task<Dictionary<int, DateTime?>> LoadLatestPortfolioDatesAsync(
        AnalyticsDbContext context,
        int clientKey,
        IReadOnlyCollection<int> campaignKeys,
        long? subPortfolioId,
        string? businessUnit,
        CancellationToken cancellationToken)
    {
        if (businessUnit is null)
        {
            return await context.AnalyticsPortfolioDailyMetrics
                .AsNoTracking()
                .Where(row =>
                    row.ClientKey == clientKey
                    && campaignKeys.Contains(row.CampaignKey)
                    && (!subPortfolioId.HasValue
                        || row.PortfolioKey == subPortfolioId.Value))
                .GroupBy(row => row.CampaignKey)
                .Select(group => new
                {
                    CampaignKey = group.Key,
                    LatestPortfolioDataDate = (DateTime?)group.Max(row => row.CalendarDate)
                })
                .ToDictionaryAsync(
                    item => item.CampaignKey,
                    item => item.LatestPortfolioDataDate,
                    cancellationToken);
        }

        return await (
            from row in context.AnalyticsPortfolioDailyMetrics.AsNoTracking()
            join portfolio in context.AnalyticsPortfolios.AsNoTracking()
                on row.PortfolioKey equals portfolio.PortfolioKey
            where row.ClientKey == clientKey
                  && campaignKeys.Contains(row.CampaignKey)
                  && (!subPortfolioId.HasValue
                      || row.PortfolioKey == subPortfolioId.Value)
                  && portfolio.SourceBusinessUnit == businessUnit
            group row by row.CampaignKey
            into grouped
            select new
            {
                CampaignKey = grouped.Key,
                LatestPortfolioDataDate = (DateTime?)grouped.Max(row => row.CalendarDate)
            })
            .ToDictionaryAsync(
                item => item.CampaignKey,
                item => item.LatestPortfolioDataDate,
                cancellationToken);
    }

    private static async Task<decimal> LoadRecoveredAmountAsync(
        AnalyticsDbContext context,
        int clientKey,
        int campaignKey,
        string? businessUnit,
        DateTime asOfDate,
        CancellationToken cancellationToken)
    {
        decimal? total;

        if (businessUnit is null)
        {
            total = await context.AnalyticsPortfolioDailyMetrics
                .AsNoTracking()
                .Where(row =>
                    row.ClientKey == clientKey
                    && row.CampaignKey == campaignKey
                    && row.CalendarDate <= asOfDate)
                .SumAsync(row => row.RecoveredAmountDay, cancellationToken);
        }
        else
        {
            total = await (
                from row in context.AnalyticsPortfolioDailyMetrics.AsNoTracking()
                join portfolio in context.AnalyticsPortfolios.AsNoTracking()
                    on row.PortfolioKey equals portfolio.PortfolioKey
                where row.ClientKey == clientKey
                      && row.CampaignKey == campaignKey
                      && row.CalendarDate <= asOfDate
                      && portfolio.SourceBusinessUnit == businessUnit
                select row.RecoveredAmountDay)
                .SumAsync(cancellationToken);
        }

        return RoundAmount(total) ?? 0m;
    }

    private static decimal? Divide(decimal numerator, decimal? denominator)
    {
        if (!denominator.HasValue || denominator.Value == 0m)
        {
            return null;
        }

        return decimal.Round(
            numerator / denominator.Value,
            6,
            MidpointRounding.AwayFromZero);
    }

    private static decimal? RoundAmount(decimal? value) =>
        value.HasValue
            ? decimal.Round(value.Value, 4, MidpointRounding.AwayFromZero)
            : null;

    private static string? NormalizeBusinessUnit(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private sealed record SelectedCampaign(
        AnalyticsCampaignDimension Campaign,
        DateTime? LatestProgressDate);
}
