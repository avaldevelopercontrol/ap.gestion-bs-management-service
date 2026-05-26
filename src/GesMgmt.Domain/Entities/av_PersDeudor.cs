using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Entities
{
    public class av_PersDeudor //: BaseEntity
    {
        public int nId_PersDeudor { get; set; }
        public string? cPers_DNI { get; set; }
        public string? cPers_RUC { get; set; }
        public string? cPers_ApePat { get; set; }
        public string? cPers_ApeMat { get; set; }
        public string? cPers_Nombres { get; set; }
        public int? bSexo { get; set; }
        public int? nId_PersEstCivil { get; set; }
        public string? cPers_Coment { get; set; }
        public bool? bEstado { get; set; }
        public DateTime? dFecIngreso { get; set; }
        public DateTime? dFecUltMov { get; set; }
        public int? nId_PersDeudorAval { get; set; }
        public int? nPers_NroDoc { get; set; }
        public int? nPers_AtrazoMax { get; set; }
        public decimal? nPers_DeudaMax { get; set; }
        public int? nPers_PagoCompro { get; set; }
        public int? nPers_PagoRecupe { get; set; }
        public DateTime? dFecUltComproPago { get; set; }
        public DateTime? dFecUltPago { get; set; }
        public int? nId_PersDeudorGestionHrs { get; set; }
        public int? nId_Ubigeo { get; set; }
        public string? cNomCompleto { get; set; }
        public DateTime? dFecNacimiento { get; set; }
        public int? nGra_Instruccion { get; set; }
        public string? codigo { get; set; }
        public int? nid_cliente { get; set; }
        public string? cCorreo { get; set; }
        public int? nPers_TipJudicial { get; set; }
        public bool? bInfoAdicional { get; set; }
        public string? cPers_PTP { get; set; }
        public string? cPers_CE { get; set; }
        public string? cPers_Pasaporte { get; set; }

        //Relaciones de navegación
        //public ICollection<av_DocxCobrar> av_DocxCobrars { get; set; }
    }
}