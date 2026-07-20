using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Entities
{
    public class av_ConfigSistema
    {
        public int nCodTabla { get; set; }
        public string cNomTabla { get; set; }
        public string cLlave { get; set; }
        public string cValor { get; set; }
        public bool bEstado { get; set; }
        public string? cDescripcion { get; set; }
        public string? cComentario { get; set; }
    }
}