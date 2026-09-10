using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
namespace GesMgmt.Application.Services.Analytics.PortfolioControlCenter;

internal static class PortfolioFilterOptionsResponseMapper
{
    public static PortfolioFilterOptionsResponse Map(
        PortfolioFilterOptionsDbResult source,
        int crmClientId,
        string? selectedBusinessUnit = null)
    {
        var scopedSource = PortfolioFilterOptionsDbResultMapper.Scope(
            source,
            selectedBusinessUnit);
        source = scopedSource;
        DateOnly? availableDateFrom = source.Campaigns.Count == 0
            ? null
            : DateOnly.FromDateTime(
                source.Campaigns.Min(item => item.AvailableDateFrom));

        DateOnly? availableDateTo = source.Campaigns.Count == 0
            ? null
            : DateOnly.FromDateTime(
                source.Campaigns.Max(item => item.AvailableDateTo));

        var updatedAtUtc = EnumerateUpdatedAt(source)
            .Where(value => value.HasValue)
            .Select(value => value!.Value)
            .DefaultIfEmpty()
            .Max();

        return new PortfolioFilterOptionsResponse(
            availableDateFrom,
            availableDateTo,
            updatedAtUtc == default
                ? null
                : ToUtcOffset(updatedAtUtc),
            new PortfolioFilterPortfolioScope(crmClientId),
            source.BusinessUnits
                .Select(code => new PortfolioFilterBusinessUnitOption(code, code))
                .ToArray(),
            selectedBusinessUnit,
            source.Campaigns
                .Select(item => new PortfolioFilterCampaignOption(
                    item.CampaignCode,
                    item.CampaignName,
                    DateOnly.FromDateTime(item.StartDate),
                    DateOnly.FromDateTime(item.EndDate),
                    DateOnly.FromDateTime(item.AvailableDateFrom),
                    DateOnly.FromDateTime(item.AvailableDateTo)))
                .ToArray(),
            source.SubPortfolios
                .Select(item => new PortfolioFilterSubPortfolioOption(
                    item.SubPortfolioId,
                    item.SubPortfolioName))
                .ToArray(),
            source.Supervisors
                .Select(item => new PortfolioFilterSupervisorOption(
                    item.SupervisorId,
                    item.SupervisorName))
                .ToArray(),
            new PortfolioFilterAvailability(
                source.SubPortfolioCampaigns
                    .Select(item => new SubPortfolioCampaignAvailability(
                        item.SubPortfolioId,
                        item.CampaignCode,
                        DateOnly.FromDateTime(item.AvailableDateFrom),
                        DateOnly.FromDateTime(item.AvailableDateTo)))
                    .ToArray(),
                source.SupervisorContexts
                    .Select(item => new PortfolioSupervisorContextAvailability(
                        item.SupervisorId,
                        item.SubPortfolioId,
                        item.CampaignCode,
                        DateOnly.FromDateTime(item.AvailableDateFrom),
                        DateOnly.FromDateTime(item.AvailableDateTo)))
                    .ToArray()));
    }

    private static IEnumerable<DateTime?> EnumerateUpdatedAt(
        PortfolioFilterOptionsDbResult source)
    {
        return source.Campaigns.Select(item => item.UpdatedAtUtc)
            .Concat(source.SubPortfolios.Select(item => item.UpdatedAtUtc))
            .Concat(source.SubPortfolioCampaigns.Select(item => item.UpdatedAtUtc))
            .Concat(source.Supervisors.Select(item => item.UpdatedAtUtc))
            .Concat(source.SupervisorContexts.Select(item => item.UpdatedAtUtc));
    }

    private static DateTimeOffset ToUtcOffset(DateTime value)
    {
        var utc = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        return new DateTimeOffset(utc);
    }
}
