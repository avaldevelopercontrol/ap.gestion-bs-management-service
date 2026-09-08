using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.DTOs.Analytics;

public sealed record UpdateAnalyticsPowerBiConfigurationRequest(
    string? OptionCode,
    string? OptionName,
    bool IsActive,
    IReadOnlyList<int>? GroupIds,
    IReadOnlyList<UpdateAnalyticsOptionReportClientEmbed>? Publications);
