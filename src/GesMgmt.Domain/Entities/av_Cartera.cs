using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Entities
{
    public class av_Cartera
    {
        public int nId_Cartera { get; set; }
        public int nId_Cliente { get; set; }
        public int nId_Contrato { get; set; }
        public ICollection<av_Contrato> av_Contratos { get; set; }
        public string? cCar_Nombre { get; set; }
        public DateTime? dFecIngreso { get; set; }
        public DateTime? dFecCargaBD { get; set; }
        public DateTime? dFecIniProceso { get; set; }
        public DateTime? dFecFinProceso { get; set; }
        public DateTime? dFecEnvioCliente { get; set; }
        public int? nCar_CtasNro { get; set; }
        public decimal? mCar_CtasMonto { get; set; }
        public bool? bEstado { get; set; }
        public decimal? nPreRecuperoPorc { get; set; }
        public decimal? mPreRecuperoMonto { get; set; }
        public decimal? nOpeRecuperoPprc { get; set; }
        public decimal? mOpeRecuperoMonto { get; set; }
        public string? cCar_ArchNombreRec { get; set; }
        public string? cCar_ArchNombreNor { get; set; }
        public int? nId_sucursal { get; set; }
        public int? nDoc_NroLote { get; set; }
        public int? nId_CarEstado { get; set; }
        public int? nId_Moneda { get; set; }
        public string? cCar_Coment { get; set; }
        public DateTime? dFecIniProcesoReal { get; set; }
        public DateTime? dFecFinProcesoReal { get; set; }
        public decimal? mOpeCtasMonto { get; set; }
        public int? nOpeCtasNro { get; set; }
        public int? nId_Grupo { get; set; }
        public int? nEstad_Car_DocxCobrar { get; set; }
        public int? nEstad_Car_DocxPagar { get; set; }
        public int? nEstad_Car_DocxAFavor { get; set; }
        public int? nEstad_Car_PersDeudor { get; set; }
        public decimal? nEstad_Car_MontoSolesxCobrar { get; set; }
        public decimal? nEstad_Car_MontoDolxCobrar { get; set; }
        public decimal? nEstad_Car_MontoSolesRecup { get; set; }
        public decimal? nEstad_Car_MontoDolRecup { get; set; }
        public int? nEstad_Car_Quejas { get; set; }
        public int? nEstad_Car_HrsGestion { get; set; }
        public int? nOpeDeudoresNro { get; set; }
        public string? cCampanna { get; set; }
        public int? anio { get; set; }
        public string? control { get; set; }
        public int? nCampanna { get; set; }
        public string? cTipo { get; set; }
        public string? cDescripcion { get; set; }
        public int? nProcesa { get; set; }
        public string? cTipoCar { get; set; }
        public int? nAnioCar { get; set; }
        public int? nCampCar { get; set; }
        public string? cCartera { get; set; }
        public string? cSubCartera { get; set; }
        public string? cCiclo { get; set; }
        public int? nId_usuarioCrea { get; set; }
        public DateTime? dDoc_FecCrea { get; set; }
        public int? nId_usuarioModifica { get; set; }
        public DateTime? dDoc_FecModifica { get; set; }

        //relations
        //public av_Cliente av_Cliente { get; set; }
        //public av_DocxCobrar av_DocxCobrar { get; set; }
    }
}