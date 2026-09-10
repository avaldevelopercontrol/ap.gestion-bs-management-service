using GesMgmt.Application.DTOs.Analytics;

namespace GesMgmt.Application.Interfaces.Analytics;

public interface IAnalyticsOptionService
{
    Task<IReadOnlyList<AnalyticsOptionConfigResponse>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<bool> ExistsAsync(
        int optionId,
        CancellationToken cancellationToken);

    Task<bool> IsActiveAsync(
        int optionId,
        CancellationToken cancellationToken);

    Task UpsertAsync(
        int optionId,
        string optionCode,
        string optionName,
        bool isActive,
        int? userId,
        CancellationToken cancellationToken);
}
