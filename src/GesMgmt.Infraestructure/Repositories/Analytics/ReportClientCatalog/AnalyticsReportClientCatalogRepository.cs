using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

internal sealed class AnalyticsReportClientCatalogRepository(
    AnalyticsDbContext context,
    IAnalyticsAccessCache cache)
    : IAnalyticsReportClientCatalogRepository
{
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

        return cache.GetOrCreateAsync<IReadOnlyList<AnalyticsReportClientCatalogItem>>(
            AnalyticsAccessCacheKeys.ReportClientCatalog(optionId),
            AnalyticsAccessCachePolicy.CatalogDuration,
            async token => await context.AnalyticsReportClientCatalog
                .AsNoTracking()
                .Where(catalog => catalog.OptionId == optionId)
                .OrderBy(catalog => catalog.ReportClientValue)
                .ThenBy(catalog => catalog.CrmClientId)
                .Select(catalog => new AnalyticsReportClientCatalogItem
                {
                    CrmClientId = catalog.CrmClientId,
                    ReportClientValue = catalog.ReportClientValue
                })
                .ToArrayAsync(token),
            cancellationToken);
    }
}
