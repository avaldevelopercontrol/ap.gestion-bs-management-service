using System.Globalization;
using System.Text.RegularExpressions;

namespace GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;

public sealed record PortfolioOverduePromisesMetadataDbRow
{
    public required string RowType { get; init; }
    public string? AgingKey { get; init; }
    public int? AdvisorId { get; init; }
    public string? AdvisorName { get; init; }
    public int? SupervisorId { get; init; }
    public string? SupervisorName { get; init; }
    public long PromiseCount { get; init; }
    public decimal PromiseAmount { get; init; }
    public decimal OutstandingAmount { get; init; }
    public DateTime? AsOfDate { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
public sealed record PortfolioOverduePromisesSummaryDbRow
{
    public long OverdueCount { get; init; }
    public decimal OverdueAmount { get; init; }
    public decimal OutstandingAmount { get; init; }
    public DateTime? AsOfDate { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
public sealed record PortfolioOverduePromisesAgingDbRow
{
    public required string AgingKey { get; init; }
    public long PromiseCount { get; init; }
    public decimal PromiseAmount { get; init; }
    public decimal OutstandingAmount { get; init; }
}
public sealed record PortfolioOverduePromiseDbRow
{
    public long PromiseId { get; init; }
    public long DebtorId { get; init; }
    public DateTime? DueDate { get; init; }
    public int? OverdueDays { get; init; }
    public decimal PromiseAmount { get; init; }
    public decimal PaidAmount { get; init; }
    public decimal OutstandingAmount { get; init; }
    public int? AdvisorId { get; init; }
    public string? AdvisorName { get; init; }
    public int? SupervisorId { get; init; }
    public string? SupervisorName { get; init; }
    public required string AgingKey { get; init; }
    public DateTime? AsOfDate { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
public sealed record PortfolioOverdueAdvisorFilterDbRow
{
    public int AdvisorId { get; init; }
    public required string AdvisorName { get; init; }
}
public sealed record PortfolioOverdueSupervisorFilterDbRow
{
    public int SupervisorId { get; init; }
    public required string SupervisorName { get; init; }
}
public sealed record PortfolioOverduePromisesQueryResult(
    PortfolioOverduePromisesSummaryDbRow Summary,
    IReadOnlyList<PortfolioOverduePromisesAgingDbRow> Aging,
    IReadOnlyList<PortfolioOverduePromiseDbRow> Items,
    IReadOnlyList<PortfolioOverdueAdvisorFilterDbRow> Advisors,
    IReadOnlyList<PortfolioOverdueSupervisorFilterDbRow> Supervisors,
    PortfolioPromisesPagination Pagination);
