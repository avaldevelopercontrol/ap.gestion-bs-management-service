using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Entities
{
    public class av_OpeCodIn
    {
        public int nId_OpeCodIn { get; set; }
        public string? cNombre_OpeCodIn { get; set; }
        public string? cSigla_OpeCodIn { get; set; }
        public string? cDesc_OpeCodIn { get; set; }
        public bool? bEstado { get; set; }
        public int? nId_OpeTipo { get; set; }
        public av_OpeTipo av_OpeTipo { get; set; }
    }
}