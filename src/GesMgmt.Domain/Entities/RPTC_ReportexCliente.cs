
namespace GesMgmt.Domain.Entities
{
    public class RPTC_ReportexCliente
    {
        public int nId_Reporte { get; set; }
        public int nId_Cliente { get; set; }
        public av_Cliente av_Cliente { get; set; }
        public string cRep_NombreRep { get; set; }
        public string? cRep_StoreProc { get; set; }
        public bool? bestado { get; set; }
        public int? nRep_EnvioDefecto { get; set; }
        public int? nTipoEjecucion { get; set; }
        public string? cPar_RutaAction { get; set; }
        public bool? bUploadForm { get; set; }
        public int? nTipoParametroStore { get; set; }
        public int? cnId_Perfil { get; set; }
        public bool? bResultStoreSinHtml { get; set; }
        public int? nNroMaxRegistros { get; set; }
        public DateTime? dFecActualizacion { get; set; }
        public string? cRutaPlantilla { get; set; }
    }
}