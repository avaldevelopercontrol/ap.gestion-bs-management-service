using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.Interfaces.Analytics;

public interface IAnalyticsOptionAccessService
{
    Task<AnalyticsOptionAccessResult> ResolveAsync(
        int userId,
        int optionId,
        CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<int, AnalyticsOptionAccessResult>> ResolveManyAsync(
        int userId,
        IReadOnlyCollection<int> optionIds,
        CancellationToken cancellationToken);
}

public sealed record AnalyticsOptionAccessResult(
    bool Allowed,
    string ScopeMode,
    IReadOnlyList<int> MatchedGroupIds,
    IReadOnlyList<int> ActiveUserGroupIds);
