using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

public sealed record PortfolioDueTodayPromisesSummary(
    long DueTodayCount,
    decimal DueTodayAmount,
    decimal PaidAmount,
    decimal OutstandingAmount);
public sealed record PortfolioDueTodayPromisesStatusBucket(
    string Key,
    string Label,
    long Count,
    decimal PromiseAmount,
    decimal PaidAmount,
    decimal OutstandingAmount);
public sealed record PortfolioDueTodayPromiseItem(
    long PromiseId,
    long DebtorId,
    decimal PromiseAmount,
    decimal PaidAmount,
    decimal OutstandingAmount,
    DateOnly? LastPaymentDate,
    string StatusKey,
    int? AdvisorId,
    string? AdvisorName,
    int? SupervisorId,
    string? SupervisorName);
public sealed record PortfolioDueTodayPromisesResponse(
    PortfolioPromisesCampaign Campaign,
    DateOnly? AsOfDate,
    DateTimeOffset? UpdatedAt,
    PortfolioDueTodayPromisesSummary Summary,
    IReadOnlyList<PortfolioDueTodayPromisesStatusBucket> Status,
    PortfolioPromisesPagination Pagination,
    IReadOnlyList<PortfolioDueTodayPromiseItem> Items);
