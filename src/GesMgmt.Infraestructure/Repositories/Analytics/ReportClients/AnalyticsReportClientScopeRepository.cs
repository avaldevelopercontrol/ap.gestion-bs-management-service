using System.Text.Json;
using Microsoft.Extensions.Options;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence.Analytics;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

internal sealed class AnalyticsReportClientScopeRepository(
    IAnalyticsQueryExecutor queryExecutor,
    AnalyticsDatabaseOptions databaseOptions)
    : IAnalyticsReportClientScopeRepository
{
    private readonly int _commandTimeoutSeconds =
        databaseOptions.CommandTimeoutSeconds;

    public async Task<bool> HasAnyScopeAsync(
        int optionId,
        CancellationToken cancellationToken)
    {
        var count = await queryExecutor.QuerySingleAsync<int>(
            AnalyticsReportClientScopeSql.HasAnyScope,
            new { OptionId = optionId },
            _commandTimeoutSeconds,
            cancellationToken);

        return count > 0;
    }

    public Task<IReadOnlyList<AnalyticsReportClientScopeMapping>> GetMappingsAsync(
        int optionId,
        CancellationToken cancellationToken) =>
        queryExecutor.QueryAsync<AnalyticsReportClientScopeMapping>(
            AnalyticsReportClientScopeSql.GetMappings,
            new { OptionId = optionId },
            _commandTimeoutSeconds,
            cancellationToken);

    public Task<IReadOnlyList<int>> GetOptionIdsWithActiveScopeAsync(
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
            return Task.FromResult<IReadOnlyList<int>>(Array.Empty<int>());
        }

        return queryExecutor.QueryAsync<int>(
            AnalyticsReportClientScopeSql.GetOptionIdsWithActiveScope,
            new
            {
                OptionIdsJson = JsonSerializer.Serialize(normalizedOptionIds)
            },
            _commandTimeoutSeconds,
            cancellationToken);
    }

    public Task ReplaceForClientAsync(
        int optionId,
        int crmClientId,
        string reportClientValue,
        IReadOnlyCollection<int> groupIds,
        int updatedBy,
        CancellationToken cancellationToken)
    {
        var normalizedName = reportClientValue.Trim();
        var normalizedGroupIds = groupIds
            .Where(groupId => groupId > 0)
            .Distinct()
            .OrderBy(groupId => groupId)
            .ToArray();
        var commands = new[]
        {
            new AnalyticsDbCommand(
                AnalyticsReportClientScopeSql.ReplaceClientGroups,
                new
                {
                    OptionId = optionId,
                    CrmClientId = crmClientId,
                    ReportClientValue = normalizedName,
                    GroupIdsJson = JsonSerializer.Serialize(normalizedGroupIds),
                    UpdatedBy = updatedBy
                })
        };

        return queryExecutor.ExecuteTransactionAsync(
            commands,
            _commandTimeoutSeconds,
            cancellationToken);
    }
}
