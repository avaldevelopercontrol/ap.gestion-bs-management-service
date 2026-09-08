using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Domain.Interfaces.Analytics;

public interface ISisgesUserGroupRepository
{
    Task<IReadOnlyList<int>> GetActiveGroupIdsAsync(
        int userId,
        CancellationToken cancellationToken);
}
