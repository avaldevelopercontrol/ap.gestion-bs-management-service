using Microsoft.Extensions.Options;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence.Analytics;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

internal sealed class AnalyticsOptionConfigRepository(
    IAnalyticsQueryExecutor queryExecutor,
    AnalyticsDatabaseOptions databaseOptions,
    IAnalyticsAccessCache cache)
    : IAnalyticsOptionConfigRepository
{
    private readonly int _commandTimeoutSeconds =
        databaseOptions.CommandTimeoutSeconds;

    public async Task<bool> ExistsAsync(
        int optionId,
        CancellationToken cancellationToken)
    {
        if (optionId <= 0)
        {
            return false;
        }

        var exists = await queryExecutor.QuerySingleAsync<int>(
            AnalyticsOptionConfigSql.Exists,
            new { OptionId = optionId },
            _commandTimeoutSeconds,
            cancellationToken);

        return exists == 1;
    }

    public async Task<bool> IsActiveAsync(
        int optionId,
        CancellationToken cancellationToken)
    {
        if (optionId <= 0)
        {
            return false;
        }

        var options = await GetActiveOptionsAsync(cancellationToken);
        return options.Any(option => option.OptionId == optionId);
    }

    public Task<IReadOnlyList<AnalyticsOptionConfig>> GetAllAsync(
        CancellationToken cancellationToken) =>
        GetActiveOptionsAsync(cancellationToken);

    public async Task UpsertAsync(
        int optionId,
        string optionCode,
        string optionName,
        bool isActive,
        int? userId,
        CancellationToken cancellationToken)
    {
        var parameters = new
        {
            OptionId = optionId,
            OptionCode = optionCode,
            OptionName = optionName,
            IsActive = isActive,
            UserId = userId
        };

        IReadOnlyCollection<AnalyticsDbCommand> commands =
        [
            new AnalyticsDbCommand(
                AnalyticsOptionConfigSql.Update,
                parameters),
            new AnalyticsDbCommand(
                AnalyticsOptionConfigSql.InsertMissing,
                parameters)
        ];

        await queryExecutor.ExecuteTransactionAsync(
            commands,
            _commandTimeoutSeconds,
            cancellationToken);

        cache.Remove(AnalyticsAccessCacheKeys.ActiveOptions);
    }

    private Task<IReadOnlyList<AnalyticsOptionConfig>> GetActiveOptionsAsync(
        CancellationToken cancellationToken) =>
        cache.GetOrCreateAsync(
            AnalyticsAccessCacheKeys.ActiveOptions,
            AnalyticsAccessCachePolicy.ConfigurationDuration,
            token => queryExecutor.QueryAsync<AnalyticsOptionConfig>(
                AnalyticsOptionConfigSql.GetAll,
                null,
                _commandTimeoutSeconds,
                token),
            cancellationToken);
}
