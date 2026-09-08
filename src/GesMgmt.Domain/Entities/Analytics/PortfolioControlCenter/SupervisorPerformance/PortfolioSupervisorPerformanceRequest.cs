using System.Globalization;
using System.Text.RegularExpressions;

namespace GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;

public sealed record PortfolioSupervisorPerformanceRequest(
    string? Campaign,
    long? SubPortfolioId,
    int? SupervisorId,
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
        string? supervisorId,
        string? dateFrom,
        string? dateTo,
        out PortfolioSupervisorPerformanceRequest? request,
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

        var parsedSubPortfolioId = ParsePositiveLong(
            "subPortfolioId",
            subPortfolioId,
            errors);

        var parsedSupervisorId = ParsePositiveInt(
            "supervisorId",
            supervisorId,
            errors);

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

        request = new PortfolioSupervisorPerformanceRequest(
            normalizedCampaign,
            parsedSubPortfolioId,
            parsedSupervisorId,
            parsedDateFrom,
            parsedDateTo);

        return true;
    }

    private static long? ParsePositiveLong(
        string fieldName,
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

        errors[fieldName] =
        [$"{fieldName} debe ser un entero positivo."];

        return null;
    }

    private static int? ParsePositiveInt(
        string fieldName,
        string? value,
        IDictionary<string, string[]> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (int.TryParse(
                value.Trim(),
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out var parsed)
            && parsed > 0)
        {
            return parsed;
        }

        errors[fieldName] =
        [$"{fieldName} debe ser un entero positivo."];

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
