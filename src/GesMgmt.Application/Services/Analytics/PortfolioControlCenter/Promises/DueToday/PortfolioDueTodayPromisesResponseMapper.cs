using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GesMgmt.Application.Services.Analytics.PortfolioControlCenter;

internal static class PortfolioDueTodayPromisesResponseMapper
{
    private static readonly IReadOnlyDictionary<string, string> StatusLabels =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["pending"] = "Pendiente",
            ["partial"] = "Pago parcial",
            ["covered"] = "Cubierta"
        };

    public static PortfolioDueTodayPromisesResponse Map(
        PortfolioPromisesContext context,
        PortfolioDueTodayPromisesQueryResult result)
    {
        return new PortfolioDueTodayPromisesResponse(
            new PortfolioPromisesCampaign(context.CampaignCode, context.CampaignName),
            result.Summary.AsOfDate.HasValue
                ? DateOnly.FromDateTime(result.Summary.AsOfDate.Value)
                : null,
            ToUtcOffset(result.Summary.UpdatedAtUtc),
            new PortfolioDueTodayPromisesSummary(
                result.Summary.DueTodayCount,
                result.Summary.DueTodayAmount,
                result.Summary.PaidAmount,
                result.Summary.OutstandingAmount),
            result.Status
                .OrderBy(item => GetStatusOrder(item.StatusKey))
                .Select(item => new PortfolioDueTodayPromisesStatusBucket(
                    item.StatusKey,
                    StatusLabels.GetValueOrDefault(item.StatusKey, item.StatusKey),
                    item.PromiseCount,
                    item.PromiseAmount,
                    item.PaidAmount,
                    item.OutstandingAmount))
                .ToArray(),
            result.Pagination,
            result.Items.Select(item => new PortfolioDueTodayPromiseItem(
                item.PromiseId,
                item.DebtorId,
                item.PromiseAmount,
                item.PaidAmount,
                item.OutstandingAmount,
                item.LastPaymentDate.HasValue
                    ? DateOnly.FromDateTime(item.LastPaymentDate.Value)
                    : null,
                item.StatusKey,
                item.AdvisorId,
                item.AdvisorName,
                item.SupervisorId,
                item.SupervisorName)).ToArray());
    }

    private static int GetStatusOrder(string statusKey) =>
        statusKey switch
        {
            "pending" => 1,
            "partial" => 2,
            "covered" => 3,
            _ => 4
        };

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
