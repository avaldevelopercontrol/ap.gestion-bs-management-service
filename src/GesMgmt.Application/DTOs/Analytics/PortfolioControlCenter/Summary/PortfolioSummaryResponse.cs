using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

public sealed record PortfolioSummaryCampaign(
    string Code,
    string Name);
public sealed record PortfolioSummaryPeriod(
    DateOnly DateFrom,
    DateOnly DateTo,
    DateOnly SnapshotDate);
public sealed record PortfolioSummaryFreshness(
    DateTimeOffset? OperationAsOfAt,
    DateTimeOffset? PortfolioBaseRefreshedAt,
    DateTimeOffset? RefreshedAt);
public sealed record PortfolioSummaryMetrics(
    long AssignedPortfolio,
    long ManagedPortfolio,
    long PendingPortfolio,
    long ManagementCount,
    decimal? ManagementIntensity,
    decimal RecoveredAmount,
    decimal? ContactabilityRate,
    decimal? RpcRate,
    decimal? CloseRate,
    long PromiseCount,
    decimal? PromiseFulfillmentRate,
    long PaymentCount);
public sealed record PortfolioSummaryResponse(
    PortfolioSummaryCampaign Campaign,
    PortfolioSummaryPeriod Period,
    DateTimeOffset? UpdatedAt,
    PortfolioSummaryFreshness Freshness,
    PortfolioSummaryMetrics Summary);
