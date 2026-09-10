using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GesMgmt.Application.Services.Analytics.PortfolioControlCenter;

internal static class PortfolioPromisesResponseMapper
{
    public static PortfolioPromisesResponse Map(
        PortfolioPromisesContext context,
        PortfolioPromisesDbRow row)
    {
        return new PortfolioPromisesResponse(
            new PortfolioPromisesCampaign(
                context.CampaignCode,
                context.CampaignName),
            ToUtcOffset(row.UpdatedAtUtc),
            new PortfolioPromiseStatusMetrics(
                row.DueTodayCount,
                row.DueTodayAmount,
                row.OverdueCount,
                ToPercentage(row.FulfillmentRate)));
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
}
