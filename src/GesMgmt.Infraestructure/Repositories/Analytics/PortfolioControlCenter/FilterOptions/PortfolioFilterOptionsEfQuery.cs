using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal static class PortfolioFilterOptionsEfQuery
{
    public static async Task<PortfolioFilterOptionsDbResult?> ExecuteAsync(
        AnalyticsDbContext context,
        int crmClientId,
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

        var campaigns = await LoadCampaignsAsync(
            context,
            clientKey.Value,
            cancellationToken);
        var subPortfolioContexts = await LoadSubPortfolioContextsAsync(
            context,
            clientKey.Value,
            cancellationToken);
        var supervisorContexts = await LoadSupervisorContextsAsync(
            context,
            clientKey.Value,
            cancellationToken);

        return PortfolioFilterOptionsDbResultMapper.Map(
            campaigns,
            subPortfolioContexts,
            supervisorContexts);
    }

    private static async Task<IReadOnlyList<PortfolioFilterCampaignDbRow>> LoadCampaignsAsync(
        AnalyticsDbContext context,
        int clientKey,
        CancellationToken cancellationToken)
    {
        var summaryAvailability = await context.AnalyticsCampaignDailySummaries
            .AsNoTracking()
            .Where(row => row.ClientKey == clientKey)
            .GroupBy(row => row.CampaignKey)
            .Select(group => new CampaignAvailability(
                group.Key,
                group.Min(row => row.CalendarDate),
                group.Max(row => row.CalendarDate),
                group.Max(row => row.LoadedAt)))
            .ToListAsync(cancellationToken);

        var evolutionAvailability = await context.AnalyticsCampaignEvolutionDaily
            .AsNoTracking()
            .Where(row => row.ClientKey == clientKey)
            .GroupBy(row => row.CampaignKey)
            .Select(group => new CampaignAvailability(
                group.Key,
                group.Min(row => row.CalendarDate),
                group.Max(row => row.CalendarDate),
                group.Max(row => row.LoadedAt)))
            .ToListAsync(cancellationToken);

        var availabilityByCampaign = summaryAvailability
            .Concat(evolutionAvailability)
            .GroupBy(item => item.CampaignKey)
            .ToDictionary(
                group => group.Key,
                group => new CampaignAvailability(
                    group.Key,
                    group.Min(item => item.AvailableDateFrom),
                    group.Max(item => item.AvailableDateTo),
                    group.Max(item => item.UpdatedAtUtc)));

        if (availabilityByCampaign.Count == 0)
        {
            return [];
        }

        var campaignKeys = availabilityByCampaign.Keys.ToArray();
        var campaignRows = await context.AnalyticsCampaigns
            .AsNoTracking()
            .Where(campaign =>
                campaign.ClientKey == clientKey
                && campaignKeys.Contains(campaign.CampaignKey))
            .ToListAsync(cancellationToken);

        return campaignRows
            .Select(campaign =>
            {
                var availability = availabilityByCampaign[campaign.CampaignKey];
                return new PortfolioFilterCampaignDbRow
                {
                    CampaignCode = campaign.CampaignCode,
                    CampaignName = campaign.CampaignName,
                    StartDate = campaign.StartDate,
                    EndDate = campaign.EndDate,
                    AvailableDateFrom = availability.AvailableDateFrom,
                    AvailableDateTo = availability.AvailableDateTo,
                    UpdatedAtUtc = availability.UpdatedAtUtc
                };
            })
            .OrderByDescending(row => row.StartDate)
            .ThenByDescending(row => row.CampaignCode, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static async Task<IReadOnlyList<PortfolioFilterSubPortfolioCampaignDbRow>> LoadSubPortfolioContextsAsync(
        AnalyticsDbContext context,
        int clientKey,
        CancellationToken cancellationToken)
    {
        var rawRows = await (
            from fact in context.AnalyticsPortfolioDailyFacts.AsNoTracking()
            join portfolio in context.AnalyticsPortfolios.AsNoTracking()
                on fact.PortfolioKey equals portfolio.PortfolioKey
            join campaign in context.AnalyticsCampaigns.AsNoTracking()
                on fact.CampaignKey equals campaign.CampaignKey
            join date in context.AnalyticsDates.AsNoTracking()
                on fact.DateKey equals date.DateKey
            where fact.ClientKey == clientKey
            group new { fact, portfolio, date } by new
            {
                fact.PortfolioKey,
                campaign.CampaignCode
            }
            into grouped
            select new SubPortfolioContextRaw(
                grouped.Key.PortfolioKey,
                grouped.Max(item => item.portfolio.PortfolioName) ?? string.Empty,
                grouped.Max(item => item.portfolio.SourceBusinessUnit),
                grouped.Key.CampaignCode,
                grouped.Min(item => item.date.CalendarDate),
                grouped.Max(item => item.date.CalendarDate),
                grouped.Max(item => item.fact.LoadedAt)))
            .ToListAsync(cancellationToken);

        var parentBySubPortfolio = rawRows
            .GroupBy(row => row.SubPortfolioId)
            .ToDictionary(
                group => group.Key,
                group => new SubPortfolioParent(
                    group
                        .Select(item => item.SubPortfolioName)
                        .OrderByDescending(name => name, StringComparer.OrdinalIgnoreCase)
                        .First(),
                    group.Max(item => item.UpdatedAtUtc)));

        var sortOrderBySubPortfolio = parentBySubPortfolio
            .OrderBy(item => item.Value.SubPortfolioName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(item => item.Key)
            .Select((item, index) => new { item.Key, SortOrder = (long)index + 1 })
            .ToDictionary(item => item.Key, item => item.SortOrder);

        return rawRows
            .Select(row => new PortfolioFilterSubPortfolioCampaignDbRow
            {
                SubPortfolioId = row.SubPortfolioId,
                SubPortfolioName = parentBySubPortfolio[row.SubPortfolioId].SubPortfolioName,
                BusinessUnitCode = NormalizeBusinessUnit(row.BusinessUnitCode),
                CampaignCode = row.CampaignCode,
                AvailableDateFrom = row.AvailableDateFrom,
                AvailableDateTo = row.AvailableDateTo,
                UpdatedAtUtc = row.UpdatedAtUtc,
                SubPortfolioUpdatedAtUtc = parentBySubPortfolio[row.SubPortfolioId].UpdatedAtUtc,
                SubPortfolioSortOrder = sortOrderBySubPortfolio[row.SubPortfolioId]
            })
            .OrderByDescending(row => row.CampaignCode, StringComparer.OrdinalIgnoreCase)
            .ThenBy(row => row.SubPortfolioId)
            .ToArray();
    }

    private static async Task<IReadOnlyList<PortfolioFilterSupervisorContextDbRow>> LoadSupervisorContextsAsync(
        AnalyticsDbContext context,
        int clientKey,
        CancellationToken cancellationToken)
    {
        var rawRows = await (
            from attribution in context.AnalyticsSupervisorAdvisorDailyAttributions.AsNoTracking()
            join campaign in context.AnalyticsCampaigns.AsNoTracking()
                on attribution.CampaignKey equals campaign.CampaignKey
            where attribution.ClientKey == clientKey
                  && attribution.SupervisorKey != null
            group attribution by new
            {
                attribution.SupervisorKey,
                attribution.PortfolioKey,
                campaign.CampaignCode
            }
            into grouped
            select new SupervisorContextRaw(
                grouped.Key.SupervisorKey!.Value,
                grouped.Max(item => item.SupervisorName) ?? string.Empty,
                grouped.Key.PortfolioKey,
                grouped.Key.CampaignCode,
                grouped.Min(item => item.CalendarDate),
                grouped.Max(item => item.CalendarDate),
                grouped.Max(item => item.LoadedAt)))
            .ToListAsync(cancellationToken);

        var parentBySupervisor = rawRows
            .GroupBy(row => row.SupervisorId)
            .ToDictionary(
                group => group.Key,
                group => new SupervisorParent(
                    group
                        .Select(item => item.SupervisorName)
                        .OrderByDescending(name => name, StringComparer.OrdinalIgnoreCase)
                        .First(),
                    group.Max(item => item.UpdatedAtUtc)));

        var sortOrderBySupervisor = parentBySupervisor
            .OrderBy(item => item.Value.SupervisorName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(item => item.Key)
            .Select((item, index) => new { item.Key, SortOrder = (long)index + 1 })
            .ToDictionary(item => item.Key, item => item.SortOrder);

        return rawRows
            .Select(row => new PortfolioFilterSupervisorContextDbRow
            {
                SupervisorId = row.SupervisorId,
                SupervisorName = parentBySupervisor[row.SupervisorId].SupervisorName,
                SubPortfolioId = row.SubPortfolioId,
                CampaignCode = row.CampaignCode,
                AvailableDateFrom = row.AvailableDateFrom,
                AvailableDateTo = row.AvailableDateTo,
                UpdatedAtUtc = row.UpdatedAtUtc,
                SupervisorUpdatedAtUtc = parentBySupervisor[row.SupervisorId].UpdatedAtUtc,
                SupervisorSortOrder = sortOrderBySupervisor[row.SupervisorId]
            })
            .OrderByDescending(row => row.CampaignCode, StringComparer.OrdinalIgnoreCase)
            .ThenBy(row => row.SubPortfolioId)
            .ThenBy(row => row.SupervisorId)
            .ToArray();
    }

    private static string? NormalizeBusinessUnit(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private sealed record CampaignAvailability(
        int CampaignKey,
        DateTime AvailableDateFrom,
        DateTime AvailableDateTo,
        DateTime? UpdatedAtUtc);

    private sealed record SubPortfolioContextRaw(
        long SubPortfolioId,
        string SubPortfolioName,
        string? BusinessUnitCode,
        string CampaignCode,
        DateTime AvailableDateFrom,
        DateTime AvailableDateTo,
        DateTime? UpdatedAtUtc);

    private sealed record SubPortfolioParent(
        string SubPortfolioName,
        DateTime? UpdatedAtUtc);

    private sealed record SupervisorContextRaw(
        int SupervisorId,
        string SupervisorName,
        long SubPortfolioId,
        string CampaignCode,
        DateTime AvailableDateFrom,
        DateTime AvailableDateTo,
        DateTime? UpdatedAtUtc);

    private sealed record SupervisorParent(
        string SupervisorName,
        DateTime? UpdatedAtUtc);
}
