using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Application.Validators.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;

namespace GesMgmt.Application.Services.Analytics;

public sealed class AnalyticsOptionAccessService(
    ISisgesUserGroupRepository userGroups,
    IAnalyticsOptionGroupScopeRepository optionGroups,
    IAnalyticsAccessService clientAccessService)
    : IAnalyticsOptionAccessService
{
    public async Task<AnalyticsOptionAccessResult> ResolveAsync(
        int userId,
        int optionId,
        CancellationToken cancellationToken)
    {
        if (userId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(userId));
        }

        if (optionId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(optionId));
        }

        var results = await ResolveManyAsync(
            userId,
            [optionId],
            cancellationToken);

        return results[optionId];
    }

    public async Task<IReadOnlyDictionary<int, AnalyticsOptionAccessResult>> ResolveManyAsync(
        int userId,
        IReadOnlyCollection<int> optionIds,
        CancellationToken cancellationToken)
    {
        if (userId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(userId));
        }

        var normalizedOptionIds = optionIds
            .Where(optionId => optionId > 0)
            .Distinct()
            .OrderBy(optionId => optionId)
            .ToArray();

        if (normalizedOptionIds.Length == 0)
        {
            return new Dictionary<int, AnalyticsOptionAccessResult>();
        }

        // Preserve the distinction between "never configured" and
        // "configured but currently inactive" by loading every scope row,
        // including inactive rows, in one Analytics round-trip.
        var configuredScopes = await optionGroups.GetScopesAsync(
            normalizedOptionIds,
            cancellationToken);
        var requestedOptionIds = normalizedOptionIds.ToHashSet();
        var scopesByOption = configuredScopes
            .Where(scope => requestedOptionIds.Contains(scope.OptionId))
            .GroupBy(scope => scope.OptionId)
            .ToDictionary(group => group.Key, group => group.ToArray());

        // SISGES remains the source of truth. Read the user's active groups
        // once for this resolution batch and share that request-scoped snapshot
        // across every requested option. Nothing is cached between requests.
        var activeUserGroupsTask = userGroups.GetActiveGroupIdsAsync(
            userId,
            cancellationToken);

        var legacyOptionIds = normalizedOptionIds
            .Where(optionId => !scopesByOption.ContainsKey(optionId))
            .ToArray();
        var legacyClientIdsTask = legacyOptionIds.Length == 0
            ? Task.FromResult<IReadOnlyDictionary<int, IReadOnlyList<int>>>(
                new Dictionary<int, IReadOnlyList<int>>())
            : clientAccessService.GetClientScopedClientIdsManyAsync(
                userId,
                legacyOptionIds,
                cancellationToken);

        await Task.WhenAll(activeUserGroupsTask, legacyClientIdsTask);

        var activeUserGroupIds = NormalizeIds(await activeUserGroupsTask);
        var legacyClientIdsByOption = await legacyClientIdsTask;
        var activeUserGroupSet = activeUserGroupIds.ToHashSet();
        var results = new Dictionary<int, AnalyticsOptionAccessResult>(
            normalizedOptionIds.Length);

        foreach (var optionId in normalizedOptionIds)
        {
            if (scopesByOption.TryGetValue(optionId, out var optionScopes))
            {
                var matchedGroupIds = NormalizeIds(
                        optionScopes
                            .Where(scope => scope.IsActive)
                            .Select(scope => scope.SisgesGroupId))
                    .Where(activeUserGroupSet.Contains)
                    .ToArray();

                results[optionId] = new AnalyticsOptionAccessResult(
                    matchedGroupIds.Length > 0,
                    "GROUP",
                    matchedGroupIds,
                    activeUserGroupIds);
                continue;
            }

            var legacyAllowed =
                legacyClientIdsByOption.TryGetValue(optionId, out var legacyClientIds) &&
                legacyClientIds.Count > 0;
            results[optionId] = new AnalyticsOptionAccessResult(
                legacyAllowed,
                "CLIENT_LEGACY",
                Array.Empty<int>(),
                activeUserGroupIds);
        }

        return results;
    }

    private static int[] NormalizeIds(IEnumerable<int> ids) =>
        ids
            .Where(id => id > 0)
            .Distinct()
            .OrderBy(id => id)
            .ToArray();
}
