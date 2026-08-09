
namespace GesMgmt.Application.DTOs.UGrupo
{
    public class UGrupoResponseDto
    {
        public class GetUsuarioGrupoListadoResponseDto
        {
            public int nId_UGrupo { get; set; }
            public int? nId_Usuario { get; set; }
            public string cUsr_Login { get; set; }
            public string? cUsr_ApePat { get; set; }
            public string? cUsr_ApeMat { get; set; }
            public string? cUsr_Nombres { get; set; }
            public int? nId_Grupo { get; set; }
            public string? cNombre_Grupo { get; set; }
            public string? dUGrupo_FecIni { get; set; }
            public string? dUGrupo_FecFin { get; set; }
            public bool? bEstado { get; set; }
            public bool? bActivo { get; set; }
            public bool? bGestion { get; set; }
        }

        public class GetUsuarioGrupoObtenerResponseDto
        {
            public int nId_UGrupo { get; set; }
            public int? nId_Usuario { get; set; }
            public int? nId_Grupo { get; set; }
            public string? dUGrupo_FecIni { get; set; }
            public string? dUGrupo_FecFin { get; set; }
            public bool? bEstado { get; set; }
            public bool? bActivo { get; set; }
            public bool? bGestion { get; set; }
        }

        public class PostUsuarioGrupoCrearResponseDto
        {
            public int nId_UGrupo { get; set; }
            public int? nId_Usuario { get; set; }
            public int? nId_Grupo { get; set; }
        }

        public class PutUsuarioGrupoModificarResponseDto
        {
            public int nId_UGrupo { get; set; }
            public int? nId_Usuario { get; set; }
            public int? nId_Grupo { get; set; }
        }

        public class GetGruposByUsuarioResponseDto
        {
            public int? nId_Usuario { get; set; }
            public int nid_grupo { get; set; }
            public string cNombre_Grupo { get; set; }
        }

        public class GetUsuariosGrupoResponseDto
        {
            public int id { get; set; }
            public string nombre { get; set; }
            public string? perfil { get; set; }
            public string login { get; set; }
            public string? subZona { get; set; }
            public string? codRecaudacion { get; set; }
        }
    }
}