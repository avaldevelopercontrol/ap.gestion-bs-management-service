using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.Interfaces.Analytics;

public interface IAnalyticsPowerBiUserAccessService
{
    Task<IReadOnlyList<AnalyticsPowerBiOptionAccess>> ResolveAsync(
        int userId,
        IReadOnlyCollection<int> optionIds,
        CancellationToken cancellationToken);
}

public sealed record AnalyticsPowerBiOptionAccess(
    int OptionId,
    bool Allowed,
    bool RequiresClientSelection);
