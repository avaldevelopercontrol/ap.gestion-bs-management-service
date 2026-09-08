namespace GesMgmt.Domain.Entities.Analytics;

public sealed record AnalyticsOptionClientScopeEntry
{
    public int OptionId { get; init; }
    public int CrmClientId { get; init; }
}
