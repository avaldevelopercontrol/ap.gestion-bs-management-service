using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.Interfaces.Analytics;

public interface IAnalyticsPowerBiConfigurationService
{
    Task<AnalyticsPowerBiConfigurationResponse> GetAsync(
        int optionId,
        CancellationToken cancellationToken);

    Task<AnalyticsAdministrationCommandResult> UpdateAsync(
        int optionId,
        UpdateAnalyticsPowerBiConfigurationRequest request,
        int userId,
        CancellationToken cancellationToken);
}
