using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.DTOs.Analytics;

public sealed record AnalyticsPowerBiConfigurationGroup(
    int GroupId,
    int ClientId,
    string Name);
