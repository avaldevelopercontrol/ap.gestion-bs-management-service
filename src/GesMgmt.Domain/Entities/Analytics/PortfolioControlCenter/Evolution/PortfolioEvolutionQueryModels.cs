using System.Globalization;
using System.Text.RegularExpressions;

namespace GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;

public sealed record PortfolioEvolutionContext(
    int ClientKey,
    int CampaignKey,
    string CampaignCode,
    string CampaignName,
    DateOnly StartDate,
    DateOnly EndDate,
    DateOnly? LatestEvolutionDate);
public readonly record struct PortfolioEvolutionRange(
    DateOnly DateFrom,
    DateOnly DateTo);
public sealed record PortfolioEvolutionDbRow
{
    public DateTime Period { get; init; }
    public long AssignedPortfolio { get; init; }
    public long ManagedPortfolio { get; init; }
    public long PendingPortfolio { get; init; }
    public decimal RecoveredAmount { get; init; }
    public DateTime? LoadedAtUtc { get; init; }
}
