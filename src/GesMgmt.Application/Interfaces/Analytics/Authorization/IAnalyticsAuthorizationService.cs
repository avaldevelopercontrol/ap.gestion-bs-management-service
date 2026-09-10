using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Domain.Constants;

namespace GesMgmt.Application.Interfaces.Analytics;

public interface IAnalyticsAuthorizationService
{
    Task<AnalyticsAuthorizationResult> CanAccessAdministrationAsync(
        int userId,
        int? groupId,
        SisgesOptionPermission permission,
        CancellationToken cancellationToken);
}
