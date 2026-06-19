
namespace GesMgmt.Domain.Entities
{
    public class av_SubZonaGeneral
    {
        public int nId_SubZonaGen { get; set; }
        public int nId_ZonaGen { get; set; }
        //public av_ZonaGeneral av_ZonaGeneral { get; set; }
        public string? cSzgn_Codigo { get; set; }
        public string? cSzgn_Nombre { get; set; }
        public bool? bestado { get; set; }
    }
}