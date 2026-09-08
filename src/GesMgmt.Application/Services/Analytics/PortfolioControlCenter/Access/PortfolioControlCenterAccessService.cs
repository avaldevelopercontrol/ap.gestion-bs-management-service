using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Application.Utils.Analytics.PortfolioControlCenter;

namespace GesMgmt.Application.Services.Analytics.PortfolioControlCenter;

public sealed class PortfolioControlCenterAccessService(
    IAnalyticsAccessService analyticsAccess,
    IAnalyticsUserContext userContext)
    : IPortfolioControlCenterAccessService
{
    public async Task<PortfolioControlCenterClientAccess> ResolveClientAsync(
        int? requestedCrmClientId,
        CancellationToken cancellationToken)
    {
        using var phase = PortfolioControlCenterDiagnostics.BeginPhase(
            PortfolioControlCenterDiagnostics.AccessPhase);

        if (requestedCrmClientId is <= 0)
        {
            return PortfolioControlCenterClientAccess.Error(
                400,
                "Cartera Analytics inválida",
                "crmClientId debe ser un entero positivo.");
        }

        if (!userContext.TryGetUserId(out var userId))
        {
            return PortfolioControlCenterClientAccess.Error(
                401,
                "Identidad Analytics no disponible",
                "No se pudo determinar el usuario SISGES para validar el acceso a Analytics.");
        }

        if (requestedCrmClientId.HasValue)
        {
            var isAllowed = await analyticsAccess.IsClientAllowedAsync(
                userId,
                AnalyticsOptionIds.PortfolioControlCenter,
                requestedCrmClientId.Value,
                cancellationToken);

            if (!isAllowed)
            {
                return PortfolioControlCenterClientAccess.Error(
                    403,
                    "Cartera Analytics no autorizada",
                    $"El usuario no tiene acceso a la cartera CRM '{requestedCrmClientId.Value}'.");
            }

            return PortfolioControlCenterClientAccess.Allowed(
                requestedCrmClientId.Value);
        }

        var allowedClientIds = await analyticsAccess.GetAllowedClientIdsAsync(
            userId,
            AnalyticsOptionIds.PortfolioControlCenter,
            cancellationToken);

        if (allowedClientIds.Count == 0)
        {
            return PortfolioControlCenterClientAccess.Error(
                403,
                "Sin acceso a Portfolio Control Center",
                "El usuario no tiene carteras autorizadas para Portfolio Control Center.");
        }

        return PortfolioControlCenterClientAccess.Allowed(
            allowedClientIds.OrderBy(x => x).First());
    }
}
