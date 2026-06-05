using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Entities
{
    public class av_DetallePersTelef
    {
        public int nId_DetallePersTelef { get; set; }
        public int nId_PersTelef { get; set; }
        public av_PersTelef av_PersTelef { get; set; }
        public int nId_Cliente { get; set; }
        public av_Cliente av_Cliente { get; set; }
        public DateTime? dFec_Registro { get; set; }
        public DateTime? dFec_Actualiza { get; set; }
        public int? nfuenteBusDet { get; set; }
        public int? nId_UsuReg { get; set; }
        public bool? bBase { get; set; }
        public bool? bestado { get; set; }
    }
}
