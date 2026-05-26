using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Entities
{
    public class av_Moneda //: BaseEntity
    {
        public int nId_Moneda { get; set; }
        public string? cNombre_Moneda { get; set; }
        public string? cSigla_Moneda { get; set; }
        public bool? bEstado { get; set; }
        public string? cAbreviado { get; set; }

        //Relaciones de navegación
        //public ICollection<av_DocxCobrar> av_DocxCobrars { get; set; }
    }
}