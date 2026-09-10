using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.Interfaces.Analytics;

public interface IAnalyticsAccessService
{
    Task<IReadOnlyList<AnalyticsAllowedClient>> GetAllowedClientsAsync(
        int userId,
        int optionId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<int>> GetAllowedClientIdsAsync(
        int userId,
        int optionId,
        CancellationToken cancellationToken);

    Task<bool> IsClientAllowedAsync(
        int userId,
        int optionId,
        int clientId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<int>> GetClientScopedClientIdsAsync(
        int userId,
        int optionId,
        CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<int, IReadOnlyList<int>>> GetClientScopedClientIdsManyAsync(
        int userId,
        IReadOnlyCollection<int> optionIds,
        CancellationToken cancellationToken);
}
