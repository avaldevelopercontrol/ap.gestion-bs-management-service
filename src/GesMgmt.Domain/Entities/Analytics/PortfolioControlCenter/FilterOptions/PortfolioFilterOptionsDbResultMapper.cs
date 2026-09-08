namespace GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;

public static class PortfolioFilterOptionsDbResultMapper
{
    public static PortfolioFilterOptionsDbResult Map(
        IReadOnlyList<PortfolioFilterCampaignDbRow> campaigns,
        IReadOnlyList<PortfolioFilterSubPortfolioCampaignDbRow> subPortfolioContexts,
        IReadOnlyList<PortfolioFilterSupervisorContextDbRow> supervisorContexts) =>
        new(
            campaigns,
            BuildSubPortfolios(subPortfolioContexts),
            subPortfolioContexts,
            BuildSupervisors(supervisorContexts),
            supervisorContexts)
        {
            BusinessUnits = BuildBusinessUnits(subPortfolioContexts)
        };

    public static PortfolioFilterOptionsDbResult Scope(
        PortfolioFilterOptionsDbResult source,
        string? businessUnit)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (string.IsNullOrWhiteSpace(businessUnit))
        {
            return source;
        }

        var scopedContexts = source.SubPortfolioCampaigns
            .Where(item => string.Equals(
                item.BusinessUnitCode?.Trim(),
                businessUnit,
                StringComparison.OrdinalIgnoreCase))
            .ToArray();

        var scopedCampaignsBySubPortfolio = scopedContexts
            .GroupBy(item => item.SubPortfolioId)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(item => item.CampaignCode)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase));

        var scopedSupervisorContexts = source.SupervisorContexts
            .Where(item =>
                scopedCampaignsBySubPortfolio.TryGetValue(
                    item.SubPortfolioId,
                    out var campaignCodes)
                && campaignCodes.Contains(item.CampaignCode))
            .ToArray();

        return new PortfolioFilterOptionsDbResult(
            BuildCampaigns(source.Campaigns, scopedContexts),
            BuildSubPortfolios(scopedContexts),
            scopedContexts,
            BuildSupervisors(scopedSupervisorContexts),
            scopedSupervisorContexts)
        {
            BusinessUnits = source.BusinessUnits
        };
    }

    private static IReadOnlyList<PortfolioFilterCampaignDbRow> BuildCampaigns(
        IReadOnlyList<PortfolioFilterCampaignDbRow> campaigns,
        IReadOnlyList<PortfolioFilterSubPortfolioCampaignDbRow> contexts)
    {
        var availabilityByCampaign = contexts
            .GroupBy(item => item.CampaignCode, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => new
                {
                    AvailableDateFrom = group.Min(item => item.AvailableDateFrom),
                    AvailableDateTo = group.Max(item => item.AvailableDateTo),
                    UpdatedAtUtc = group
                        .Where(item => item.UpdatedAtUtc.HasValue)
                        .Select(item => item.UpdatedAtUtc!.Value)
                        .DefaultIfEmpty()
                        .Max()
                },
                StringComparer.OrdinalIgnoreCase);

        return campaigns
            .Where(item => availabilityByCampaign.ContainsKey(item.CampaignCode))
            .Select(item =>
            {
                var availability = availabilityByCampaign[item.CampaignCode];
                return new PortfolioFilterCampaignDbRow
                {
                    CampaignCode = item.CampaignCode,
                    CampaignName = item.CampaignName,
                    StartDate = item.StartDate,
                    EndDate = item.EndDate,
                    AvailableDateFrom = availability.AvailableDateFrom,
                    AvailableDateTo = availability.AvailableDateTo,
                    UpdatedAtUtc = availability.UpdatedAtUtc == default
                        ? null
                        : availability.UpdatedAtUtc
                };
            })
            .ToArray();
    }

    private static IReadOnlyList<string> BuildBusinessUnits(
        IReadOnlyList<PortfolioFilterSubPortfolioCampaignDbRow> contexts) =>
        contexts
            .Select(item => item.BusinessUnitCode)
            .Where(static value => !string.IsNullOrWhiteSpace(value))
            .Select(static value => value!.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(static value => value, StringComparer.OrdinalIgnoreCase)
            .ToArray();

    private static IReadOnlyList<PortfolioFilterSubPortfolioDbRow> BuildSubPortfolios(
        IReadOnlyList<PortfolioFilterSubPortfolioCampaignDbRow> contexts) =>
        contexts
            .GroupBy(item => item.SubPortfolioId)
            .Select(group =>
            {
                var parent = group.First();
                return new
                {
                    Row = new PortfolioFilterSubPortfolioDbRow
                    {
                        SubPortfolioId = group.Key,
                        SubPortfolioName = parent.SubPortfolioName,
                        UpdatedAtUtc = parent.SubPortfolioUpdatedAtUtc
                    },
                    SortOrder = parent.SubPortfolioSortOrder
                };
            })
            .OrderBy(item => item.SortOrder)
            .Select(item => item.Row)
            .ToArray();

    private static IReadOnlyList<PortfolioFilterSupervisorDbRow> BuildSupervisors(
        IReadOnlyList<PortfolioFilterSupervisorContextDbRow> contexts) =>
        contexts
            .GroupBy(item => item.SupervisorId)
            .Select(group =>
            {
                var parent = group.First();
                return new
                {
                    Row = new PortfolioFilterSupervisorDbRow
                    {
                        SupervisorId = group.Key,
                        SupervisorName = parent.SupervisorName,
                        UpdatedAtUtc = parent.SupervisorUpdatedAtUtc
                    },
                    SortOrder = parent.SupervisorSortOrder
                };
            })
            .OrderBy(item => item.SortOrder)
            .Select(item => item.Row)
            .ToArray();
}
