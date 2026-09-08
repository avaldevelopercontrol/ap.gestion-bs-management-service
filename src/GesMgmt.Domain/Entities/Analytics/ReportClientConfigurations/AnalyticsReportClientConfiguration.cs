namespace GesMgmt.Domain.Entities.Analytics;

public sealed record AnalyticsReportClientConfiguration(
    int ClientId,
    string Name,
    bool IsAvailable,
    string GroupResolution,
    bool HasExplicitGroupConfiguration,
    IReadOnlyList<int> GroupIds,
    IReadOnlyList<AnalyticsReportClientConfigurationGroup> CandidateGroups,
    string? EmbedUrl,
    bool IsReady);
