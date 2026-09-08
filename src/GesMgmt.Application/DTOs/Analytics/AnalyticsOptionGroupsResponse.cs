using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.DTOs.Analytics;

public sealed record AnalyticsOptionGroupsResponse(
    int OptionId,
    IReadOnlyList<int> GroupIds);
