
namespace GesMgmt.Domain.Entities
{
    public class av_PersTelefOpeDetalle
    {
        public int nId_PersTelefOpeDet { get; set; }
        public int? nId_PersTelef { get; set; }
        public av_PersTelef av_PersTelef { get; set; }
        public int? nId_PersTelefOpe { get; set; }
        public av_PersTelefOpe av_PersTelefOpe { get; set; }
        public DateTime? dFec_PerstelefOpe { get; set; }
        public int? nId_Usuario { get; set; }
    }
}