using GesMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Application.DTOs.PerfilOpcion
{
    public class PerfilOpcionRequestDto
    {
        public class EditPerfilOpcionRequestDto
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
            public int nModifica { get; set; }
            public DateTime dFechaModifica { get; set; }
        }

        public class CreatePerfilOpcionRequestDto
        {
            public int nId_Perfil { get; set; }
            public int nId_Opcion { get; set; }
            public bool bConsultar { get; set; }
            public bool bInsertar { get; set; }
            public bool bEditar { get; set; }
            public bool bEliminar { get; set; }
            public bool bExportar { get; set; }
            public bool bEstado { get; set; }
            public int nCrea { get; set; }
            public DateTime dFechaCrea { get; set; }
        }
    }
}