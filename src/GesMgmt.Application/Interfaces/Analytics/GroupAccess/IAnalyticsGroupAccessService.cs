using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.Interfaces.Analytics;

public interface IAnalyticsGroupAccessService
{
    Task<IReadOnlyList<int>> GetGroupScopedGroupIdsAsync(
        int userId,
        int optionId,
        CancellationToken cancellationToken);
}
