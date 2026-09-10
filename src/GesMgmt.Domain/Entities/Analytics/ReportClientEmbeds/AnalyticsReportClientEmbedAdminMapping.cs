namespace GesMgmt.Domain.Entities.Analytics;

public sealed record AnalyticsReportClientEmbedAdminMapping
{
    public int CrmClientId { get; init; }
    public required string ReportClientValue { get; init; }
    public string? EmbedUrl { get; init; }
}
