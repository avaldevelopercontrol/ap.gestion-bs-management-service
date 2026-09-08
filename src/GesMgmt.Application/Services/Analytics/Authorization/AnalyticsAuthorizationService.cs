using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Application.Validators.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;

namespace GesMgmt.Application.Services.Analytics;

public sealed class AnalyticsAuthorizationService(
    AnalyticsAdministrationOptions options)
    : IAnalyticsAuthorizationService
{
    private readonly HashSet<int> _administratorUserIds = options
        .AdministratorUserIds
        .Where(userId => userId > 0)
        .ToHashSet();

    public Task<AnalyticsAuthorizationResult> CanManageAsync(
        int userId,
        CancellationToken cancellationToken)
    {
        var result = _administratorUserIds.Contains(userId)
            ? AnalyticsAuthorizationResult.Allow()
            : AnalyticsAuthorizationResult.Deny(
                "El usuario no tiene permisos administrativos de Analytics.");

        return Task.FromResult(result);
    }
}
