
using GesMgmt.Domain.Entities;

namespace GesMgmt.Application.DTOs.UsuarioGrupoOpcion
{
    public class UsuarioGrupoOpcionRequestDto
    {
        public class GetUsuarioGrupoOpcionListadoResponseDto
        {
            public int nId_UsuarioGrupoOpcion { get; set; }
            public int nId_Usuario { get; set; }
            public string cUsr_NroDoc { get; set; }
            public string? cUsr_ApePat { get; set; }
            public string? cUsr_ApeMat { get; set; }
            public string? cUsr_Nombres { get; set; }
            public string cUsr_Login { get; set; }
            public int nId_Grupo { get; set; }
            public string? cNombre_Grupo { get; set; }
            public int nId_Opcion { get; set; }
            public string sCodigoOpcion { get; set; }
            public string sNombreOpcion { get; set; }
            public bool? bConsultar { get; set; }
            public bool? bInsertar { get; set; }
            public bool? bEditar { get; set; }
            public bool? bEliminar { get; set; }
            public bool? bExportar { get; set; }
            public bool bEstado { get; set; }
            public int nCrea { get; set; }
            public string dFechaCrea { get; set; }
            public int? nModifica { get; set; }
            public string? dFechaModifica { get; set; }
        }

        public class GetUsuarioGrupoOpcionObtenerResponseDto
        {
            public int nId_UsuarioGrupoOpcion { get; set; }
            public int nId_Usuario { get; set; }
            public int nId_Grupo { get; set; }
            public int nId_Opcion { get; set; }
            public bool? bConsultar { get; set; }
            public bool? bInsertar { get; set; }
            public bool? bEditar { get; set; }
            public bool? bEliminar { get; set; }
            public bool? bExportar { get; set; }
            public bool bEstado { get; set; }
            public int nCrea { get; set; }
            public string dFechaCrea { get; set; }
            public int? nModifica { get; set; }
            public string? dFechaModifica { get; set; }
        }

        public class PostUsuarioGrupoOpcionCrearRequestDto
        {
            public int nId_Usuario { get; set; }
            public int nId_Grupo { get; set; }
            public int nId_Opcion { get; set; }
            public bool? bConsultar { get; set; }
            public bool? bInsertar { get; set; }
            public bool? bEditar { get; set; }
            public bool? bEliminar { get; set; }
            public bool? bExportar { get; set; }
            public bool bEstado { get; set; }
            public int nCrea { get; set; }
            public DateTime dFechaCrea { get; set; }
            public int? nModifica { get; set; }
            public DateTime? dFechaModifica { get; set; }
        }

        public class PutUsuarioGrupoOpcionEditarRequestDto
        {
            public int nId_UsuarioGrupoOpcion { get; set; }
            public int nId_Usuario { get; set; }
            public int nId_Grupo { get; set; }
            public int nId_Opcion { get; set; }
            public bool? bConsultar { get; set; }
            public bool? bInsertar { get; set; }
            public bool? bEditar { get; set; }
            public bool? bEliminar { get; set; }
            public bool? bExportar { get; set; }
            public bool bEstado { get; set; }
            public int nModifica { get; set; }
            public DateTime dFechaModifica { get; set; }
        }
    }
}