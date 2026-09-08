using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.Interfaces.Analytics;

public interface IAnalyticsOptionClientAdministrationService
{
    Task<AnalyticsOptionClientsResponse?> GetAsync(
        int optionId,
        CancellationToken cancellationToken);

    Task<AnalyticsAdministrationCommandResult> UpdateAsync(
        int optionId,
        IReadOnlyCollection<int>? requestedClientIds,
        int userId,
        CancellationToken cancellationToken);
}
