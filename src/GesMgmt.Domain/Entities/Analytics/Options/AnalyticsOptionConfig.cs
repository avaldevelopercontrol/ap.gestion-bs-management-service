namespace GesMgmt.Domain.Entities.Analytics;

public sealed class AnalyticsOptionConfig
{
    public int OptionId { get; set; }
    public string OptionCode { get; set; } = string.Empty;
    public string OptionName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime? CreatedAt { get; set; }
    public int? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
