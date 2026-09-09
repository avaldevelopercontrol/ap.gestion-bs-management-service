using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;

namespace GesMgmt.Infraestructure.Repositories.Analytics;

public static class SisgesUserClientSql
{
    public const string GetActiveClients = """
SELECT DISTINCT g.nid_cliente
FROM dbo.av_Usuario u
INNER JOIN dbo.av_UGrupo ug ON ug.nId_Usuario = u.nId_Usuario
INNER JOIN dbo.av_Grupo g ON g.nId_Grupo = ug.nId_Grupo
WHERE u.nId_Usuario = @UserId
AND u.bEstado = 1
AND ug.bEstado = 1
AND ug.bActivo = 1
AND g.bEstado = 1
AND g.nid_cliente IS NOT NULL
AND g.nid_cliente > 0
ORDER BY g.nid_cliente;
""";

    public const string IsActiveClient = """
SELECT TOP (1) 1
FROM dbo.av_Usuario u
INNER JOIN dbo.av_UGrupo ug ON ug.nId_Usuario = u.nId_Usuario
INNER JOIN dbo.av_Grupo g ON g.nId_Grupo = ug.nId_Grupo
WHERE u.nId_Usuario = @UserId
AND g.nid_cliente = @ClientId
AND u.bEstado = 1
AND ug.bEstado = 1
AND ug.bActivo = 1
AND g.bEstado = 1;
""";
}
