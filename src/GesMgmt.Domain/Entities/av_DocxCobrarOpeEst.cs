
namespace GesMgmt.Domain.Entities
{
    public class av_DocxCobrarOpeEst
    {
        public int nId_DocxCobrarOpe { get; set; }
        public int nId_DocxCobrar { get; set; }
        public av_DocxCobrar av_DocxCobrar { get; set; }
        public int nId_OpeCodIn { get; set; }
        public DateTime? dDocCobOpe_FecIni { get; set; }
        public DateTime? dDocCobOpe_FecFin { get; set; }
        public string? cDocOpeCobIn_Descr { get; set; }
        public int nId_OpeCodCliOut { get; set; }
        //public int nId_OpeCodOut { get; set; }
        public av_OpeCodCliOutEst av_OpeCodCliOutEst { get; set; }
        public bool? bEstado { get; set; }
        public int? nId_Usuario { get; set; }
        public av_Usuario? av_Usuario { get; set; }
        public int? nId_Estrategia { get; set; }
        public int? nId_UsrLider { get; set; }
        public int? nDoc_NroLote { get; set; }
        public string? cDocOpeCobOut_Descr { get; set; }
        public int? nId_Cliente { get; set; }
        public int? nId_Contrato { get; set; }
        public int nId_Cartera { get; set; }
        public av_Cartera av_Cartera { get; set; }
        public int nId_PersDeudor { get; set; }
        public av_PersDeudor av_PersDeudor { get; set; }
        public bool? bOpeEfectiva { get; set; }
        public DateTime? dFechCompromisoPago { get; set; }
        public DateTime? dFechNuevaGestion { get; set; }
        public int? nId_OpeContacto { get; set; }
        public int? nId_OpeCodOut2 { get; set; }
        public string? nTelef_Nro { get; set; }
        public decimal? monto_comp { get; set; }
        public decimal? monto_compDolares { get; set; }
        public bool? cDocxCobOpeInconcert { get; set; }
        public int? nId_TipoGestion { get; set; }
        public av_TipoGestion? av_TipoGestion { get; set; }
        public string? cusuar { get; set; }
        public int? usu_reg { get; set; }
        public int? nid_docxcobraropeOrig { get; set; }
        public DateTime? dDoc_FecIngreso { get; set; }
        public int? nId_Gestion { get; set; }
        public int? nId_GestionDisp { get; set; }
        public string? cID_Llamada { get; set; }
        public string? cnombreContacto { get; set; }
        public string? ccargoContacto { get; set; }
    }
}