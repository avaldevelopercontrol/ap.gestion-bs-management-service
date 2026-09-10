namespace GesMgmt.Domain.Entities.Analytics;

public sealed record AnalyticsAllowedClient(
    int CrmClientId,
    string Name);
