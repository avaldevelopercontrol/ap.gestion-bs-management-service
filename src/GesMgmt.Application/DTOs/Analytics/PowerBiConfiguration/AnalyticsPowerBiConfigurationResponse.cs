using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.DTOs.Analytics;

public sealed record AnalyticsPowerBiConfigurationResponse(
    int OptionId,
    bool IsConfigured,
    IReadOnlyList<int> GroupIds,
    IReadOnlyList<AnalyticsPowerBiConfigurationGroup> AvailableGroups,
    IReadOnlyList<AnalyticsOptionReportClientEmbed> Clients);
