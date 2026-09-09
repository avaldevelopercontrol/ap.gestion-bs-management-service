namespace GesMgmt.Domain.Entities.Analytics;

public sealed class AnalyticsOptionClientScopeEntry
{
    public int OptionId { get; set; }
    public int CrmClientId { get; set; }
    public bool IsActive { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
