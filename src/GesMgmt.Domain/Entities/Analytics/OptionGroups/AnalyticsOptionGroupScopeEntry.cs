namespace GesMgmt.Domain.Entities.Analytics;

public sealed record AnalyticsOptionGroupScopeEntry
{
    public int OptionId { get; init; }
    public int SisgesGroupId { get; init; }
    public bool IsActive { get; init; }
}
