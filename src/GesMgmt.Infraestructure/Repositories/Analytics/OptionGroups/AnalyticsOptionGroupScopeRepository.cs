using System.Text.Json;
using Microsoft.Extensions.Options;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence.Analytics;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

internal sealed class AnalyticsOptionGroupScopeRepository(
    IAnalyticsQueryExecutor queryExecutor,
    AnalyticsDatabaseOptions databaseOptions)
    : IAnalyticsOptionGroupScopeRepository
{
    private readonly int _commandTimeoutSeconds =
        databaseOptions.CommandTimeoutSeconds;

    public async Task<bool> HasAnyScopeAsync(
        int optionId,
        CancellationToken cancellationToken)
    {
        var count = await queryExecutor.QuerySingleAsync<int>(
            AnalyticsOptionGroupScopeSql.HasAnyScope,
            new { OptionId = optionId },
            _commandTimeoutSeconds,
            cancellationToken);

        return count > 0;
    }

    public Task<IReadOnlyList<int>> GetGroupIdsAsync(
        int optionId,
        CancellationToken cancellationToken) =>
        queryExecutor.QueryAsync<int>(
            AnalyticsOptionGroupScopeSql.GetGroups,
            new { OptionId = optionId },
            _commandTimeoutSeconds,
            cancellationToken);

    public Task<IReadOnlyList<AnalyticsOptionGroupScopeEntry>> GetScopesAsync(
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
            return Task.FromResult<IReadOnlyList<AnalyticsOptionGroupScopeEntry>>(
                Array.Empty<AnalyticsOptionGroupScopeEntry>());
        }

        return queryExecutor.QueryAsync<AnalyticsOptionGroupScopeEntry>(
            AnalyticsOptionGroupScopeSql.GetScopes,
            new
            {
                OptionIdsJson = JsonSerializer.Serialize(normalizedOptionIds)
            },
            _commandTimeoutSeconds,
            cancellationToken);
    }

    public Task ReplaceAsync(
        int optionId,
        IReadOnlyCollection<int> previousGroupIds,
        IReadOnlyCollection<int> groupIds,
        int? userId,
        CancellationToken cancellationToken)
    {
        var normalizedPreviousGroupIds = Normalize(previousGroupIds);
        var normalizedGroupIds = Normalize(groupIds);
        var commands = new List<AnalyticsDbCommand>
        {
            new(
                AnalyticsOptionGroupScopeSql.Replace,
                new
                {
                    OptionId = optionId,
                    GroupIdsJson = JsonSerializer.Serialize(normalizedGroupIds),
                    UserId = userId
                })
        };

        commands.Add(new AnalyticsDbCommand(
            AnalyticsOptionGroupScopeAuditSql.Insert,
            new
            {
                OptionId = optionId,
                PreviousGroupIdsJson = JsonSerializer.Serialize(normalizedPreviousGroupIds),
                NewGroupIdsJson = JsonSerializer.Serialize(normalizedGroupIds),
                UserId = userId
            }));

        return queryExecutor.ExecuteTransactionAsync(
            commands,
            _commandTimeoutSeconds,
            cancellationToken);
    }

    private static int[] Normalize(IReadOnlyCollection<int> groupIds) =>
        groupIds
            .Where(groupId => groupId > 0)
            .Distinct()
            .OrderBy(groupId => groupId)
            .ToArray();
}
