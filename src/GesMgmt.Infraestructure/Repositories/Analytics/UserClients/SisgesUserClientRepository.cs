using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

internal sealed class SisgesUserClientRepository(AvalDbContext context)
    : ISisgesUserClientRepository
{
    public async Task<IReadOnlyList<int>> GetActiveClientIdsAsync(
        int userId,
        CancellationToken cancellationToken)
    {
        if (userId <= 0)
        {
            return Array.Empty<int>();
        }

        return await context.av_UGrupos
            .AsNoTracking()
            .Where(userGroup =>
                userGroup.nId_Usuario == userId &&
                userGroup.bEstado == true &&
                userGroup.bActivo == true &&
                userGroup.av_Usuario.bEstado &&
                userGroup.av_Grupo.bEstado == true &&
                userGroup.av_Grupo.nid_cliente.HasValue &&
                userGroup.av_Grupo.nid_cliente.Value > 0)
            .Select(userGroup => userGroup.av_Grupo.nid_cliente!.Value)
            .Distinct()
            .OrderBy(clientId => clientId)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> IsActiveClientAsync(
        int userId,
        int clientId,
        CancellationToken cancellationToken)
    {
        if (userId <= 0 || clientId <= 0)
        {
            return Task.FromResult(false);
        }

        return context.av_UGrupos
            .AsNoTracking()
            .AnyAsync(
                userGroup =>
                    userGroup.nId_Usuario == userId &&
                    userGroup.bEstado == true &&
                    userGroup.bActivo == true &&
                    userGroup.av_Usuario.bEstado &&
                    userGroup.av_Grupo.bEstado == true &&
                    userGroup.av_Grupo.nid_cliente == clientId,
                cancellationToken);
    }
}
