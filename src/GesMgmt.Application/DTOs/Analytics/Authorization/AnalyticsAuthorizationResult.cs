using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.DTOs.Analytics;

public sealed record AnalyticsAuthorizationResult(
    bool Allowed,
    string? Reason = null)
{
    public static AnalyticsAuthorizationResult Allow() =>
        new(true);

    public static AnalyticsAuthorizationResult Deny(
        string reason) =>
        new(false, reason);
}
