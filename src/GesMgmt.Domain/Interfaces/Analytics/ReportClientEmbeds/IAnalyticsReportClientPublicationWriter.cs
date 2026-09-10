using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Domain.Interfaces.Analytics;

public interface IAnalyticsReportClientPublicationWriter
{
    Task PatchAsync(
        int optionId,
        IReadOnlyCollection<AnalyticsReportClientPublicationUpdate> publications,
        int updatedBy,
        CancellationToken cancellationToken);
}
