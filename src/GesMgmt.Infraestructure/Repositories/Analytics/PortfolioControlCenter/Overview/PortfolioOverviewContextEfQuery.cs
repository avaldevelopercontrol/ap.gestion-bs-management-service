using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal static class PortfolioOverviewContextEfQuery
{
    public static async Task<PortfolioOverviewContext?> ExecuteAsync(
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

        var latestDataByCampaign = await LoadLatestSummaryDatesAsync(
            context,
            clientKey.Value,
            campaignKeys,
            subPortfolioId,
            businessUnit,
            cancellationToken);

        var selected = campaigns
            .Select(campaign => new SelectedCampaign(
                campaign,
                latestDataByCampaign.GetValueOrDefault(campaign.CampaignKey)))
            .Where(item =>
                (subPortfolioId is null && businessUnit is null)
                || item.LatestDataDate.HasValue)
            .OrderBy(item => item.LatestDataDate.HasValue ? 0 : 1)
            .ThenByDescending(item => item.Campaign.StartDate)
            .ThenByDescending(item => item.Campaign.CampaignKey)
            .FirstOrDefault();

        if (selected is null)
        {
            return null;
        }

        var operationalSubPortfolioAvailable = subPortfolioId is null
            || await HasOperationalSubPortfolioDataAsync(
                context,
                clientKey.Value,
                selected.Campaign.CampaignKey,
                subPortfolioId.Value,
                businessUnit,
                cancellationToken);

        return new PortfolioOverviewContext(
            new PortfolioSummaryContext(
                clientKey.Value,
                selected.Campaign.CampaignKey,
                selected.Campaign.CampaignCode,
                selected.Campaign.CampaignName,
                DateOnly.FromDateTime(selected.Campaign.StartDate),
                DateOnly.FromDateTime(selected.Campaign.EndDate),
                selected.LatestDataDate.HasValue
                    ? DateOnly.FromDateTime(selected.LatestDataDate.Value)
                    : null),
            operationalSubPortfolioAvailable);
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

    private static async Task<Dictionary<int, DateTime?>> LoadLatestSummaryDatesAsync(
        AnalyticsDbContext context,
        int clientKey,
        IReadOnlyCollection<int> campaignKeys,
        long? subPortfolioId,
        string? businessUnit,
        CancellationToken cancellationToken)
    {
        if (businessUnit is null)
        {
            return await context.AnalyticsPortfolioSummaryStateDaily
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
                    LatestDataDate = (DateTime?)group.Max(row => row.CalendarDate)
                })
                .ToDictionaryAsync(
                    item => item.CampaignKey,
                    item => item.LatestDataDate,
                    cancellationToken);
        }

        return await (
            from row in context.AnalyticsPortfolioSummaryStateDaily.AsNoTracking()
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
                LatestDataDate = (DateTime?)grouped.Max(row => row.CalendarDate)
            })
            .ToDictionaryAsync(
                item => item.CampaignKey,
                item => item.LatestDataDate,
                cancellationToken);
    }

    private static async Task<bool> HasOperationalSubPortfolioDataAsync(
        AnalyticsDbContext context,
        int clientKey,
        int campaignKey,
        long subPortfolioId,
        string? businessUnit,
        CancellationToken cancellationToken)
    {
        if (businessUnit is null)
        {
            return await context.AnalyticsPortfolioDailyMetrics
                .AsNoTracking()
                .AnyAsync(
                    row =>
                        row.ClientKey == clientKey
                        && row.CampaignKey == campaignKey
                        && row.PortfolioKey == subPortfolioId,
                    cancellationToken);
        }

        return await (
            from row in context.AnalyticsPortfolioDailyMetrics.AsNoTracking()
            join portfolio in context.AnalyticsPortfolios.AsNoTracking()
                on row.PortfolioKey equals portfolio.PortfolioKey
            where row.ClientKey == clientKey
                  && row.CampaignKey == campaignKey
                  && row.PortfolioKey == subPortfolioId
                  && portfolio.SourceBusinessUnit == businessUnit
            select row.PortfolioKey)
            .AnyAsync(cancellationToken);
    }

    private static string? NormalizeBusinessUnit(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private sealed record SelectedCampaign(
        AnalyticsCampaignDimension Campaign,
        DateTime? LatestDataDate);
}
