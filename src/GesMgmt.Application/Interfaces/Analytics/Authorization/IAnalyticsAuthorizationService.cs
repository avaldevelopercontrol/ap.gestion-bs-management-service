using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.Interfaces.Analytics;

public interface IAnalyticsAuthorizationService
{
    Task<AnalyticsAuthorizationResult> CanManageAsync(
        int userId,
        CancellationToken cancellationToken);
}
