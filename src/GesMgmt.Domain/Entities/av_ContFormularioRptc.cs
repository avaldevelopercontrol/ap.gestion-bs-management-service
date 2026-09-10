
namespace GesMgmt.Domain.Entities
{
    public class av_ContFormularioRptc
    {
        public int nId_ContFormRptc { get; set; }
        public int nId_Contrato { get; set; }
        public av_Contrato av_Contrato { get; set; }
        public int nTipoFormCrud { get; set; }
        public av_ContFormTipoCrud av_ContFormTipoCrud { get; set; }
        public string? cScriptStoreParamList { get; set; }
        public int nTipoFormParamOpe { get; set; }
        public av_ContFormTipoParamOpe av_ContFormTipoParamOpe { get; set; }
        public int nId_ReporteFormParam { get; set; }
        public RPTC_ReportexCliente RPTC_ReportexCliente { get; set; }
        public string? cScriptStoreParamEdit { get; set; }
        public bool bEstado { get; set; }
        public string? cNombreFormLink { get; set; }
    }
}