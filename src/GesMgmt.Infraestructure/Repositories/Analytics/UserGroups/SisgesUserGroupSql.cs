using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence.Analytics;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

public static class SisgesUserGroupSql
{
    public const string GetActiveGroups = """
        SELECT DISTINCT ug.nId_Grupo
        FROM dbo.av_UGrupo AS ug
        INNER JOIN dbo.av_Grupo AS g
            ON g.nId_Grupo = ug.nId_Grupo
        WHERE ug.nId_Usuario = @UserId
          AND ug.bEstado = 1
          AND ug.bActivo = 1
          AND g.bEstado = 1
        ORDER BY ug.nId_Grupo;
        """;
}
