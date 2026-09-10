using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Domain.Interfaces.Analytics;

public interface IAnalyticsReportClientCatalogRepository
{
    bool Supports(int optionId);

    Task<IReadOnlyList<AnalyticsReportClientCatalogItem>> GetCurrentAsync(
        int optionId,
        CancellationToken cancellationToken);
}
