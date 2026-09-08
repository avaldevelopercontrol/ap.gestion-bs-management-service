using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence.Analytics;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

internal sealed class SisgesUserGroupRepository(
    ISisgesQueryExecutor queryExecutor)
    : ISisgesUserGroupRepository
{
    public Task<IReadOnlyList<int>> GetActiveGroupIdsAsync(
        int userId,
        CancellationToken cancellationToken) =>
        queryExecutor.QueryAsync<int>(
            SisgesUserGroupSql.GetActiveGroups,
            new { UserId = userId },
            cancellationToken);
}
