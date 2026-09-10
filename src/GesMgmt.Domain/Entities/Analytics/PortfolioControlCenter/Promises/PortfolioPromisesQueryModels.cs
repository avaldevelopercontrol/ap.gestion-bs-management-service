using System.Globalization;
using System.Text.RegularExpressions;

namespace GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;

public sealed record PortfolioPromisesContext(
    int ClientKey,
    int CampaignKey,
    string CampaignCode,
    string CampaignName);
public sealed record PortfolioPromisesDbRow
{
    public long DueTodayCount { get; init; }
    public decimal DueTodayAmount { get; init; }
    public long OverdueCount { get; init; }
    public decimal? FulfillmentRate { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
