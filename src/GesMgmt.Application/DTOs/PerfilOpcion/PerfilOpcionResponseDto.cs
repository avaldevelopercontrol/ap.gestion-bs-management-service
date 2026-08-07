using GesMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Application.DTOs.PerfilOpcion
{
    public class PerfilOpcionResponseDto
    {
        public class CreatePerfilOpcionResponseDto
        {
            public int nId_Perfil { get; set; }
            public int nId_Opcion { get; set; }
        }

        public class EditPerfilOpcionResponseDto
        {
            public int nId_PerfilOpcion { get; set; }
            public int nId_Perfil { get; set; }
            public int nId_Opcion { get; set; }
        }

        public class GetPerfilOpcionResponseDto
        {
            public int nId_Perfil { get; set; }
            public string per_Nombre { get; set; }
            public int nCantidadOpciones { get; set; }
        }

        public class GetOpcionesPorPerfilResponseDto
        {
            public int nId_PerfilOpcion { get; set; }
            public int nId_Perfil { get; set; }
            public int nId_Opcion { get; set; }
            public bool bConsultar { get; set; }
            public bool bInsertar { get; set; }
            public bool bEditar { get; set; }
            public bool bEliminar { get; set; }
            public bool bExportar { get; set; }
            public bool bEstado { get; set; }
            public string sEstado { get; set; }
            public int nCrea { get; set; }
            public string dFechaCrea { get; set; }
            public int? nModifica { get; set; }
            public string? dFechaModifica { get; set; }
        }
    }
}