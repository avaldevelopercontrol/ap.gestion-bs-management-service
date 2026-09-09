namespace GesMgmt.Domain.Entities.Analytics;

public sealed class AnalyticsClientDimension
{
    public int ClientKey { get; set; }
    public int CrmClientId { get; set; }
    public string? ClientCode { get; set; }
    public string? ClientName { get; set; }
}
