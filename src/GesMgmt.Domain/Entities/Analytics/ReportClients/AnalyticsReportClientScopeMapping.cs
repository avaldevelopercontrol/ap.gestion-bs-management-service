namespace GesMgmt.Domain.Entities.Analytics;

public sealed record AnalyticsReportClientScopeMapping
{
    public int CrmClientId { get; init; }
    public required string ReportClientValue { get; init; }
    public int SisgesGroupId { get; init; }
}
