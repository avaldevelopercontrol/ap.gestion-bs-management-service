using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GesMgmt.Application.Services.Analytics.PortfolioControlCenter;

internal static class PortfolioEvolutionResponseMapper
{
    public static PortfolioEvolutionResponse Map(
        PortfolioEvolutionContext context,
        PortfolioEvolutionRange range,
        IReadOnlyList<PortfolioEvolutionDbRow> rows)
    {
        var points = rows
            .Select(row => new PortfolioEvolutionPoint(
                DateOnly.FromDateTime(row.Period),
                row.AssignedPortfolio,
                row.ManagedPortfolio,
                row.PendingPortfolio,
                row.RecoveredAmount))
            .ToArray();

        var latestLoadedAt = rows
            .Where(row => row.LoadedAtUtc.HasValue)
            .Select(row => row.LoadedAtUtc!.Value)
            .DefaultIfEmpty()
            .Max();

        return new PortfolioEvolutionResponse(
            new PortfolioEvolutionCampaign(
                context.CampaignCode,
                context.CampaignName),
            new PortfolioEvolutionPeriod(
                range.DateFrom,
                range.DateTo),
            latestLoadedAt == default
                ? null
                : ToUtcOffset(latestLoadedAt),
            points);
    }

    private static DateTimeOffset ToUtcOffset(DateTime value)
    {
        var utc = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        return new DateTimeOffset(utc);
    }
}
