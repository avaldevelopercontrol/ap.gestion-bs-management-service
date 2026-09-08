using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence.Analytics;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

internal sealed class SisgesClientGroupRepository(
    ISisgesQueryExecutor queryExecutor)
    : ISisgesClientGroupRepository
{
    public Task<IReadOnlyList<SisgesClientGroup>> GetAllActiveGroupsAsync(
        CancellationToken cancellationToken) =>
        queryExecutor.QueryAsync<SisgesClientGroup>(
            SisgesClientGroupSql.GetAllActiveGroups,
            null,
            cancellationToken);

    public Task<IReadOnlyList<SisgesClientGroup>> GetActiveGroupsAsync(
        IReadOnlyCollection<int> clientIds,
        CancellationToken cancellationToken)
    {
        var normalizedClientIds = clientIds
            .Where(clientId => clientId > 0)
            .Distinct()
            .OrderBy(clientId => clientId)
            .ToArray();

        if (normalizedClientIds.Length == 0)
        {
            return Task.FromResult<IReadOnlyList<SisgesClientGroup>>(
                Array.Empty<SisgesClientGroup>());
        }

        return queryExecutor.QueryAsync<SisgesClientGroup>(
            SisgesClientGroupSql.GetActiveGroups,
            new { ClientIds = normalizedClientIds },
            cancellationToken);
    }
}
