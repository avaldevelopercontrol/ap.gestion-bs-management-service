using Microsoft.Extensions.Options;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence.Analytics;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

internal sealed class AnalyticsReportClientCatalogRepository(
    IAnalyticsQueryExecutor queryExecutor,
    AnalyticsDatabaseOptions databaseOptions,
    IAnalyticsAccessCache cache)
    : IAnalyticsReportClientCatalogRepository
{
    private readonly int _commandTimeoutSeconds =
        databaseOptions.CommandTimeoutSeconds;

    public bool Supports(int optionId) =>
        optionId == AnalyticsOptionIds.GestionIntegralCobranza;

    public Task<IReadOnlyList<AnalyticsReportClientCatalogItem>> GetCurrentAsync(
        int optionId,
        CancellationToken cancellationToken)
    {
        if (!Supports(optionId))
        {
            return Task.FromResult<IReadOnlyList<AnalyticsReportClientCatalogItem>>(
                Array.Empty<AnalyticsReportClientCatalogItem>());
        }

        return cache.GetOrCreateAsync(
            AnalyticsAccessCacheKeys.ReportClientCatalog(optionId),
            AnalyticsAccessCachePolicy.CatalogDuration,
            token => queryExecutor.QueryAsync<AnalyticsReportClientCatalogItem>(
                AnalyticsReportClientCatalogSql.Current,
                new { OptionId = optionId },
                _commandTimeoutSeconds,
                token),
            cancellationToken);
    }
}
