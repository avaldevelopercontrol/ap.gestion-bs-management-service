using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Domain.Interfaces.Analytics;

public interface IAnalyticsPowerBiConfigurationWriter
{
    Task UpdateAsync(
        int optionId,
        string optionCode,
        string optionName,
        bool isActive,
        IReadOnlyCollection<int> previousGroupIds,
        IReadOnlyCollection<int> groupIds,
        IReadOnlyCollection<AnalyticsReportClientPublicationUpdate> publications,
        int updatedBy,
        CancellationToken cancellationToken);
}
