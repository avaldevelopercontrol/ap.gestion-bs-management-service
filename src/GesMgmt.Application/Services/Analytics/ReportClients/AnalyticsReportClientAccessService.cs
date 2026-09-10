using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Application.Validators.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;

namespace GesMgmt.Application.Services.Analytics;

public sealed class AnalyticsReportClientAccessService(
    IAnalyticsOptionAccessService optionAccessService,
    IAnalyticsReportClientConfigurationService configurationService)
    : IAnalyticsReportClientAccessService
{
    public async Task<AnalyticsReportClientAccessResult> ResolveAsync(
        int userId,
        int optionId,
        CancellationToken cancellationToken)
    {
        if (userId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(userId));
        }

        if (optionId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(optionId));
        }

        var optionAccess = await optionAccessService.ResolveAsync(
            userId,
            optionId,
            cancellationToken);

        if (!optionAccess.Allowed)
        {
            return new AnalyticsReportClientAccessResult(
                false,
                Array.Empty<AnalyticsReportClientOption>());
        }

        var activeUserGroupSet = optionAccess.ActiveUserGroupIds.ToHashSet();
        var configurations = await configurationService.ResolveAsync(
            optionId,
            cancellationToken);

        var clients = configurations
            .Where(configuration =>
                AnalyticsReportClientAuthorization.IsAuthorized(
                    configuration,
                    activeUserGroupSet))
            .Select(configuration => new AnalyticsReportClientOption(
                configuration.ClientId,
                configuration.Name))
            .OrderBy(client => client.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(client => client.ClientId)
            .ToArray();

        return new AnalyticsReportClientAccessResult(true, clients);
    }
}
