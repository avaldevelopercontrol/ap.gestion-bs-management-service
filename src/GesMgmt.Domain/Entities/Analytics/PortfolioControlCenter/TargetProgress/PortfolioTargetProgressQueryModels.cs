using System.Globalization;
using System.Text.RegularExpressions;

namespace GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;

public sealed record PortfolioTargetProgressContext(
    int ClientKey,
    int CampaignKey,
    string CampaignCode,
    string CampaignName,
    DateOnly StartDate,
    DateOnly EndDate,
    DateOnly? LatestProgressDate);
public sealed record PortfolioTargetProgressDbRow
{
    public DateTime AsOfDate { get; init; }
    public decimal? MonthlyTargetAmount { get; init; }
    public decimal? ExpectedToDateAmount { get; init; }
    public decimal? TargetAchievementRate { get; init; }
    public decimal? PaceAchievementRate { get; init; }
    public decimal? GapAmount { get; init; }
    public decimal? GapRate { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
