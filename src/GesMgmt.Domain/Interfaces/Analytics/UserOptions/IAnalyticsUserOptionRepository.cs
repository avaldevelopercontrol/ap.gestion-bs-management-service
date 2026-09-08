using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Domain.Interfaces.Analytics;

public interface IAnalyticsUserOptionRepository
{
    Task<bool> HasAccessAsync(
        int userId,
        int optionId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<AnalyticsUserOption>> GetUserOptionsAsync(
        int userId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<int>> GetUserIdsAsync(
        int optionId,
        CancellationToken cancellationToken);

    Task ReplaceAsync(
        int optionId,
        IReadOnlyCollection<int> previousUserIds,
        IReadOnlyCollection<int> userIds,
        int? adminUserId,
        CancellationToken cancellationToken);
}
