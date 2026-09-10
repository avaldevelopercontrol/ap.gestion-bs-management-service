using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.DTOs.Analytics;

public sealed record AnalyticsPowerBiViewerContextResponse(
    int OptionId,
    bool Allowed,
    bool RequiresClientSelection,
    string ClientSelectionStatus,
    AnalyticsReportClientOption? SelectedClient,
    string? EmbedUrl);
