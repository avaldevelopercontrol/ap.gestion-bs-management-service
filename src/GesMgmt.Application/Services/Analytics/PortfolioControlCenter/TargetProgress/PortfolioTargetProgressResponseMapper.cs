using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GesMgmt.Application.Services.Analytics.PortfolioControlCenter;

internal static class PortfolioTargetProgressResponseMapper
{
    public static PortfolioTargetProgressResponse Map(
        PortfolioTargetProgressContext context,
        DateOnly dateTo,
        PortfolioTargetProgressDbRow? row)
    {
        var hasTarget = row?.MonthlyTargetAmount is not null;

        return new PortfolioTargetProgressResponse(
            new PortfolioTargetProgressCampaign(
                context.CampaignCode,
                context.CampaignName),
            new PortfolioTargetProgressPeriod(
                dateTo,
                row is null
                    ? null
                    : DateOnly.FromDateTime(row.AsOfDate)),
            ToUtcOffset(row?.UpdatedAtUtc),
            hasTarget
                ? new PortfolioTargetProgressMetrics(
                    row!.MonthlyTargetAmount!.Value,
                    row.ExpectedToDateAmount ?? 0m,
                    ToPercentage(row.TargetAchievementRate),
                    ToPercentage(row.PaceAchievementRate),
                    row.GapAmount ?? 0m,
                    ToPercentage(row.GapRate))
                : null);
    }

    private static decimal? ToPercentage(decimal? value)
    {
        return value * 100m;
    }

    private static DateTimeOffset? ToUtcOffset(DateTime? value)
    {
        if (!value.HasValue)
        {
            return null;
        }

        var utc = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
        return new DateTimeOffset(utc);
    }
}
