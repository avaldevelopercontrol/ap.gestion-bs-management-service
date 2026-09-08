using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.DTOs.Analytics;

public sealed record AnalyticsUserAllowedGroupsResponse(
    int OptionId,
    bool Allowed,
    string ScopeMode,
    IReadOnlyList<int> GroupIds,
    bool RequiresClientSelection);
