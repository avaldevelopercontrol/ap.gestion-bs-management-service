namespace GesMgmt.Domain.Entities.Analytics;

public sealed class AnalyticsUserOptionScopeEntry
{
    public int UserId { get; set; }
    public int OptionId { get; set; }
    public bool IsActive { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
