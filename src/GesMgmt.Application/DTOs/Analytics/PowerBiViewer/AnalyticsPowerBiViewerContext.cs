using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.DTOs.Analytics;

public static class AnalyticsPowerBiClientSelectionStatus
{
    public const string NotRequired = "NOT_REQUIRED";
    public const string Valid = "VALID";
    public const string Missing = "MISSING";
    public const string Invalid = "INVALID";
}

public sealed record AnalyticsPowerBiViewerSelection(
    int ClientId,
    string Name);

public sealed record AnalyticsPowerBiViewerContext(
    bool Allowed,
    bool RequiresClientSelection,
    string ClientSelectionStatus,
    AnalyticsReportClientOption? SelectedClient,
    string? EmbedUrl);
