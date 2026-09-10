using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.Utils.Analytics;

public static class AnalyticsAccessCacheKeys
{
    public const string ActiveOptions =
        "analytics-access:options:active";

    public static string ReportClientCatalog(int optionId) =>
        $"analytics-access:report-client-catalog:{optionId}";

    public static string ReportClientPublications(int optionId) =>
        $"analytics-access:report-client-publications:{optionId}";
}
