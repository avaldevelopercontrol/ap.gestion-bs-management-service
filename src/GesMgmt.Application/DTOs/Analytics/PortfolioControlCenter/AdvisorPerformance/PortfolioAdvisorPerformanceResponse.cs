using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

public sealed record PortfolioAdvisorPerformanceItem(
    int AdvisorId,
    string AdvisorName,
    int? CurrentSupervisorId,
    string? CurrentSupervisorName,
    long ManagementCount,
    decimal? RpcRate,
    decimal? CloseRate,
    long PromiseCount,
    long PaymentCount,
    decimal AttributableRecoveredAmount);
public sealed record PortfolioAdvisorPerformanceResponse(
    DateOnly? DateFrom,
    DateOnly? DateTo,
    DateTimeOffset? UpdatedAt,
    IReadOnlyList<PortfolioAdvisorPerformanceItem> Advisors);
