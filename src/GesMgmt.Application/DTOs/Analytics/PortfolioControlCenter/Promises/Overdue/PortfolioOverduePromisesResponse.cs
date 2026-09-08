using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

public sealed record PortfolioOverduePromisesSummary(
    long OverdueCount,
    decimal OverdueAmount,
    decimal OutstandingAmount);
public sealed record PortfolioOverduePromisesAgingBucket(
    string Key,
    string Label,
    long Count,
    decimal PromiseAmount,
    decimal OutstandingAmount);
public sealed record PortfolioOverduePromiseItem(
    long PromiseId,
    long DebtorId,
    DateOnly? DueDate,
    int? OverdueDays,
    decimal PromiseAmount,
    decimal PaidAmount,
    decimal OutstandingAmount,
    string AgingKey,
    int? AdvisorId,
    string? AdvisorName,
    int? SupervisorId,
    string? SupervisorName);
public sealed record PortfolioOverduePromiseFilterOption(
    int Id,
    string Name);
public sealed record PortfolioOverduePromiseFilterOptions(
    IReadOnlyList<PortfolioOverduePromiseFilterOption> Advisors,
    IReadOnlyList<PortfolioOverduePromiseFilterOption> Supervisors);
public sealed record PortfolioOverduePromisesResponse(
    PortfolioPromisesCampaign Campaign,
    DateOnly? AsOfDate,
    DateTimeOffset? UpdatedAt,
    PortfolioOverduePromisesSummary Summary,
    IReadOnlyList<PortfolioOverduePromisesAgingBucket> Aging,
    PortfolioOverduePromiseFilterOptions Filters,
    PortfolioPromisesPagination Pagination,
    IReadOnlyList<PortfolioOverduePromiseItem> Items);
