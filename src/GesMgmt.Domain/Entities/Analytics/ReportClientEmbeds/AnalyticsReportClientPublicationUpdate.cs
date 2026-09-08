namespace GesMgmt.Domain.Entities.Analytics;

public sealed record AnalyticsReportClientPublicationUpdate(
    int ClientId,
    string Name,
    IReadOnlyCollection<int>? GroupIds,
    string? EmbedUrl);
