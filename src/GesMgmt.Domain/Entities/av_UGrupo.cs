
namespace GesMgmt.Domain.Entities
{
    public class av_UGrupo
    {
        public int nId_UGrupo { get; set; }
        public int? nId_Usuario { get; set; }
        public av_Usuario av_Usuario { get; set; }
        public int? nId_Grupo { get; set; }
        public av_Grupo av_Grupo { get; set; }
        public DateTime? dUGrupo_FecIni { get; set; }
        public DateTime? dUGrupo_FecFin { get; set; }
        public bool? bEstado { get; set; }
        public bool? bActivo { get; set; }
        public bool? bGestion { get; set; }
    }
}