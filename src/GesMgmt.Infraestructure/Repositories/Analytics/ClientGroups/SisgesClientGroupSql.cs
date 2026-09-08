using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;
using GesMgmt.Infraestructure.Persistence.Analytics;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

public static class SisgesClientGroupSql
{
    public const string GetAllActiveGroups = """
        SELECT DISTINCT
            g.nId_Grupo AS GroupId,
            g.nid_cliente AS ClientId,
            LTRIM(RTRIM(COALESCE(g.cNombre_Grupo, ''))) AS GroupName
        FROM dbo.av_Grupo AS g
        WHERE g.bEstado = 1
          AND g.nid_cliente IS NOT NULL
          AND g.nid_cliente > 0
        ORDER BY
            GroupName,
            g.nId_Grupo;
        """;

    public const string GetActiveGroups = """
        SELECT DISTINCT
            g.nId_Grupo AS GroupId,
            g.nid_cliente AS ClientId,
            LTRIM(RTRIM(COALESCE(g.cNombre_Grupo, ''))) AS GroupName
        FROM dbo.av_Grupo AS g
        WHERE g.bEstado = 1
          AND g.nid_cliente IN @ClientIds
          AND g.nid_cliente IS NOT NULL
          AND g.nid_cliente > 0
        ORDER BY
            g.nid_cliente,
            g.nId_Grupo;
        """;
}
