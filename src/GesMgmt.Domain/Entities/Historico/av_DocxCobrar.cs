namespace GesMgmt.Domain.Entities.Historico
{
    public class av_DocxCobrar
    {
        public int nId_DocxCobrar { get; set; }

        public int nId_Cliente { get; set; }
        public int nId_Cartera { get; set; }
        public int nId_PersDeudor { get; set; }
        public int? nId_Moneda { get; set; }
        public int? nId_Usuario { get; set; }

        public virtual av_Cliente av_Cliente { get; set; }
        public virtual av_Cartera av_Cartera { get; set; }
        public virtual av_PersDeudor av_PersDeudor { get; set; }
        public virtual av_Moneda av_Moneda { get; set; }
        public virtual av_Usuario av_Usuario { get; set; }
        public string? cDoc_Numero { get; set; }
        public DateTime? dDoc_FecEmision { get; set; }
        public DateTime? dDoc_FecVenc { get; set; }
        public decimal? nDoc_ImpTotal { get; set; }
        public decimal? nDoc_ImpSaldo { get; set; }
        public int? bEstado { get; set; }
        public string? cPers_CodCliente { get; set; }
        public string? cDoc_Coment { get; set; }
        public int? nDoc_DiasAtrazo { get; set; }
        public int? nid_estrategia { get; set; }
        public int? mej_status { get; set; }
    }
}