using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Entities
{
    public class av_CampanaDiscador
    {
        public int id { get; set; }
        public int NroCampanaDiscador { get; set; }
        public string cNombreCampana { get; set; }
        public bool? bestado { get; set; }
        public DateTime? FecActualizacion { get; set; }
        public int nId_Cliente { get; set; }
        public av_Cliente av_Cliente { get; set; }
        public int nId_Discador { get; set; }
        public av_Discador av_Discador { get; set; }
    }
}