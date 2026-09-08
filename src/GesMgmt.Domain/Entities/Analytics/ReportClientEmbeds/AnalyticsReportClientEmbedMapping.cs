namespace GesMgmt.Domain.Entities.Analytics;

public sealed record AnalyticsReportClientEmbedMapping
{
    public required string ReportClientValue { get; init; }
    public required string EmbedUrl { get; init; }
}
