using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Domain.Interfaces.Analytics;

public interface IAnalyticsOptionGroupScopeRepository
{
    Task<bool> HasAnyScopeAsync(
        int optionId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<int>> GetGroupIdsAsync(
        int optionId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<AnalyticsOptionGroupScopeEntry>> GetScopesAsync(
        IReadOnlyCollection<int> optionIds,
        CancellationToken cancellationToken);

    Task ReplaceAsync(
        int optionId,
        IReadOnlyCollection<int> previousGroupIds,
        IReadOnlyCollection<int> groupIds,
        int? userId,
        CancellationToken cancellationToken);
}
