using System.Text.Json;
using Microsoft.Extensions.Options;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence.Analytics;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

internal sealed class AnalyticsUserOptionRepository(
    IAnalyticsQueryExecutor executor,
    AnalyticsDatabaseOptions databaseOptions)
    : IAnalyticsUserOptionRepository
{
    private readonly int _commandTimeoutSeconds =
        databaseOptions.CommandTimeoutSeconds;

    public async Task<bool> HasAccessAsync(
        int userId,
        int optionId,
        CancellationToken cancellationToken)
    {
        var count = await executor.QuerySingleAsync<int>(
            AnalyticsUserOptionSql.HasAccess,
            new
            {
                UserId = userId,
                OptionId = optionId
            },
            _commandTimeoutSeconds,
            cancellationToken);

        return count > 0;
    }

    public Task<IReadOnlyList<AnalyticsUserOption>> GetUserOptionsAsync(
        int userId,
        CancellationToken cancellationToken) =>
        executor.QueryAsync<AnalyticsUserOption>(
            AnalyticsUserOptionSql.GetUserOptions,
            new { UserId = userId },
            _commandTimeoutSeconds,
            cancellationToken);

    public Task<IReadOnlyList<int>> GetUserIdsAsync(
        int optionId,
        CancellationToken cancellationToken) =>
        executor.QueryAsync<int>(
            AnalyticsUserOptionSql.GetUsers,
            new { OptionId = optionId },
            _commandTimeoutSeconds,
            cancellationToken);

    public Task ReplaceAsync(
        int optionId,
        IReadOnlyCollection<int> previousUserIds,
        IReadOnlyCollection<int> userIds,
        int? adminUserId,
        CancellationToken cancellationToken)
    {
        var normalizedPreviousUserIds = Normalize(previousUserIds);
        var normalizedUserIds = Normalize(userIds);
        var commands = new List<AnalyticsDbCommand>
        {
            new(
                AnalyticsUserOptionSql.Replace,
                new
                {
                    OptionId = optionId,
                    UserIdsJson = JsonSerializer.Serialize(normalizedUserIds),
                    AdminUserId = adminUserId
                })
        };

        commands.Add(new AnalyticsDbCommand(
            AnalyticsUserOptionScopeAuditSql.Insert,
            new
            {
                OptionId = optionId,
                PreviousUserIdsJson = JsonSerializer.Serialize(normalizedPreviousUserIds),
                NewUserIdsJson = JsonSerializer.Serialize(normalizedUserIds),
                UserId = adminUserId
            }));

        return executor.ExecuteTransactionAsync(
            commands,
            _commandTimeoutSeconds,
            cancellationToken);
    }

    private static int[] Normalize(IReadOnlyCollection<int> userIds) =>
        userIds
            .Where(userId => userId > 0)
            .Distinct()
            .OrderBy(userId => userId)
            .ToArray();
}
