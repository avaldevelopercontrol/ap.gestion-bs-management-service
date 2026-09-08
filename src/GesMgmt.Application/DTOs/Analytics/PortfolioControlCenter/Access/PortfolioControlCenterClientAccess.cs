namespace GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

public sealed record PortfolioControlCenterClientAccess(
    int? CrmClientId,
    int? ErrorStatusCode,
    string? ErrorTitle,
    string? ErrorDetail)
{
    public bool IsAllowed => CrmClientId.HasValue;

    public static PortfolioControlCenterClientAccess Allowed(int crmClientId) =>
        new(crmClientId, null, null, null);

    public static PortfolioControlCenterClientAccess Error(
        int statusCode,
        string title,
        string detail) =>
        new(null, statusCode, title, detail);

}
