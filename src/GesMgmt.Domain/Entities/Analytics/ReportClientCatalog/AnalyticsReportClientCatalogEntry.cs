namespace GesMgmt.Domain.Entities.Analytics;

public sealed class AnalyticsReportClientCatalogEntry
{
    public int OptionId { get; set; }
    public int CrmClientId { get; set; }
    public string ReportClientValue { get; set; } = string.Empty;
}
