using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.DTOs.Analytics;

public sealed record AnalyticsOptionClientsResponse(
    int OptionId,
    IReadOnlyList<int> ClientIds);
