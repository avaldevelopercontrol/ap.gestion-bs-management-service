using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence.Analytics;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

internal sealed class AnalyticsReportClientPublicationWriter(
    IAnalyticsQueryExecutor queryExecutor,
    AnalyticsDatabaseOptions databaseOptions,
    IAnalyticsAccessCache cache)
    : IAnalyticsReportClientPublicationWriter
{
    private readonly int _commandTimeoutSeconds =
        databaseOptions.CommandTimeoutSeconds;

    public async Task PatchAsync(
        int optionId,
        IReadOnlyCollection<AnalyticsReportClientPublicationUpdate> publications,
        int updatedBy,
        CancellationToken cancellationToken)
    {
        if (publications.Count == 0)
        {
            return;
        }

        await queryExecutor.ExecuteTransactionAsync(
            [
                AnalyticsReportClientPublicationPatch.CreateCommand(
                    optionId,
                    publications,
                    updatedBy)
            ],
            _commandTimeoutSeconds,
            cancellationToken);

        cache.Remove(AnalyticsAccessCacheKeys.ReportClientPublications(optionId));
    }
}
