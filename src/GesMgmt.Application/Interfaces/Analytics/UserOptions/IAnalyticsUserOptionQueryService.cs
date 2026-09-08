using GesMgmt.Application.DTOs.Analytics;

namespace GesMgmt.Application.Interfaces.Analytics;

public interface IAnalyticsUserOptionQueryService
{
    Task<IReadOnlyList<AnalyticsUserOptionResponse>> GetAsync(
        int userId,
        CancellationToken cancellationToken);
}
