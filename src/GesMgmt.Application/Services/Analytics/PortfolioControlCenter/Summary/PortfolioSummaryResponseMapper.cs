using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GesMgmt.Application.Services.Analytics.PortfolioControlCenter;

internal static class PortfolioSummaryResponseMapper
{
    public static PortfolioSummaryResponse Map(
        PortfolioSummaryContext context,
        PortfolioSummaryRange range,
        PortfolioSummaryDbRow row)
    {
        if (!row.SnapshotDate.HasValue)
        {
            throw new InvalidOperationException(
                "No se puede construir el resumen sin snapshot real.");
        }

        return new PortfolioSummaryResponse(
            new PortfolioSummaryCampaign(
                context.CampaignCode,
                context.CampaignName),
            new PortfolioSummaryPeriod(
                range.DateFrom,
                range.DateTo,
                DateOnly.FromDateTime(row.SnapshotDate.Value)),
            ToUtcOffset(row.UpdatedAtUtc),
            new PortfolioSummaryFreshness(
                ToPeruOffset(row.OperationAsOfLocal),
                ToUtcOffset(row.PortfolioBaseRefreshedAtUtc),
                ToUtcOffset(row.RefreshedAtUtc)),
            new PortfolioSummaryMetrics(
                row.AssignedPortfolio,
                row.ManagedPortfolio,
                row.PendingPortfolio,
                row.ManagementCount,
                row.ManagementIntensity,
                row.RecoveredAmount,
                ToPercentage(row.ContactabilityRate),
                ToPercentage(row.RpcRate),
                ToPercentage(row.CloseRate),
                row.PromiseCount,
                ToPercentage(row.PromiseFulfillmentRate),
                row.PaymentCount));
    }

    private static decimal? ToPercentage(decimal? value)
    {
        return value * 100m;
    }

    private static DateTimeOffset? ToUtcOffset(DateTime? value)
    {
        if (!value.HasValue)
        {
            return null;
        }

        var utc = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
        return new DateTimeOffset(utc);
    }

    private static DateTimeOffset? ToPeruOffset(DateTime? value)
    {
        if (!value.HasValue)
        {
            return null;
        }

        var local = DateTime.SpecifyKind(
            value.Value,
            DateTimeKind.Unspecified);

        return new DateTimeOffset(
            local,
            TimeSpan.FromHours(-5));
    }
}
