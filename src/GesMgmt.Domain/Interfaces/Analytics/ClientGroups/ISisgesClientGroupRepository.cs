using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Domain.Interfaces.Analytics;

public interface ISisgesClientGroupRepository
{
    Task<IReadOnlyList<SisgesClientGroup>> GetAllActiveGroupsAsync(
        CancellationToken cancellationToken);

    Task<IReadOnlyList<SisgesClientGroup>> GetActiveGroupsAsync(
        IReadOnlyCollection<int> clientIds,
        CancellationToken cancellationToken);
}
