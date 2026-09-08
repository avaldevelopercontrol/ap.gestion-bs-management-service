using System.Globalization;
using System.Text.RegularExpressions;

namespace GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;

public sealed record PortfolioSummaryContext(
    int ClientKey,
    int CampaignKey,
    string CampaignCode,
    string CampaignName,
    DateOnly StartDate,
    DateOnly EndDate,
    DateOnly? LatestDataDate);
public readonly record struct PortfolioSummaryRange(
    DateOnly DateFrom,
    DateOnly DateTo);
public sealed record PortfolioSummaryDbRow
{
    public DateTime? SnapshotDate { get; init; }
    public long AssignedPortfolio { get; init; }
    public long ManagedPortfolio { get; init; }
    public long PendingPortfolio { get; init; }
    public long ManagementCount { get; init; }
    public decimal? ManagementIntensity { get; init; }
    public decimal RecoveredAmount { get; init; }
    public decimal? ContactabilityRate { get; init; }
    public decimal? RpcRate { get; init; }
    public decimal? CloseRate { get; init; }
    public long PromiseCount { get; init; }
    public decimal? PromiseFulfillmentRate { get; init; }
    public long PaymentCount { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
    public DateTime? OperationAsOfLocal { get; init; }
    public DateTime? PortfolioBaseRefreshedAtUtc { get; init; }
    public DateTime? RefreshedAtUtc { get; init; }
}
