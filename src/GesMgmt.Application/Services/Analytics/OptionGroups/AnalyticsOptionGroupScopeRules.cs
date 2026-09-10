using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Application.Validators.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;

namespace GesMgmt.Application.Services.Analytics;

public static class AnalyticsOptionGroupScopeRules
{
    public static bool RequiresExactlyOneGroup(int optionId) =>
        optionId == AnalyticsOptionIds.GestionIntegralCobranza;
}
