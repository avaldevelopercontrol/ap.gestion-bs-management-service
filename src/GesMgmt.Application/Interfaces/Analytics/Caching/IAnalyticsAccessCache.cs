using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.Interfaces.Analytics;

public interface IAnalyticsAccessCache
{
    Task<T> GetOrCreateAsync<T>(
        string key,
        TimeSpan duration,
        Func<CancellationToken, Task<T>> factory,
        CancellationToken cancellationToken);

    void Remove(string key);
}
