using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Entities
{
    public class av_DetCobZonaGeneral
    {
        public int nId_Cobertura { get; set; }
        public av_CobZonaGeneral av_CobZonaGeneral { get; set; }
        public int nId_Ubigeo { get; set; }
        public av_Ubigeo av_Ubigeo { get; set; }
        public int nId_Cliente { get; set; }
        public av_Cliente av_Cliente { get; set; }
        public int nId_DetCobZonaGen { get; set; }
    }
}