using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Entities
{
    public class av_OficinaAval
    {
        public int nid_OficinaAval { get; set; }
        public string? cNombre_Oficina { get; set; }
        public string? cDireccion { get; set; }
        public int? nId_Usuario { get; set; }
        public av_Usuario av_Usuario { get; set; }
        public bool? bLimaProv { get; set; }
        public int? nid_usuarioAsistente { get; set; }
        public int? nid_cliente { get; set; }
        public int? nId_ZonaGen { get; set; }
        public int? nId_subZonaGen { get; set; }
    }
}