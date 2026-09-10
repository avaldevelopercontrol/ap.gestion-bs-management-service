using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Entities
{
    public class av_ContFormTipoCrud
    {
        public int nTipoFormCrud { get; set; }
        public string cNombreTipoForm { get; set; }
        public bool? bListaPorDeudor { get; set; }
        public bool? bListaPorDocumento { get; set; }
        public bool? bAdjuntaArchivo { get; set; }
        public string? bTipoAccion { get; set; }
        public bool? bEliminaDocxCobrar { get; set; }
        public string? cDescripcion { get; set; }
    }
}