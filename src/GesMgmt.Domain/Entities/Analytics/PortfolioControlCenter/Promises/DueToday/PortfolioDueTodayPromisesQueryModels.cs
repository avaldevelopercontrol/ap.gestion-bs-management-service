using System.Globalization;
using System.Text.RegularExpressions;

namespace GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;

public sealed record PortfolioDueTodayPromiseDbRow
{
    public long PromiseId { get; init; }
    public long DebtorId { get; init; }
    public DateTime? DueDate { get; init; }
    public decimal PromiseAmount { get; init; }
    public decimal PaidAmount { get; init; }
    public decimal OutstandingAmount { get; init; }
    public DateTime? LastPaymentDate { get; init; }
    public required string StatusKey { get; init; }
    public int? AdvisorId { get; init; }
    public string? AdvisorName { get; init; }
    public int? SupervisorId { get; init; }
    public string? SupervisorName { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
public sealed record PortfolioDueTodayPromisesMetadataDbRow
{
    public string? StatusKey { get; init; }
    public long PromiseCount { get; init; }
    public decimal PromiseAmount { get; init; }
    public decimal PaidAmount { get; init; }
    public decimal OutstandingAmount { get; init; }
    public DateTime? AsOfDate { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
public sealed record PortfolioDueTodayPromisesSummaryDbRow
{
    public long DueTodayCount { get; init; }
    public decimal DueTodayAmount { get; init; }
    public decimal PaidAmount { get; init; }
    public decimal OutstandingAmount { get; init; }
    public DateTime? AsOfDate { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
public sealed record PortfolioDueTodayPromisesStatusDbRow
{
    public required string StatusKey { get; init; }
    public long PromiseCount { get; init; }
    public decimal PromiseAmount { get; init; }
    public decimal PaidAmount { get; init; }
    public decimal OutstandingAmount { get; init; }
}
public sealed record PortfolioDueTodayPromisesQueryResult(
    PortfolioDueTodayPromisesSummaryDbRow Summary,
    IReadOnlyList<PortfolioDueTodayPromisesStatusDbRow> Status,
    IReadOnlyList<PortfolioDueTodayPromiseDbRow> Items,
    PortfolioPromisesPagination Pagination);
