using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Entities
{
    public class av_Grupo
    {
        public int nId_Grupo { get; set; }
        public string? cNombre_Grupo { get; set; }
        public string? cSigla_Grupo { get; set; }
        public bool? bEstado { get; set; }
        public int? nCant_Grupo { get; set; }
        public int? nid_cliente { get; set; }
        public av_Cliente av_Cliente { get; set; }
    }
}