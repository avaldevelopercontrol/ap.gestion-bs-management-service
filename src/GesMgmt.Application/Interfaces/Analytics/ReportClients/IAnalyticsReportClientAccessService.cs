using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.Interfaces.Analytics;

public interface IAnalyticsReportClientAccessService
{
    Task<AnalyticsReportClientAccessResult> ResolveAsync(
        int userId,
        int optionId,
        CancellationToken cancellationToken);
}

public sealed record AnalyticsReportClientAccessResult(
    bool HasOptionAccess,
    IReadOnlyList<AnalyticsReportClientOption> Clients);
