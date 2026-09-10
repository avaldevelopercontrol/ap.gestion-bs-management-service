namespace GesMgmt.Domain.Constants.Analytics;

public static class AnalyticsReportClientGroupResolution
{
    public const string Configured = "CONFIGURED";
    public const string AutoDetected = "AUTO_DETECTED";
    public const string Ambiguous = "AMBIGUOUS";
    public const string Missing = "MISSING";
    public const string InvalidConfigured = "INVALID_CONFIGURED";
    public const string Unavailable = "UNAVAILABLE";

    public static bool IsResolved(string value) =>
        string.Equals(value, Configured, StringComparison.Ordinal) ||
        string.Equals(value, AutoDetected, StringComparison.Ordinal);
}
