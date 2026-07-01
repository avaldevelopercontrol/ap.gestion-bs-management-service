using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Entities
{
    public class av_MotivoNoPago
    {
        public int nId_MotivoNoPago { get; set; }
        public int nId_Cliente { get; set; }
        public string cNombreMotivoNoPago { get; set; }
        public string? cDescripcionMotivoNoPago { get; set; }
        public bool bEstado { get; set; }
        public string? cTipoEmpresa { get; set; }
    }
}