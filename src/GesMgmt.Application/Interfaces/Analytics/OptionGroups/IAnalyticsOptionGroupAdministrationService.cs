using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.Interfaces.Analytics;

public interface IAnalyticsOptionGroupAdministrationService
{
    Task<AnalyticsOptionGroupsResponse?> GetAsync(
        int optionId,
        CancellationToken cancellationToken);

    Task<AnalyticsAdministrationCommandResult> UpdateAsync(
        int optionId,
        IReadOnlyCollection<int>? requestedGroupIds,
        int userId,
        CancellationToken cancellationToken);
}
