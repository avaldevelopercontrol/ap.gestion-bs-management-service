using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Application.DTOs.UsuarioGrupoOpcion
{
    public class UsuarioGrupoOpcionResponseDto
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

        public class GetByIdUsuarioIdGrupoAsyncResponseDto
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

        public class PostUsuarioGrupoOpcionCrearResponseDto
        {
            public int nId_UsuarioGrupoOpcion { get; set; }
            public int? nId_Usuario { get; set; }
            public int? nId_Grupo { get; set; }
            public int? nId_Opcion { get; set; }
        }

        public class PutUsuarioGrupoOpcionModificarResponseDto
        {
            public int nId_UsuarioGrupoOpcion { get; set; }
            public int? nId_Usuario { get; set; }
            public int? nId_Grupo { get; set; }
            public int? nId_Opcion { get; set; }
        }

        
    }
}