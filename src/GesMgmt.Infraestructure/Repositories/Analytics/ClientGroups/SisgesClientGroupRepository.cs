using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

internal sealed class SisgesClientGroupRepository(AvalDbContext context)
    : ISisgesClientGroupRepository
{
    public async Task<IReadOnlyList<SisgesClientGroup>> GetAllActiveGroupsAsync(
        CancellationToken cancellationToken) =>
        await context.av_Grupos
            .AsNoTracking()
            .Where(group =>
                group.bEstado == true &&
                group.nid_cliente.HasValue &&
                group.nid_cliente.Value > 0)
            .Select(group => new SisgesClientGroup
            {
                GroupId = group.nId_Grupo,
                ClientId = group.nid_cliente!.Value,
                GroupName = (group.cNombre_Grupo ?? string.Empty).Trim()
            })
            .OrderBy(group => group.GroupName)
            .ThenBy(group => group.GroupId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<SisgesClientGroup>> GetActiveGroupsAsync(
        IReadOnlyCollection<int> clientIds,
        CancellationToken cancellationToken)
    {
        var normalizedClientIds = clientIds
            .Where(clientId => clientId > 0)
            .Distinct()
            .OrderBy(clientId => clientId)
            .ToArray();

        if (normalizedClientIds.Length == 0)
        {
            return Array.Empty<SisgesClientGroup>();
        }

        return await context.av_Grupos
            .AsNoTracking()
            .Where(group =>
                group.bEstado == true &&
                group.nid_cliente.HasValue &&
                group.nid_cliente.Value > 0 &&
                normalizedClientIds.Contains(group.nid_cliente.Value))
            .OrderBy(group => group.nid_cliente)
            .ThenBy(group => group.nId_Grupo)
            .Select(group => new SisgesClientGroup
            {
                GroupId = group.nId_Grupo,
                ClientId = group.nid_cliente!.Value,
                GroupName = (group.cNombre_Grupo ?? string.Empty).Trim()
            })
            .ToListAsync(cancellationToken);
    }
}
