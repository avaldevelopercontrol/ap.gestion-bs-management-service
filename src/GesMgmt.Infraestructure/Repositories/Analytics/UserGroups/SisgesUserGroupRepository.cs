using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

internal sealed class SisgesUserGroupRepository(AvalDbContext context)
    : ISisgesUserGroupRepository
{
    public async Task<IReadOnlyList<int>> GetActiveGroupIdsAsync(
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
                userGroup.av_Grupo.bEstado == true &&
                userGroup.nId_Grupo.HasValue)
            .Select(userGroup => userGroup.nId_Grupo!.Value)
            .Distinct()
            .OrderBy(groupId => groupId)
            .ToListAsync(cancellationToken);
    }
}
