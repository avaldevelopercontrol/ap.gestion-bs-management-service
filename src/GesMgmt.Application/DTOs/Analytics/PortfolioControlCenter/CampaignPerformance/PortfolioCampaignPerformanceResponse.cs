using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

public sealed record PortfolioCampaignPerformanceItem(
    string CampaignCode,
    string CampaignName,
    DateOnly DateFrom,
    DateOnly DateTo,
    DateOnly SnapshotDate,
    long AssignedPortfolio,
    long ManagedPortfolio,
    long PendingPortfolio,
    decimal? ProgressRate,
    long ManagementCount,
    decimal? ContactabilityRate,
    decimal? RpcRate,
    decimal? CloseRate,
    long PromiseCount,
    decimal? PromiseFulfillmentRate,
    long PaymentCount,
    decimal RecoveredAmount,
    decimal? TargetAmount);
public sealed record PortfolioCampaignPerformanceResponse(
    DateTimeOffset? UpdatedAt,
    IReadOnlyList<PortfolioCampaignPerformanceItem> Campaigns);
