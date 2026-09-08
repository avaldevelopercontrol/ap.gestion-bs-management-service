using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GesMgmt.Application.Services.Analytics.PortfolioControlCenter;

internal static class PortfolioCampaignPerformanceResponseMapper
{
    public static PortfolioCampaignPerformanceResponse Map(
        IReadOnlyList<PortfolioCampaignPerformanceDbRow> rows)
    {
        var items = rows
            .Select(row => new PortfolioCampaignPerformanceItem(
                row.CampaignCode,
                row.CampaignName,
                DateOnly.FromDateTime(row.DateFrom),
                DateOnly.FromDateTime(row.DateTo),
                DateOnly.FromDateTime(row.SnapshotDate),
                row.AssignedPortfolio,
                row.ManagedPortfolio,
                row.PendingPortfolio,
                ToPercentage(row.ProgressRate),
                row.ManagementCount,
                ToPercentage(row.ContactabilityRate),
                ToPercentage(row.RpcRate),
                ToPercentage(row.CloseRate),
                row.PromiseCount,
                ToPercentage(row.PromiseFulfillmentRate),
                row.PaymentCount,
                row.RecoveredAmount,
                row.TargetAmount))
            .ToArray();

        var updatedAtUtc = rows
            .Where(row => row.UpdatedAtUtc.HasValue)
            .Select(row => row.UpdatedAtUtc!.Value)
            .DefaultIfEmpty()
            .Max();

        return new PortfolioCampaignPerformanceResponse(
            updatedAtUtc == default
                ? null
                : ToUtcOffset(updatedAtUtc),
            items);
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
