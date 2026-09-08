using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GesMgmt.Application.Services.Analytics.PortfolioControlCenter;

internal static class PortfolioAdvisorPerformanceResponseMapper
{
    public static PortfolioAdvisorPerformanceResponse Map(
        IReadOnlyList<PortfolioAdvisorPerformanceDbRow> rows)
    {
        DateOnly? dateFrom = rows.Count == 0
            ? null
            : DateOnly.FromDateTime(rows.Min(row => row.DateFrom));

        DateOnly? dateTo = rows.Count == 0
            ? null
            : DateOnly.FromDateTime(rows.Max(row => row.DateTo));

        var updatedAtUtc = rows
            .Where(row => row.UpdatedAtUtc.HasValue)
            .Select(row => row.UpdatedAtUtc!.Value)
            .DefaultIfEmpty()
            .Max();

        var advisors = rows
            .Select(row => new PortfolioAdvisorPerformanceItem(
                row.AdvisorId,
                row.AdvisorName,
                row.CurrentSupervisorId,
                row.CurrentSupervisorName,
                row.ManagementCount,
                ToPercentage(row.RpcRate),
                ToPercentage(row.CloseRate),
                row.PromiseCount,
                row.PaymentCount,
                row.AttributableRecoveredAmount))
            .ToArray();

        return new PortfolioAdvisorPerformanceResponse(
            dateFrom,
            dateTo,
            updatedAtUtc == default
                ? null
                : ToUtcOffset(updatedAtUtc),
            advisors);
    }

    private static decimal? ToPercentage(decimal? value)
    {
        return value * 100m;
    }

    private static DateTimeOffset ToUtcOffset(DateTime value)
    {
        var utc = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        return new DateTimeOffset(utc);
    }
}
