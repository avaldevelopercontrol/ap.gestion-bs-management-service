using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.DTOs.Analytics;

public enum AnalyticsAdministrationCommandStatus
{
    Success,
    NotFound,
    InvalidRequest
}

public sealed record AnalyticsAdministrationCommandResult(
    AnalyticsAdministrationCommandStatus Status,
    string? Title = null,
    string? Detail = null)
{
    public static AnalyticsAdministrationCommandResult Success() =>
        new(AnalyticsAdministrationCommandStatus.Success);

    public static AnalyticsAdministrationCommandResult NotFound() =>
        new(AnalyticsAdministrationCommandStatus.NotFound);

    public static AnalyticsAdministrationCommandResult Invalid(
        string title,
        string detail) =>
        new(
            AnalyticsAdministrationCommandStatus.InvalidRequest,
            title,
            detail);
}
