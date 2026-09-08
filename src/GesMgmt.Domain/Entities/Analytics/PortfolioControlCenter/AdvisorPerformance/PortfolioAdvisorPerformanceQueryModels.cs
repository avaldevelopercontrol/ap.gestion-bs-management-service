using System.Globalization;
using System.Text.RegularExpressions;

namespace GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;

public sealed record PortfolioAdvisorPerformanceDbRow
{
    public int AdvisorId { get; init; }
    public required string AdvisorName { get; init; }
    public int? CurrentSupervisorId { get; init; }
    public string? CurrentSupervisorName { get; init; }
    public DateTime DateFrom { get; init; }
    public DateTime DateTo { get; init; }
    public long ManagementCount { get; init; }
    public decimal? RpcRate { get; init; }
    public decimal? CloseRate { get; init; }
    public long PromiseCount { get; init; }
    public long PaymentCount { get; init; }
    public decimal AttributableRecoveredAmount { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
