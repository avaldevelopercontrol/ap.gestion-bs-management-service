using System.Globalization;
using System.Text.RegularExpressions;

namespace GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;

public sealed record PortfolioOverduePromisesRequest(
    string? Campaign,
    long? SubPortfolioId,
    int Page,
    int PageSize,
    string? Aging,
    string SortBy,
    string SortDirection)
{
    public string? BusinessUnit { get; init; }

    private static readonly Regex CampaignPattern = new(
        @"^\d{4}-(0[1-9]|1[0-2])$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly HashSet<string> AllowedAging =
        new(StringComparer.OrdinalIgnoreCase) { "1-3", "4-7", "8-plus", "unclassified" };

    private static readonly HashSet<string> AllowedSortKeys =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "debtorId", "dueDate", "overdueDays", "promiseAmount",
            "paidAmount", "outstandingAmount", "advisorName", "supervisorName"
        };

    public static bool TryCreate(
        string? campaign,
        string? subPortfolioId,
        out PortfolioOverduePromisesRequest? request,
        out Dictionary<string, string[]> errors) =>
        TryCreate(campaign, subPortfolioId, null, null, null, null, null, out request, out errors);

    public static bool TryCreate(
        string? campaign,
        string? subPortfolioId,
        string? page,
        string? pageSize,
        out PortfolioOverduePromisesRequest? request,
        out Dictionary<string, string[]> errors) =>
        TryCreate(campaign, subPortfolioId, page, pageSize, null, null, null, out request, out errors);

    public static bool TryCreate(
        string? campaign,
        string? subPortfolioId,
        string? page,
        string? pageSize,
        string? aging,
        string? sortBy,
        string? sortDirection,
        out PortfolioOverduePromisesRequest? request,
        out Dictionary<string, string[]> errors)
    {
        errors = [];

        var normalizedCampaign = Normalize(campaign);
        if (normalizedCampaign is not null && !CampaignPattern.IsMatch(normalizedCampaign))
        {
            errors["campaign"] = ["campaign debe tener formato YYYY-MM."];
        }

        var parsedSubPortfolioId = ParseSubPortfolioId(subPortfolioId, errors);
        PortfolioPromisesPaging.Parse(page, pageSize, errors, out var parsedPage, out var parsedPageSize);

        var normalizedAging = Normalize(aging);
        if (normalizedAging is not null && !AllowedAging.Contains(normalizedAging))
        {
            errors["aging"] = ["aging no es válido."];
        }

        var normalizedSortBy = Normalize(sortBy) ?? "overdueDays";
        if (!AllowedSortKeys.Contains(normalizedSortBy))
        {
            errors["sortBy"] = ["sortBy no es válido."];
        }

        var normalizedSortDirection = (Normalize(sortDirection) ?? "desc").ToLowerInvariant();
        if (normalizedSortDirection is not ("asc" or "desc"))
        {
            errors["sortDirection"] = ["sortDirection debe ser asc o desc."];
        }

        if (errors.Count > 0)
        {
            request = null;
            return false;
        }

        request = new PortfolioOverduePromisesRequest(
            normalizedCampaign,
            parsedSubPortfolioId,
            parsedPage,
            parsedPageSize,
            normalizedAging?.ToLowerInvariant(),
            normalizedSortBy,
            normalizedSortDirection);
        return true;
    }

    private static long? ParseSubPortfolioId(
        string? value,
        IDictionary<string, string[]> errors)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        if (long.TryParse(value.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out var parsed)
            && parsed > 0)
        {
            return parsed;
        }

        errors["subPortfolioId"] = ["subPortfolioId debe ser un entero positivo."];
        return null;
    }

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
