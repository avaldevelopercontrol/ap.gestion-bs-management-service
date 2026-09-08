using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Application.Validators.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;

namespace GesMgmt.Application.Services.Analytics;

public sealed class AnalyticsAccessService(
    ISisgesUserClientRepository users,
    IAnalyticsOptionClientScopeRepository scopes) : IAnalyticsAccessService
{
    public async Task<IReadOnlyList<AnalyticsAllowedClient>> GetAllowedClientsAsync(
        int userId,
        int optionId,
        CancellationToken cancellationToken)
    {
        var authorizedClients = await scopes.GetAuthorizedClientsAsync(
            userId,
            optionId,
            cancellationToken);

        if (authorizedClients.Count == 0)
        {
            return [];
        }

        var userClients = await users.GetActiveClientIdsAsync(
            userId,
            cancellationToken);
        var activeUserClientIds = NormalizeClientIds(userClients).ToHashSet();

        return authorizedClients
            .Where(client =>
                client.CrmClientId > 0 &&
                activeUserClientIds.Contains(client.CrmClientId))
            .GroupBy(client => client.CrmClientId)
            .Select(group =>
            {
                var clientId = group.Key;
                var name = group
                    .Select(client => client.Name?.Trim())
                    .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value))
                    ?? $"Cartera {clientId}";

                return new AnalyticsAllowedClient(clientId, name);
            })
            .OrderBy(client => client.CrmClientId)
            .ToArray();
    }

    public async Task<IReadOnlyList<int>> GetAllowedClientIdsAsync(
        int userId,
        int optionId,
        CancellationToken cancellationToken)
    {
        // Resolve the Analytics-side authorization and client scope first.
        // Portfolio Control Center inherits authorization from its active client
        // scope; options with explicit user assignment keep user_option_scope.
        // A denied/unconfigured option therefore avoids the SISGES read.
        var allowedClients = await scopes.GetAuthorizedClientIdsAsync(
            userId,
            optionId,
            cancellationToken);

        if (allowedClients.Count == 0)
        {
            return [];
        }

        // SISGES remains the source of truth for the user's current client
        // assignments, so revocations continue to take effect immediately.
        var userClients = await users.GetActiveClientIdsAsync(
            userId,
            cancellationToken);

        return IntersectClientIds(userClients, allowedClients);
    }

    public async Task<bool> IsClientAllowedAsync(
        int userId,
        int optionId,
        int clientId,
        CancellationToken cancellationToken)
    {
        if (userId <= 0 || optionId <= 0 || clientId <= 0)
        {
            return false;
        }

        // The exact checks target different databases and return at most one row.
        // Running them concurrently reduces authorization latency for the normal
        // Portfolio Control Center path without caching user permissions.
        var analyticsAuthorizationTask = scopes.IsAuthorizedClientAsync(
            userId,
            optionId,
            clientId,
            cancellationToken);
        var sisgesAssignmentTask = users.IsActiveClientAsync(
            userId,
            clientId,
            cancellationToken);

        await Task.WhenAll(analyticsAuthorizationTask, sisgesAssignmentTask);

        return await analyticsAuthorizationTask && await sisgesAssignmentTask;
    }

    public async Task<IReadOnlyList<int>> GetClientScopedClientIdsAsync(
        int userId,
        int optionId,
        CancellationToken cancellationToken)
    {
        var results = await GetClientScopedClientIdsManyAsync(
            userId,
            [optionId],
            cancellationToken);

        return results.TryGetValue(optionId, out var clientIds)
            ? clientIds
            : Array.Empty<int>();
    }

    public async Task<IReadOnlyDictionary<int, IReadOnlyList<int>>> GetClientScopedClientIdsManyAsync(
        int userId,
        IReadOnlyCollection<int> optionIds,
        CancellationToken cancellationToken)
    {
        var normalizedOptionIds = optionIds
            .Where(optionId => optionId > 0)
            .Distinct()
            .OrderBy(optionId => optionId)
            .ToArray();

        if (normalizedOptionIds.Length == 0)
        {
            return new Dictionary<int, IReadOnlyList<int>>();
        }

        // These reads are independent and target different authorization
        // sources. Execute them concurrently, but share the resulting SISGES
        // client snapshot only inside this request so revocations remain fresh
        // on the next request.
        var userClientsTask = users.GetActiveClientIdsAsync(
            userId,
            cancellationToken);
        var optionScopesTask = scopes.GetActiveScopesAsync(
            normalizedOptionIds,
            cancellationToken);

        await Task.WhenAll(userClientsTask, optionScopesTask);

        var activeUserClientSet = NormalizeClientIds(await userClientsTask)
            .ToHashSet();
        var requestedOptionIds = normalizedOptionIds.ToHashSet();
        var optionClientIds = (await optionScopesTask)
            .Where(scope => requestedOptionIds.Contains(scope.OptionId))
            .GroupBy(scope => scope.OptionId)
            .ToDictionary(
                group => group.Key,
                group => NormalizeClientIds(
                    group.Select(scope => scope.CrmClientId)));

        var results = new Dictionary<int, IReadOnlyList<int>>(
            normalizedOptionIds.Length);

        foreach (var optionId in normalizedOptionIds)
        {
            if (!optionClientIds.TryGetValue(optionId, out var allowedClientIds))
            {
                results[optionId] = Array.Empty<int>();
                continue;
            }

            results[optionId] = allowedClientIds
                .Where(activeUserClientSet.Contains)
                .ToArray();
        }

        return results;
    }

    private static IReadOnlyList<int> IntersectClientIds(
        IReadOnlyList<int> userClients,
        IReadOnlyList<int> allowedClients) =>
        userClients
            .Intersect(allowedClients)
            .OrderBy(clientId => clientId)
            .ToArray();

    private static int[] NormalizeClientIds(IEnumerable<int> clientIds) =>
        clientIds
            .Where(clientId => clientId > 0)
            .Distinct()
            .OrderBy(clientId => clientId)
            .ToArray();
}
