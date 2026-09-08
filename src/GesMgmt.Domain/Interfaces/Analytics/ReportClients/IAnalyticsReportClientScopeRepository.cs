using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Domain.Interfaces.Analytics;

public interface IAnalyticsReportClientScopeRepository
{
    Task<bool> HasAnyScopeAsync(
        int optionId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<AnalyticsReportClientScopeMapping>> GetMappingsAsync(
        int optionId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<int>> GetOptionIdsWithActiveScopeAsync(
        IReadOnlyCollection<int> optionIds,
        CancellationToken cancellationToken);

    Task ReplaceForClientAsync(
        int optionId,
        int crmClientId,
        string reportClientValue,
        IReadOnlyCollection<int> groupIds,
        int updatedBy,
        CancellationToken cancellationToken);

}
