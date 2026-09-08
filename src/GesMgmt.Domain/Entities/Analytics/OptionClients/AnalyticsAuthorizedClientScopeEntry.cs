namespace GesMgmt.Domain.Entities.Analytics;

public sealed record AnalyticsAuthorizedClientScopeEntry
{
    public int CrmClientId { get; init; }
    public required string Name { get; init; }
}
