using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GesMgmt.Application.Services.Analytics.PortfolioControlCenter;

internal static class PortfolioOverduePromisesResponseMapper
{
    private static readonly IReadOnlyDictionary<string, string> AgingLabels =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["1-3"] = "1 - 3 días",
            ["4-7"] = "4 - 7 días",
            ["8-plus"] = "8+ días",
            ["unclassified"] = "Sin fecha"
        };

    public static PortfolioOverduePromisesResponse Map(
        PortfolioPromisesContext context,
        PortfolioOverduePromisesQueryResult result)
    {
        return new PortfolioOverduePromisesResponse(
            new PortfolioPromisesCampaign(context.CampaignCode, context.CampaignName),
            result.Summary.AsOfDate.HasValue
                ? DateOnly.FromDateTime(result.Summary.AsOfDate.Value)
                : null,
            ToUtcOffset(result.Summary.UpdatedAtUtc),
            new PortfolioOverduePromisesSummary(
                result.Summary.OverdueCount,
                result.Summary.OverdueAmount,
                result.Summary.OutstandingAmount),
            result.Aging
                .OrderBy(item => GetAgingOrder(item.AgingKey))
                .Select(item => new PortfolioOverduePromisesAgingBucket(
                    item.AgingKey,
                    AgingLabels.GetValueOrDefault(item.AgingKey, item.AgingKey),
                    item.PromiseCount,
                    item.PromiseAmount,
                    item.OutstandingAmount))
                .ToArray(),
            new PortfolioOverduePromiseFilterOptions(
                result.Advisors.Select(item =>
                    new PortfolioOverduePromiseFilterOption(item.AdvisorId, item.AdvisorName)).ToArray(),
                result.Supervisors.Select(item =>
                    new PortfolioOverduePromiseFilterOption(item.SupervisorId, item.SupervisorName)).ToArray()),
            result.Pagination,
            result.Items.Select(item => new PortfolioOverduePromiseItem(
                item.PromiseId,
                item.DebtorId,
                item.DueDate.HasValue ? DateOnly.FromDateTime(item.DueDate.Value) : null,
                item.OverdueDays,
                item.PromiseAmount,
                item.PaidAmount,
                item.OutstandingAmount,
                item.AgingKey,
                item.AdvisorId,
                item.AdvisorName,
                item.SupervisorId,
                item.SupervisorName)).ToArray());
    }

    private static int GetAgingOrder(string agingKey) =>
        agingKey switch
        {
            "1-3" => 1,
            "4-7" => 2,
            "8-plus" => 3,
            "unclassified" => 4,
            _ => 5
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
