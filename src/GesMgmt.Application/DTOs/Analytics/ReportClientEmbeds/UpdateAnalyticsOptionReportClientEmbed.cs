using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.DTOs.Analytics;

public sealed record UpdateAnalyticsOptionReportClientEmbed(
    int ClientId,
    string Name,
    IReadOnlyList<int>? GroupIds,
    string? EmbedUrl);
