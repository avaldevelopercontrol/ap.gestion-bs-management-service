using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.Interfaces.Analytics;

public interface IAnalyticsReportClientConfigurationService
{
    Task<bool> RequiresClientSelectionAsync(
        int optionId,
        CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<int, bool>> RequiresClientSelectionManyAsync(
        IReadOnlyCollection<int> optionIds,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<AnalyticsReportClientConfiguration>> ResolveAsync(
        int optionId,
        CancellationToken cancellationToken);
}
