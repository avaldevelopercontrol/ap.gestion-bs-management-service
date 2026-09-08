using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.DTOs.Analytics;

public sealed record AnalyticsOptionUsersResponse(
    int OptionId,
    IReadOnlyList<int> UserIds);
