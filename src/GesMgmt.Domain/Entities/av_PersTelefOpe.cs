using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Entities
{
    public class av_PersTelefOpe
    {
        public int nId_PersTelefOpe { get; set; }
        public string? cNombre_PersTelefOpe { get; set; }
        public string? cSigla_PersTelefOpe { get; set; }
        public bool? bEstado { get; set; }
    }
}