using GesMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Application.DTOs.PerfilOpcion
{
    public class PerfilOpcionResponseDto
    {
        public class GetPerfilOpcionResponseDto
        {
            public int nId_Perfil { get; set; }
            public string per_Nombre { get; set; }
            public int nCantidadOpciones { get; set; }
        }
    }
}