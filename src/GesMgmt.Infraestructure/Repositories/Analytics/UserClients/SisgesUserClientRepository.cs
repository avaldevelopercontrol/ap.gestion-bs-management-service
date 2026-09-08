using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence.Analytics;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

internal sealed class SisgesUserClientRepository(ISisgesQueryExecutor executor)
    : ISisgesUserClientRepository
{
    public Task<IReadOnlyList<int>> GetActiveClientIdsAsync(
        int userId,
        CancellationToken cancellationToken) =>
        executor.QueryAsync<int>(
            SisgesUserClientSql.GetActiveClients,
            new { UserId = userId },
            cancellationToken);

    public async Task<bool> IsActiveClientAsync(
        int userId,
        int clientId,
        CancellationToken cancellationToken)
    {
        if (userId <= 0 || clientId <= 0)
        {
            return false;
        }

        var result = await executor.QueryAsync<int>(
            SisgesUserClientSql.IsActiveClient,
            new
            {
                UserId = userId,
                ClientId = clientId
            },
            cancellationToken);

        return result.Count > 0;
    }
}
