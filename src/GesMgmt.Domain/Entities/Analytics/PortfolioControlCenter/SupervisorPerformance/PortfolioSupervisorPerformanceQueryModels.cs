using System.Globalization;
using System.Text.RegularExpressions;

namespace GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;

public sealed record PortfolioSupervisorPerformanceDbRow
{
    public int SupervisorId { get; init; }
    public required string SupervisorName { get; init; }
    public DateTime DateFrom { get; init; }
    public DateTime DateTo { get; init; }
    public long AdvisorCount { get; init; }
    public long ManagementCount { get; init; }
    public decimal? RpcRate { get; init; }
    public decimal? CloseRate { get; init; }
    public long PromiseCount { get; init; }
    public decimal? PromiseFulfillmentRate { get; init; }
    public long PaymentCount { get; init; }
    public decimal AttributableRecoveredAmount { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
