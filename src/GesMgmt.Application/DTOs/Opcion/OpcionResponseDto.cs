using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Application.DTOs.Opcion
{
    public class OpcionResponseDto
    {
        public class GetOpcionesResponseDto
        {
            public int nId_Opcion { get; set; }
            public string sNombreOpcion { get; set; }
            public string sUrlOpcion { get; set; }
            public string? sIcono { get; set; }
            public int nTipo { get; set; }
            public int? nId_OpcionPadre { get; set; }
            public int nOrden { get; set; }
            public bool bVisible { get; set; }
            public bool bEstado { get; set; }
        }
    }
}