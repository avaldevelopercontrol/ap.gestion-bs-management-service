using GesMgmt.Domain.Constants;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

internal sealed class SisgesOptionPermissionRepository(AvalDbContext context)
    : ISisgesOptionPermissionRepository
{
    public async Task<bool> HasPermissionAsync(
        int userId,
        int? groupId,
        string optionCode,
        SisgesOptionPermission permission,
        CancellationToken cancellationToken)
    {
        if (userId <= 0 ||
            string.IsNullOrWhiteSpace(optionCode) ||
            (groupId.HasValue && groupId.Value <= 0))
        {
            return false;
        }

        var user = await context.av_Usuarios
            .AsNoTracking()
            .Where(candidate =>
                candidate.nId_Usuario == userId &&
                candidate.bEstado &&
                candidate.nid_perfil.HasValue &&
                candidate.nid_perfil.Value > 0)
            .Select(candidate => new
            {
                ProfileId = candidate.nid_perfil!.Value
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return false;
        }

        var normalizedOptionCode = optionCode.Trim();
        var optionId = await context.av_Opcions
            .AsNoTracking()
            .Where(option =>
                option.sCodigoOpcion == normalizedOptionCode &&
                option.bEstado &&
                option.bVisible)
            .Select(option => (int?)option.nId_Opcion)
            .SingleOrDefaultAsync(cancellationToken);

        if (!optionId.HasValue)
        {
            return false;
        }

        if (groupId.HasValue)
        {
            var currentGroupId = groupId.Value;
            var hasActiveMembership = await context.av_UGrupos
                .AsNoTracking()
                .AnyAsync(userGroup =>
                    userGroup.nId_Usuario == userId &&
                    userGroup.nId_Grupo == currentGroupId &&
                    userGroup.bEstado == true &&
                    userGroup.bActivo == true &&
                    userGroup.av_Grupo.bEstado == true,
                    cancellationToken);

            if (!hasActiveMembership)
            {
                return false;
            }

            var userGroupPermissions = await context.av_UsuarioGrupoOpcions
                .AsNoTracking()
                .Where(assignment =>
                    assignment.nId_Usuario == userId &&
                    assignment.nId_Grupo == currentGroupId &&
                    assignment.nId_Opcion == optionId.Value &&
                    assignment.bEstado)
                .Select(assignment => new PermissionProjection(
                    assignment.bConsultar,
                    assignment.bInsertar,
                    assignment.bEditar,
                    assignment.bEliminar,
                    assignment.bExportar))
                .Take(2)
                .ToArrayAsync(cancellationToken);

            // SISGES trata un acceso especial activo como reemplazo completo del
            // permiso de perfil. Si hubiera más de uno, se deniega por inconsistencia.
            if (userGroupPermissions.Length > 1)
            {
                return false;
            }

            if (userGroupPermissions.Length == 1)
            {
                return HasPermission(userGroupPermissions[0], permission);
            }
        }

        var profilePermissions = await context.av_PerfilOpcions
            .AsNoTracking()
            .Where(assignment =>
                assignment.nId_Perfil == user.ProfileId &&
                assignment.nId_Opcion == optionId.Value &&
                assignment.bEstado)
            .Select(assignment => new PermissionProjection(
                assignment.bConsultar,
                assignment.bInsertar,
                assignment.bEditar,
                assignment.bEliminar,
                assignment.bExportar))
            .Take(2)
            .ToArrayAsync(cancellationToken);

        if (profilePermissions.Length != 1)
        {
            return false;
        }

        return HasPermission(profilePermissions[0], permission);
    }

    private static bool HasPermission(
        PermissionProjection permissions,
        SisgesOptionPermission permission) =>
        permission switch
        {
            SisgesOptionPermission.Consult => permissions.Consult == true,
            SisgesOptionPermission.Insert => permissions.Insert == true,
            SisgesOptionPermission.Edit => permissions.Edit == true,
            SisgesOptionPermission.Delete => permissions.Delete == true,
            SisgesOptionPermission.Export => permissions.Export == true,
            _ => false
        };

    private sealed record PermissionProjection(
        bool? Consult,
        bool? Insert,
        bool? Edit,
        bool? Delete,
        bool? Export);
}
