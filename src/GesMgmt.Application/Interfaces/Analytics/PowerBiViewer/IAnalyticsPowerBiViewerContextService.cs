using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.Interfaces.Analytics;

public interface IAnalyticsPowerBiViewerContextService
{
    Task<AnalyticsPowerBiViewerContext> ResolveAsync(
        int userId,
        int optionId,
        AnalyticsPowerBiViewerSelection? selection,
        CancellationToken cancellationToken);
}
