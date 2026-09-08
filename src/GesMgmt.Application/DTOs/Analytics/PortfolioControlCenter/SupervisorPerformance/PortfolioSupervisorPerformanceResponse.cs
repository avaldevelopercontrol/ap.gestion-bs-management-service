using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

public sealed record PortfolioSupervisorPerformanceItem(
    int SupervisorId,
    string SupervisorName,
    long AdvisorCount,
    long ManagementCount,
    decimal? RpcRate,
    decimal? CloseRate,
    long PromiseCount,
    decimal? PromiseFulfillmentRate,
    long PaymentCount,
    decimal AttributableRecoveredAmount);
public sealed record PortfolioSupervisorPerformanceResponse(
    DateOnly? DateFrom,
    DateOnly? DateTo,
    DateTimeOffset? UpdatedAt,
    IReadOnlyList<PortfolioSupervisorPerformanceItem> Supervisors);
