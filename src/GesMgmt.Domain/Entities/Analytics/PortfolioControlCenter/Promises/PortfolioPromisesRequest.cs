using System.Globalization;
using System.Text.RegularExpressions;

namespace GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;

public sealed record PortfolioPromisesRequest(
    string? Campaign,
    long? SubPortfolioId)
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
        out PortfolioPromisesRequest? request,
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

        if (!string.IsNullOrWhiteSpace(dateFrom))
        {
            errors["dateFrom"] =
            ["dateFrom no aplica al estado operativo actual de promesas."];
        }

        if (!string.IsNullOrWhiteSpace(dateTo))
        {
            errors["dateTo"] =
            ["dateTo no aplica al estado operativo actual de promesas."];
        }

        if (errors.Count > 0)
        {
            request = null;
            return false;
        }

        request = new PortfolioPromisesRequest(
            normalizedCampaign,
            parsedSubPortfolioId);
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
}
