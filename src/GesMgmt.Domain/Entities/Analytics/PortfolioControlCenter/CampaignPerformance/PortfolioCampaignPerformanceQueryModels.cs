using System.Globalization;
using System.Text.RegularExpressions;

namespace GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;

public sealed record PortfolioCampaignPerformanceDbRow
{
    public required string CampaignCode { get; init; }
    public required string CampaignName { get; init; }
    public DateTime DateFrom { get; init; }
    public DateTime DateTo { get; init; }
    public DateTime SnapshotDate { get; init; }
    public long AssignedPortfolio { get; init; }
    public long ManagedPortfolio { get; init; }
    public long PendingPortfolio { get; init; }
    public decimal? ProgressRate { get; init; }
    public long ManagementCount { get; init; }
    public decimal? ContactabilityRate { get; init; }
    public decimal? RpcRate { get; init; }
    public decimal? CloseRate { get; init; }
    public long PromiseCount { get; init; }
    public decimal? PromiseFulfillmentRate { get; init; }
    public long PaymentCount { get; init; }
    public decimal RecoveredAmount { get; init; }
    public decimal? TargetAmount { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
