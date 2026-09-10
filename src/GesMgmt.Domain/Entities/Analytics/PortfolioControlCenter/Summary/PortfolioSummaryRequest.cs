using System.Globalization;
using System.Text.RegularExpressions;

namespace GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;

public sealed record PortfolioSummaryRequest(
    string? Campaign,
    long? SubPortfolioId,
    DateOnly? DateFrom,
    DateOnly? DateTo)
{
    public string? BusinessUnit { get; init; }

    private static readonly Regex CampaignPattern = new(
        @"^\d{4}-(0[1-9]|1[0-2])$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static bool TryCreate(
        string? campaign,
        string? subPortfolioId,
        string? dateFrom,
        string? dateTo,
        out PortfolioSummaryRequest? request,
        out Dictionary<string, string[]> errors)
    {
        errors = [];

        var normalizedCampaign = string.IsNullOrWhiteSpace(campaign)
            ? null
            : campaign.Trim();

        if (normalizedCampaign is not null
            && !CampaignPattern.IsMatch(normalizedCampaign))
        {
            errors["campaign"] =
            ["campaign debe tener formato YYYY-MM."];
        }

        var parsedSubPortfolioId = ParseSubPortfolioId(subPortfolioId, errors);
        var parsedDateFrom = ParseDate("dateFrom", dateFrom, errors);
        var parsedDateTo = ParseDate("dateTo", dateTo, errors);

        if (parsedDateFrom.HasValue
            && parsedDateTo.HasValue
            && parsedDateFrom.Value > parsedDateTo.Value)
        {
            errors["dateRange"] =
            ["dateFrom no puede ser posterior a dateTo."];
        }

        if (errors.Count > 0)
        {
            request = null;
            return false;
        }

        request = new PortfolioSummaryRequest(
            normalizedCampaign,
            parsedSubPortfolioId,
            parsedDateFrom,
            parsedDateTo);

        return true;
    }

    public bool TryResolveRange(
        PortfolioSummaryContext context,
        out PortfolioSummaryRange range,
        out Dictionary<string, string[]> errors)
    {
        errors = [];

        var dateFrom = DateFrom ?? context.StartDate;
        var dateTo = DateTo ?? context.LatestDataDate ?? context.EndDate;

        if (dateFrom < context.StartDate || dateFrom > context.EndDate)
        {
            errors["dateFrom"] =
            [$"dateFrom debe estar entre {context.StartDate:yyyy-MM-dd} y {context.EndDate:yyyy-MM-dd}."];
        }

        if (dateTo < context.StartDate || dateTo > context.EndDate)
        {
            errors["dateTo"] =
            [$"dateTo debe estar entre {context.StartDate:yyyy-MM-dd} y {context.EndDate:yyyy-MM-dd}."];
        }

        if (dateFrom > dateTo)
        {
            errors["dateRange"] =
            ["El rango efectivo no es válido: dateFrom es posterior a dateTo."];
        }

        if (errors.Count > 0)
        {
            range = default;
            return false;
        }

        range = new PortfolioSummaryRange(dateFrom, dateTo);
        return true;
    }

    private static long? ParseSubPortfolioId(
        string? value,
        IDictionary<string, string[]> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (long.TryParse(
                value.Trim(),
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out var parsed)
            && parsed > 0)
        {
            return parsed;
        }

        errors["subPortfolioId"] =
        ["subPortfolioId debe ser un entero positivo."];

        return null;
    }

    private static DateOnly? ParseDate(
        string fieldName,
        string? value,
        IDictionary<string, string[]> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (DateOnly.TryParseExact(
                value.Trim(),
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsed))
        {
            return parsed;
        }

        errors[fieldName] =
        [$"{fieldName} debe tener formato YYYY-MM-DD."];

        return null;
    }
}
