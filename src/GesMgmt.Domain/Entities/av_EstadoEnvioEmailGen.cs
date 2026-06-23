using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Entities
{
    public class av_EstadoEnvioEmailGen
    {
        public int nId_EstadoEnvioEmailGen { get; set; }
        public string? cEstadoGenOriginal { get; set; }
        public string? cEstadoGenTraducido { get; set; }
        public int? nPeso { get; set; }
        public bool? bEstado { get; set; }
    }
}