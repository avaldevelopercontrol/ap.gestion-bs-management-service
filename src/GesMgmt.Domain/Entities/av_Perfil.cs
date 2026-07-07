using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Entities
{
    public class av_Perfil
    {
        public int nid_perfil { get; set; }
        public DateTime? per_Fecha { get; set; }
        public string per_Nombre { get; set; }
        public int? nper_EliminaRegJud { get; set; }
        public int? nper_AvisoVencidoJud { get; set; }
        public int? nper_RegistraRegJud { get; set; }
        public int? nper_MantUsuario { get; set; }
        public string? per_abreviatura { get; set; }
        public int? nEquiv_rrhh { get; set; }
        public int? nEstadoGest { get; set; }
        public bool? bProduccionOnline { get; set; }
        public int? nId_TipoGestion { get; set; }
        public bool? bvisualiza_deudorhistoria { get; set; }
    }
}