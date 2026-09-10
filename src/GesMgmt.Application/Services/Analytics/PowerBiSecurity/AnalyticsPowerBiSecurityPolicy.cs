using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Application.Validators.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;

namespace GesMgmt.Application.Services.Analytics;

public sealed class AnalyticsPowerBiSecurityPolicy(
    AnalyticsPowerBiSecurityOptions options)
    : IAnalyticsPowerBiSecurityPolicy
{
    public bool AllowPublishToWeb => options.AllowPublishToWeb;
}
