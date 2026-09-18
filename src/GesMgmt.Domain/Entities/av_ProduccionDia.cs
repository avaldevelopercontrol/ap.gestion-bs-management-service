namespace GesMgmt.Domain.Entities
{
    public class av_ProduccionDia
    {
        public int? nId_UsuOpe { get; set; }
        public int? nid_cliente { get; set; }
        public int? nId_PersDeudor { get; set; }
        public int? nId_opeCodOut { get; set; }
        public DateTime? dDocCobOpe_FecIni { get; set; }
        public int? cDocxCobOpeInconcert { get; set; }
    }
}