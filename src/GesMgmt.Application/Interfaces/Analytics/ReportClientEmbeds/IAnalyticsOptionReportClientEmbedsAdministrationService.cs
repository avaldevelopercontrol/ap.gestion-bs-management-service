using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.Interfaces.Analytics;

public interface IAnalyticsOptionReportClientEmbedsAdministrationService
{
    Task<AnalyticsOptionReportClientEmbedsResponse> GetAsync(
        int optionId,
        CancellationToken cancellationToken);

    Task<AnalyticsAdministrationCommandResult> UpdateAsync(
        int optionId,
        IReadOnlyList<UpdateAnalyticsOptionReportClientEmbed>? publications,
        int adminUserId,
        CancellationToken cancellationToken);
}
