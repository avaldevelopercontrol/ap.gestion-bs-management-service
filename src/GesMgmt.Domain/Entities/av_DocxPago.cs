
namespace GesMgmt.Domain.Entities
{
    public class av_DocxPago
    {
        public int nId_DocxPago { get; set; }
        public int nId_DocxCobrar { get; set; }
        public int nId_Cliente { get; set; }
        public int nId_Contrato { get; set; }
        public int nId_Cartera { get; set; }
        public int nId_PersDeudor { get; set; }
        public DateTime dDoc_FecIngreso { get; set; }
        public int? nId_MonDeuda { get; set; }
        public decimal? nDoc_ImpDeuda { get; set; }
        public int? nId_DocTipo { get; set; }
        public string? cDoc_Numero { get; set; }
        public DateTime dDoc_FecPago { get; set; }
        public int? nId_MonPago { get; set; }
        public decimal? nDoc_ImpPago { get; set; }
        public string? cDoc_NroOpeCli { get; set; }
        public string? cDoc_NroCuota { get; set; }
        public string? cDoc_NroCtaCargo { get; set; }
        public string? cPers_CodCliente { get; set; }
        public int? nDoc_NroLote { get; set; }
        public int? nDoc_Tramo { get; set; }
        public string? cDoc_Coment { get; set; }
        public int? nId_Usuario { get; set; }
        public bool? bDentro { get; set; }
        public bool? bEstado { get; set; }
        public decimal? nDoc_ImpSaldo { get; set; }
        public string? cDocxPago_Control { get; set; }
        public int? nId_DocxPagoMedio { get; set; }
        public int? nId_Banco { get; set; }
        public int? nId_UsuarioCob { get; set; }
        public int? nId_CliBcoCta { get; set; }
        public DateTime dFec_BancoDep { get; set; }
        public int? nId_DocxCobrarNC { get; set; }
        public int? nAtrazo_DocxCobrar { get; set; }
        public decimal? nComision_Porc { get; set; }
        public decimal? nComision_Importe { get; set; }
        public decimal? nDoc_ImpParam01 { get; set; }
        public decimal? nDoc_ImpParam02 { get; set; }
        public string? cMarca { get; set; }
        public string? cDoc_Param01 { get; set; }
        public string? cDoc_Param02 { get; set; }
        public string? cDoc_Param03 { get; set; }
        public int? nId_DocxCobrarEst { get; set; }
    }
}