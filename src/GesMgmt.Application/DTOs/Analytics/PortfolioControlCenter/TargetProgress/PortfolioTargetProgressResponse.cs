using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

public sealed record PortfolioTargetProgressCampaign(
    string Code,
    string Name);
public sealed record PortfolioTargetProgressPeriod(
    DateOnly DateTo,
    DateOnly? AsOfDate);
public sealed record PortfolioTargetProgressMetrics(
    decimal MonthlyTargetAmount,
    decimal ExpectedToDateAmount,
    decimal? TargetAchievementRate,
    decimal? PaceAchievementRate,
    decimal GapAmount,
    decimal? GapRate);
public sealed record PortfolioTargetProgressResponse(
    PortfolioTargetProgressCampaign Campaign,
    PortfolioTargetProgressPeriod Period,
    DateTimeOffset? UpdatedAt,
    PortfolioTargetProgressMetrics? Target);
