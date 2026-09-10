using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.Interfaces.Analytics;

public interface IAnalyticsUserOptionAdministrationService
{
    Task<AnalyticsOptionUsersResponse?> GetAsync(
        int optionId,
        CancellationToken cancellationToken);

    Task<AnalyticsAdministrationCommandResult> UpdateAsync(
        int optionId,
        IReadOnlyCollection<int>? requestedUserIds,
        int adminUserId,
        CancellationToken cancellationToken);
}
