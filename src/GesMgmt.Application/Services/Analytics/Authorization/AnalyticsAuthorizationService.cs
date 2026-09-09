using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces.Analytics;

namespace GesMgmt.Application.Services.Analytics;

public sealed class AnalyticsAuthorizationService(
    ISisgesOptionPermissionRepository permissionRepository)
    : IAnalyticsAuthorizationService
{
    public async Task<AnalyticsAuthorizationResult> CanAccessAdministrationAsync(
        int userId,
        int? groupId,
        SisgesOptionPermission permission,
        CancellationToken cancellationToken)
    {
        var allowed = await permissionRepository.HasPermissionAsync(
            userId,
            groupId,
            SisgesOptionCodes.MaintainModule,
            permission,
            cancellationToken);

        if (allowed)
        {
            return AnalyticsAuthorizationResult.Allow();
        }

        return AnalyticsAuthorizationResult.Deny(
            $"El usuario no tiene permiso {GetPermissionName(permission)} sobre Mantener módulo.");
    }

    private static string GetPermissionName(SisgesOptionPermission permission) =>
        permission switch
        {
            SisgesOptionPermission.Consult => "Consultar",
            SisgesOptionPermission.Insert => "Insertar",
            SisgesOptionPermission.Edit => "Editar",
            SisgesOptionPermission.Delete => "Eliminar",
            SisgesOptionPermission.Export => "Exportar",
            _ => "requerido"
        };
}
