
namespace GesMgmt.Domain.Entities
{
    public class av_OpeCodCliOutEst
    {
        public int nId_OpeCodCliOut { get; set; }
        public string? cNombre_OpeCodCliOut { get; set; }
        public int? nId_OpeCodOut2 { get; set; }
        public int? ncat_gestion { get; set; }
        public int? nFecCompDiaIni { get; set; }
        public int? nFecCompDiaFin { get; set; }
        public int? nValida_telef { get; set; }
        public int? nPeso { get; set; }
        public bool? bEstado { get; set; }
        public int? nEquivDiscImpGestion { get; set; }
        public int? nNivelPaleta { get; set; }
        public string? cParam01 { get; set; }
        public string? cParam02 { get; set; }
        public string? cParam03 { get; set; }
        public string? cParam04 { get; set; }
        public string? cCodigo_OpeCodCliOut { get; set; }
        public string? cSigla_OpeCodCliOut { get; set; }
        public string? cRequer_OpeCodCliOut { get; set; }
        public int? nId_Cliente { get; set; }
        public int? nNivel1 { get; set; }
        public int? nEstado_Gestion { get; set; }
        public int? nId_OpeCodCliIn { get; set; }
        public bool? bIsClient { get; set; }
    }
}