using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Domain.Interfaces.Analytics;

public interface IAnalyticsOptionClientScopeRepository
{
    Task<IReadOnlyList<int>> GetClientIdsAsync(
        int optionId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<int>> GetAuthorizedClientIdsAsync(
        int userId,
        int optionId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<AnalyticsAuthorizedClientScopeEntry>> GetAuthorizedClientsAsync(
        int userId,
        int optionId,
        CancellationToken cancellationToken);

    Task<bool> IsAuthorizedClientAsync(
        int userId,
        int optionId,
        int clientId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<AnalyticsOptionClientScopeEntry>> GetActiveScopesAsync(
        IReadOnlyCollection<int> optionIds,
        CancellationToken cancellationToken);

    Task ReplaceAsync(
        int optionId,
        IReadOnlyCollection<int> previousClientIds,
        IReadOnlyCollection<int> clientIds,
        int? userId,
        CancellationToken cancellationToken);
}
