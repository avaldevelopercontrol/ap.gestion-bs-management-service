namespace GesMgmt.Domain.Entities
{
    public class av_AloTelefonoPerfil
    {
        public int nId_PersTelef { get; set; }
        public int nId_PersDeudor { get; set; }
        public string cTelefNroNorm { get; set; }
        public string? AloI { get; set; }
        public DateTime? dFecUltCIIvr { get; set; }
        public int? nIdGestionCIIvr { get; set; }
        public DateTime? dFecUltIvr { get; set; }
        public int nCallHis { get; set; }
        public int nDiscadorHis { get; set; }
        public int nAgenteHis { get; set; }
        public int nIvrHis { get; set; }
        public int nSmsHis { get; set; }
        public int nSistemaHis { get; set; }
        public int nCallAct { get; set; }
        public int nIvrAct { get; set; }
        public int nCdCall { get; set; }
        public int nCiCall { get; set; }
        public int nNcCall { get; set; }
        public int nCiIvr { get; set; }
        public int nCyC { get; set; }
        public int nEquivHis { get; set; }
        public int nEquivAct { get; set; }
        public int nBuzonHis { get; set; }
        public int nBuzonAct { get; set; }
        public int nCallM1 { get; set; }
        public int nCallM2 { get; set; }
        public int nCallM3 { get; set; }
        public int nCallM4 { get; set; }
        public int nCallM0 { get; set; }
        public int nCallActM1 { get; set; }
        public int nCallActM2 { get; set; }
        public int nCallActM3 { get; set; }
        public int nCallActM4 { get; set; }
        public int nCallActM0 { get; set; }
        public int nIvrM1 { get; set; }
        public int nIvrM2 { get; set; }
        public int nIvrM3 { get; set; }
        public int nIvrM4 { get; set; }
        public int nIvrM0 { get; set; }
        public DateTime? dPrimeraGes { get; set; }
        public DateTime? dUltimaGes { get; set; }
        public DateTime? dUltimaCall { get; set; }
        public DateTime? dFecProximoCambioI { get; set; }
        public DateTime dFecActualizacion { get; set; }
    }
}