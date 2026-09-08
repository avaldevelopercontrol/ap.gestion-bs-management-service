using System.Text.Json;
using Microsoft.Extensions.Options;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence.Analytics;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

internal sealed class AnalyticsPowerBiConfigurationWriter(
    IAnalyticsQueryExecutor queryExecutor,
    AnalyticsDatabaseOptions databaseOptions,
    IAnalyticsAccessCache cache)
    : IAnalyticsPowerBiConfigurationWriter
{
    private readonly int _commandTimeoutSeconds =
        databaseOptions.CommandTimeoutSeconds;

    public async Task UpdateAsync(
        int optionId,
        string optionCode,
        string optionName,
        bool isActive,
        IReadOnlyCollection<int> previousGroupIds,
        IReadOnlyCollection<int> groupIds,
        IReadOnlyCollection<AnalyticsReportClientPublicationUpdate> publications,
        int updatedBy,
        CancellationToken cancellationToken)
    {
        var normalizedPreviousGroupIds = NormalizeGroupIds(previousGroupIds);
        var normalizedGroupIds = NormalizeGroupIds(groupIds);
        var optionParameters = new
        {
            OptionId = optionId,
            OptionCode = optionCode.Trim(),
            OptionName = optionName.Trim(),
            IsActive = isActive,
            UserId = updatedBy
        };

        var commands = new List<AnalyticsDbCommand>
        {
            new(
                AnalyticsOptionConfigSql.Update,
                optionParameters),
            new(
                AnalyticsOptionConfigSql.InsertMissing,
                optionParameters)
        };

        if (!normalizedPreviousGroupIds.SequenceEqual(normalizedGroupIds))
        {
            AddOptionGroupCommands(
                commands,
                optionId,
                normalizedPreviousGroupIds,
                normalizedGroupIds,
                updatedBy);
        }

        if (publications.Count > 0)
        {
            commands.Add(AnalyticsReportClientPublicationPatch.CreateCommand(
                optionId,
                publications,
                updatedBy));
        }

        await queryExecutor.ExecuteTransactionAsync(
            commands,
            _commandTimeoutSeconds,
            cancellationToken);

        cache.Remove(AnalyticsAccessCacheKeys.ActiveOptions);

        if (publications.Count > 0)
        {
            cache.Remove(AnalyticsAccessCacheKeys.ReportClientPublications(optionId));
        }
    }

    private static void AddOptionGroupCommands(
        ICollection<AnalyticsDbCommand> commands,
        int optionId,
        IReadOnlyCollection<int> previousGroupIds,
        IReadOnlyCollection<int> groupIds,
        int updatedBy)
    {
        commands.Add(new AnalyticsDbCommand(
            AnalyticsOptionGroupScopeSql.Replace,
            new
            {
                OptionId = optionId,
                GroupIdsJson = JsonSerializer.Serialize(groupIds),
                UserId = updatedBy
            }));

        commands.Add(new AnalyticsDbCommand(
            AnalyticsOptionGroupScopeAuditSql.Insert,
            new
            {
                OptionId = optionId,
                PreviousGroupIdsJson = JsonSerializer.Serialize(previousGroupIds),
                NewGroupIdsJson = JsonSerializer.Serialize(groupIds),
                UserId = updatedBy
            }));
    }

    private static int[] NormalizeGroupIds(
        IEnumerable<int> groupIds) =>
        groupIds
            .Where(groupId => groupId > 0)
            .Distinct()
            .OrderBy(groupId => groupId)
            .ToArray();
}
