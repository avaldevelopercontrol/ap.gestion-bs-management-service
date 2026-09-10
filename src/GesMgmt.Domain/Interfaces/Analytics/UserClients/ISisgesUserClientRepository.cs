using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Domain.Interfaces.Analytics;

public interface ISisgesUserClientRepository
{
    Task<IReadOnlyList<int>> GetActiveClientIdsAsync(
        int userId,
        CancellationToken cancellationToken);

    Task<bool> IsActiveClientAsync(
        int userId,
        int clientId,
        CancellationToken cancellationToken);
}
