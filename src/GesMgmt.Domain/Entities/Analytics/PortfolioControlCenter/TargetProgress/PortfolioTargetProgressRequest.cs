using System.Globalization;
using System.Text.RegularExpressions;

namespace GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;

public sealed record PortfolioTargetProgressRequest(
    string? Campaign,
    long? SubPortfolioId,
    DateOnly? DateTo)
{
    public string? BusinessUnit { get; init; }

    private static readonly Regex CampaignPattern = new(
        @"^\d{4}-(0[1-9]|1[0-2])$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static bool TryCreate(
        string? campaign,
        string? subPortfolioId,
        string? dateTo,
        out PortfolioTargetProgressRequest? request,
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
        var parsedDateTo = ParseDate("dateTo", dateTo, errors);

        if (errors.Count > 0)
        {
            request = null;
            return false;
        }

        request = new PortfolioTargetProgressRequest(
            normalizedCampaign,
            parsedSubPortfolioId,
            parsedDateTo);

        return true;
    }

    public bool TryResolveDateTo(
        PortfolioTargetProgressContext context,
        out DateOnly dateTo,
        out Dictionary<string, string[]> errors)
    {
        errors = [];

        dateTo = DateTo
            ?? context.LatestProgressDate
            ?? context.EndDate;

        if (dateTo < context.StartDate || dateTo > context.EndDate)
        {
            errors["dateTo"] =
            [$"dateTo debe estar entre {context.StartDate:yyyy-MM-dd} y {context.EndDate:yyyy-MM-dd}."];
        }

        return errors.Count == 0;
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
