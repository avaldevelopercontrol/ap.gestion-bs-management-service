using Microsoft.Extensions.Options;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence.Analytics;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

internal sealed class AnalyticsReportClientEmbedRepository(
    IAnalyticsQueryExecutor queryExecutor,
    AnalyticsDatabaseOptions databaseOptions,
    IAnalyticsAccessCache cache)
    : IAnalyticsReportClientEmbedRepository
{
    private readonly int _commandTimeoutSeconds =
        databaseOptions.CommandTimeoutSeconds;

    public async Task<AnalyticsReportClientEmbedMapping?> GetAsync(
        int optionId,
        int crmClientId,
        string reportClientValue,
        CancellationToken cancellationToken)
    {
        var normalizedName = reportClientValue.Trim();
        var publications = await GetActiveForOptionAsync(
            optionId,
            cancellationToken);

        var publication = publications.FirstOrDefault(item =>
            item.CrmClientId == crmClientId &&
            string.Equals(
                item.ReportClientValue,
                normalizedName,
                StringComparison.OrdinalIgnoreCase));

        return publication is null
            ? null
            : new AnalyticsReportClientEmbedMapping
            {
                ReportClientValue = publication.ReportClientValue,
                EmbedUrl = publication.EmbedUrl ?? string.Empty
            };
    }

    public Task<IReadOnlyList<AnalyticsReportClientEmbedAdminMapping>> GetActiveForOptionAsync(
        int optionId,
        CancellationToken cancellationToken) =>
        cache.GetOrCreateAsync(
            AnalyticsAccessCacheKeys.ReportClientPublications(optionId),
            AnalyticsAccessCachePolicy.ConfigurationDuration,
            token => queryExecutor.QueryAsync<AnalyticsReportClientEmbedAdminMapping>(
                AnalyticsReportClientEmbedSql.GetActiveForOption,
                new { OptionId = optionId },
                _commandTimeoutSeconds,
                token),
            cancellationToken);

    public async Task UpsertAsync(
        int optionId,
        int crmClientId,
        string reportClientValue,
        string embedUrl,
        int updatedBy,
        CancellationToken cancellationToken)
    {
        var parameters = new
        {
            OptionId = optionId,
            CrmClientId = crmClientId,
            ReportClientValue = reportClientValue.Trim(),
            EmbedUrl = embedUrl.Trim(),
            UpdatedBy = updatedBy
        };

        IReadOnlyCollection<AnalyticsDbCommand> commands =
        [
            new AnalyticsDbCommand(
                AnalyticsReportClientEmbedSql.UpdatePublication,
                parameters),
            new AnalyticsDbCommand(
                AnalyticsReportClientEmbedSql.InsertPublication,
                parameters)
        ];

        await queryExecutor.ExecuteTransactionAsync(
            commands,
            _commandTimeoutSeconds,
            cancellationToken);

        cache.Remove(AnalyticsAccessCacheKeys.ReportClientPublications(optionId));
    }

    public async Task DeactivateAsync(
        int optionId,
        int crmClientId,
        string reportClientValue,
        int updatedBy,
        CancellationToken cancellationToken)
    {
        await queryExecutor.ExecuteAsync(
            AnalyticsReportClientEmbedSql.DeactivatePublication,
            new
            {
                OptionId = optionId,
                CrmClientId = crmClientId,
                ReportClientValue = reportClientValue.Trim(),
                UpdatedBy = updatedBy
            },
            _commandTimeoutSeconds,
            cancellationToken);

        cache.Remove(AnalyticsAccessCacheKeys.ReportClientPublications(optionId));
    }
}
