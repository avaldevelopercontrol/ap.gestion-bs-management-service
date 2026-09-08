using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.DTOs.Analytics;

public sealed record UpdateAnalyticsOptionClientsRequest(
    IReadOnlyList<int>? ClientIds);
