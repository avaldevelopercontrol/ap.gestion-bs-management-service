using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Application.Validators.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;

namespace GesMgmt.Application.Services.Analytics;

public static class AnalyticsReportClientAuthorization
{
    public static bool IsAuthorized(
        AnalyticsReportClientConfiguration configuration,
        IReadOnlySet<int> activeUserGroupIds) =>
        configuration.IsReady &&
        configuration.GroupIds.Count > 0 &&
        configuration.GroupIds.All(activeUserGroupIds.Contains);

    public static bool Matches(
        AnalyticsReportClientConfiguration configuration,
        int clientId,
        string reportClient) =>
        configuration.ClientId == clientId &&
        string.Equals(
            configuration.Name.Trim(),
            reportClient.Trim(),
            StringComparison.OrdinalIgnoreCase);
}
