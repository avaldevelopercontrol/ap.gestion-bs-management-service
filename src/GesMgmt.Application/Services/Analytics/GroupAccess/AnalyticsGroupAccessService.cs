using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Application.Validators.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;

namespace GesMgmt.Application.Services.Analytics;

public sealed class AnalyticsGroupAccessService(
    ISisgesUserGroupRepository users,
    IAnalyticsOptionGroupScopeRepository scopes)
    : IAnalyticsGroupAccessService
{
    public async Task<IReadOnlyList<int>> GetGroupScopedGroupIdsAsync(
        int userId,
        int optionId,
        CancellationToken cancellationToken)
    {
        var userGroupsTask = users.GetActiveGroupIdsAsync(
            userId,
            cancellationToken);
        var allowedGroupsTask = scopes.GetGroupIdsAsync(
            optionId,
            cancellationToken);

        var groupSets = await Task.WhenAll(
            userGroupsTask,
            allowedGroupsTask);

        return groupSets[0]
            .Intersect(groupSets[1])
            .OrderBy(groupId => groupId)
            .ToArray();
    }
}
