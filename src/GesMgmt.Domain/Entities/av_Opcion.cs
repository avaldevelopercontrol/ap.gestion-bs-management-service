using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Entities
{
    public class av_Opcion
    {
        public int nId_Opcion { get; set; }
        public string sCodigoOpcion { get; set; }
        public string sNombreOpcion { get; set; }
        public string sDescripcionOpcion { get; set; }
        public string sUrlOpcion { get; set; }
        public string? sIcono { get; set; }
        public int nTipo { get; set; }
        public int? nId_OpcionPadre { get; set; }
        public int nOrden { get; set; }
        public bool bVisible { get; set; }
        public bool bEstado { get; set; }
        public int nCrea { get; set; }
        public DateTime dFechaCrea { get; set; }
        public int? nModifica { get; set; }
        public DateTime? dFechaModifica { get; set; }
    }
}