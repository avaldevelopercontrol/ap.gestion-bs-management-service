using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.DTOs.Analytics;

public sealed record AnalyticsOptionReportClientEmbed(
    int ClientId,
    string Name,
    bool IsAvailable,
    string GroupResolution,
    bool HasExplicitGroupConfiguration,
    IReadOnlyList<int> GroupIds,
    IReadOnlyList<AnalyticsOptionReportClientGroup> CandidateGroups,
    string? EmbedUrl,
    bool IsReady);
