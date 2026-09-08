using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Domain.Interfaces.Analytics;

public interface IAnalyticsOptionConfigRepository
{
    Task<bool> ExistsAsync(
        int optionId,
        CancellationToken cancellationToken);

    Task<bool> IsActiveAsync(
        int optionId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<AnalyticsOptionConfig>> GetAllAsync(
        CancellationToken cancellationToken);

    Task UpsertAsync(
        int optionId,
        string optionCode,
        string optionName,
        bool isActive,
        int? userId,
        CancellationToken cancellationToken);
}
