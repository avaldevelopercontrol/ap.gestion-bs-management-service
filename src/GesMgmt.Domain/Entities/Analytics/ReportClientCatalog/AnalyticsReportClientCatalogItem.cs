namespace GesMgmt.Domain.Entities.Analytics;

public sealed record AnalyticsReportClientCatalogItem
{
    public int CrmClientId { get; init; }
    public required string ReportClientValue { get; init; }
}
