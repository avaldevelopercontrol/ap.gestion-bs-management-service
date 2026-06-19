
namespace GesMgmt.Domain.Entities
{
    public class av_CabPantallaCob
    {
        public int nId_CabPantalla { get; set; }
        public string cTitulo { get; set; }
        public string cTipoDato { get; set; }
        public bool? bOperaTotal { get; set; }
        public bool? bCompromisoClick { get; set; }
        public int nOrden { get; set; }
        public int nPantalla { get; set; }
        public string? cAlignHtml { get; set; }
        public int? nId_Contrato { get; set; }
        public int nId_Cliente { get; set; }
    }
}