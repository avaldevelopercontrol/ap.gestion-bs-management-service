
namespace GesMgmt.Domain.Entities
{
    public class av_PersDeudorGestionHrs
    {
        public int nId_PersDeudorGestionHrs { get; set; }
        public string? cNombren_PersDeudorGestionHrs { get; set; }
        public string? cSigla_PersDeudorGestionHrs { get; set; }
        public bool? bEstado { get; set; }
        public int? nHr_ini { get; set; }
        public int? nHr_fin { get; set; }
    }
}