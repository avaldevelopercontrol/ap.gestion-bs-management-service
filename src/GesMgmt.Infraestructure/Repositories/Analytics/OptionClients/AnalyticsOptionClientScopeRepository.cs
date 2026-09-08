using System.Text.Json;
using Microsoft.Extensions.Options;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence.Analytics;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

internal sealed class AnalyticsOptionClientScopeRepository(
    IAnalyticsQueryExecutor queryExecutor,
    AnalyticsDatabaseOptions databaseOptions)
    : IAnalyticsOptionClientScopeRepository
{
    private readonly int _commandTimeoutSeconds =
        databaseOptions.CommandTimeoutSeconds;

    public Task<IReadOnlyList<int>> GetClientIdsAsync(
        int optionId,
        CancellationToken cancellationToken) =>
        queryExecutor.QueryAsync<int>(
            AnalyticsOptionClientScopeSql.GetClients,
            new { OptionId = optionId },
            _commandTimeoutSeconds,
            cancellationToken);

    public Task<IReadOnlyList<int>> GetAuthorizedClientIdsAsync(
        int userId,
        int optionId,
        CancellationToken cancellationToken)
    {
        if (UsesInheritedClientAccess(optionId))
        {
            return queryExecutor.QueryAsync<int>(
                AnalyticsOptionClientScopeSql.GetInheritedClientIds,
                new { OptionId = optionId },
                _commandTimeoutSeconds,
                cancellationToken);
        }

        return queryExecutor.QueryAsync<int>(
            AnalyticsOptionClientScopeSql.GetAuthorizedClientIds,
            new
            {
                UserId = userId,
                OptionId = optionId
            },
            _commandTimeoutSeconds,
            cancellationToken);
    }

    public Task<IReadOnlyList<AnalyticsAuthorizedClientScopeEntry>> GetAuthorizedClientsAsync(
        int userId,
        int optionId,
        CancellationToken cancellationToken)
    {
        if (UsesInheritedClientAccess(optionId))
        {
            return queryExecutor.QueryAsync<AnalyticsAuthorizedClientScopeEntry>(
                AnalyticsOptionClientScopeSql.GetInheritedClients,
                new { OptionId = optionId },
                _commandTimeoutSeconds,
                cancellationToken);
        }

        return queryExecutor.QueryAsync<AnalyticsAuthorizedClientScopeEntry>(
            AnalyticsOptionClientScopeSql.GetAuthorizedClients,
            new
            {
                UserId = userId,
                OptionId = optionId
            },
            _commandTimeoutSeconds,
            cancellationToken);
    }

    public async Task<bool> IsAuthorizedClientAsync(
        int userId,
        int optionId,
        int clientId,
        CancellationToken cancellationToken)
    {
        if (userId <= 0 || optionId <= 0 || clientId <= 0)
        {
            return false;
        }

        var result = UsesInheritedClientAccess(optionId)
            ? await queryExecutor.QuerySingleOrDefaultAsync<int>(
                AnalyticsOptionClientScopeSql.IsInheritedClient,
                new
                {
                    OptionId = optionId,
                    ClientId = clientId
                },
                _commandTimeoutSeconds,
                cancellationToken)
            : await queryExecutor.QuerySingleOrDefaultAsync<int>(
                AnalyticsOptionClientScopeSql.IsAuthorizedClient,
                new
                {
                    UserId = userId,
                    OptionId = optionId,
                    ClientId = clientId
                },
                _commandTimeoutSeconds,
                cancellationToken);

        return result == 1;
    }

    public Task<IReadOnlyList<AnalyticsOptionClientScopeEntry>> GetActiveScopesAsync(
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
            return Task.FromResult<IReadOnlyList<AnalyticsOptionClientScopeEntry>>(
                Array.Empty<AnalyticsOptionClientScopeEntry>());
        }

        return queryExecutor.QueryAsync<AnalyticsOptionClientScopeEntry>(
            AnalyticsOptionClientScopeSql.GetActiveScopes,
            new
            {
                OptionIdsJson = JsonSerializer.Serialize(normalizedOptionIds)
            },
            _commandTimeoutSeconds,
            cancellationToken);
    }

    public Task ReplaceAsync(
        int optionId,
        IReadOnlyCollection<int> previousClientIds,
        IReadOnlyCollection<int> clientIds,
        int? userId,
        CancellationToken cancellationToken)
    {
        var normalizedPreviousClientIds = Normalize(previousClientIds);
        var normalizedClientIds = Normalize(clientIds);
        var commands = new List<AnalyticsDbCommand>
        {
            new(
                AnalyticsOptionClientScopeSql.Replace,
                new
                {
                    OptionId = optionId,
                    ClientIdsJson = JsonSerializer.Serialize(normalizedClientIds),
                    UserId = userId
                })
        };

        commands.Add(new AnalyticsDbCommand(
            AnalyticsOptionClientScopeAuditSql.Insert,
            new
            {
                OptionId = optionId,
                PreviousClientIdsJson = JsonSerializer.Serialize(normalizedPreviousClientIds),
                NewClientIdsJson = JsonSerializer.Serialize(normalizedClientIds),
                UserId = userId
            }));

        return queryExecutor.ExecuteTransactionAsync(
            commands,
            _commandTimeoutSeconds,
            cancellationToken);
    }

    // Portfolio Control Center is an operational module whose data scope is
    // inherited from SISGES user/group/client assignments. Requiring a second
    // per-user row in Analytics duplicates authorization state and does not
    // scale as users are added or moved between groups. Other options retain
    // the explicit user_option_scope behavior.
    private static bool UsesInheritedClientAccess(int optionId) =>
        optionId == AnalyticsOptionIds.PortfolioControlCenter;

    private static int[] Normalize(IReadOnlyCollection<int> clientIds) =>
        clientIds
            .Where(clientId => clientId > 0)
            .Distinct()
            .OrderBy(clientId => clientId)
            .ToArray();
}
