namespace GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;

/// <summary>
/// Shared Portfolio Control Center contract for selecting a single business-unit scope.
/// The canonical code is the value persisted in analytics.dim_portfolio.source_business_unit;
/// the API does not invent a second business-unit identifier.
/// </summary>
public static class PortfolioBusinessUnitContract
{
    public const int MaxCodeLength = 150;

    public static bool TryNormalizeRequested(
        string? requestedBusinessUnit,
        out string? normalizedBusinessUnit,
        out Dictionary<string, string[]> errors)
    {
        errors = [];
        return TryNormalize(
            "businessUnit",
            requestedBusinessUnit,
            out normalizedBusinessUnit,
            errors);
    }

    public static bool TryResolve(
        string? requestedBusinessUnit,
        string? backwardCompatibleDefaultBusinessUnit,
        IEnumerable<string?> sourceBusinessUnits,
        out PortfolioBusinessUnitSelection selection,
        out Dictionary<string, string[]> errors)
    {
        ArgumentNullException.ThrowIfNull(sourceBusinessUnits);

        errors = [];

        if (!TryNormalize(
                "businessUnit",
                requestedBusinessUnit,
                out var normalizedRequested,
                errors)
            || !TryNormalize(
                "defaultBusinessUnit",
                backwardCompatibleDefaultBusinessUnit,
                out var normalizedDefault,
                errors))
        {
            selection = PortfolioBusinessUnitSelection.Empty;
            return false;
        }

        var availableBusinessUnits = sourceBusinessUnits
            .Where(static value => !string.IsNullOrWhiteSpace(value))
            .Select(static value => value!.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(static value => value, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (availableBusinessUnits.Any(
                static value => value.Length > MaxCodeLength
                    || value.Any(char.IsControl)))
        {
            errors["businessUnit"] =
            ["Analytics contiene una Business Unit con formato no válido."];
            selection = PortfolioBusinessUnitSelection.Empty;
            return false;
        }

        if (normalizedRequested is not null)
        {
            return TrySelectCanonical(
                normalizedRequested,
                availableBusinessUnits,
                wasDefaulted: false,
                out selection,
                errors);
        }

        if (availableBusinessUnits.Length == 0)
        {
            selection = PortfolioBusinessUnitSelection.Empty;
            return true;
        }

        if (availableBusinessUnits.Length == 1)
        {
            selection = new PortfolioBusinessUnitSelection(
                availableBusinessUnits[0],
                availableBusinessUnits,
                WasDefaulted: true);
            return true;
        }

        if (normalizedDefault is not null)
        {
            return TrySelectCanonical(
                normalizedDefault,
                availableBusinessUnits,
                wasDefaulted: true,
                out selection,
                errors);
        }

        errors["businessUnit"] =
        ["businessUnit es obligatorio cuando el cliente tiene múltiples Business Units y no existe un default backward-compatible configurado."];
        selection = PortfolioBusinessUnitSelection.Empty;
        return false;
    }

    private static bool TrySelectCanonical(
        string candidate,
        IReadOnlyList<string> availableBusinessUnits,
        bool wasDefaulted,
        out PortfolioBusinessUnitSelection selection,
        IDictionary<string, string[]> errors)
    {
        var canonical = availableBusinessUnits.FirstOrDefault(
            value => string.Equals(
                value,
                candidate,
                StringComparison.OrdinalIgnoreCase));

        if (canonical is null)
        {
            errors["businessUnit"] =
            ["businessUnit no está disponible para el cliente autorizado."];
            selection = PortfolioBusinessUnitSelection.Empty;
            return false;
        }

        selection = new PortfolioBusinessUnitSelection(
            canonical,
            availableBusinessUnits,
            wasDefaulted);
        return true;
    }

    private static bool TryNormalize(
        string fieldName,
        string? value,
        out string? normalized,
        IDictionary<string, string[]> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            normalized = null;
            return true;
        }

        normalized = value.Trim();

        if (normalized.Length > MaxCodeLength)
        {
            errors[fieldName] =
            [$"{fieldName} no puede exceder {MaxCodeLength} caracteres."];
            return false;
        }

        if (normalized.Any(char.IsControl))
        {
            errors[fieldName] =
            [$"{fieldName} contiene caracteres no válidos."];
            return false;
        }

        return true;
    }
}

public sealed record PortfolioBusinessUnitSelection(
    string? SelectedBusinessUnit,
    IReadOnlyList<string> AvailableBusinessUnits,
    bool WasDefaulted)
{
    public static PortfolioBusinessUnitSelection Empty { get; } =
        new(null, [], WasDefaulted: false);

    public bool HasMultipleBusinessUnits => AvailableBusinessUnits.Count > 1;
}


/// <summary>
/// Explicit compatibility rules that belong to the Portfolio Control Center API boundary.
/// They do not redefine the dimensional model: source_business_unit remains the canonical
/// business-unit identity.
/// </summary>
public static class PortfolioBusinessUnitPolicy
{
    public const int ClaroCrmClientId = 95;
    public const string ClaroAdministrative = "CLARO ADMINISTRATIVO";
    public const string ClaroGovernment = "CLARO GOBIERNO";

    public static string? GetBackwardCompatibleDefault(int crmClientId) =>
        crmClientId == ClaroCrmClientId
            ? ClaroAdministrative
            : null;

    public static string? ResolveRequestedOrDefault(
        int crmClientId,
        string? requestedBusinessUnit) =>
        requestedBusinessUnit ?? GetBackwardCompatibleDefault(crmClientId);

    public static bool CanUseClientLevelTarget(
        int crmClientId,
        string? businessUnit)
    {
        _ = crmClientId;
        _ = businessUnit;

        // The monthly target remains client/campaign scoped. For CLARO, both
        // business units intentionally share that same target amount/curve;
        // actual recovery is still restricted by BusinessUnit downstream.
        return true;
    }
}
