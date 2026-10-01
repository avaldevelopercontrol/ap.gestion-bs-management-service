namespace GesMgmt.Domain.Entities
{
    public class av_AloEstadoDocumental
    {
        public int nId_PersTelef { get; set; }
        public string cTelefNroNorm { get; set; }
        public DateTime? dFecUltTitular { get; set; }
        public int? nIdGestionTitular { get; set; }
        public DateTime? dFecUltTerceroRel { get; set; }
        public int? nIdGestionTerceroRel { get; set; }
        public DateTime? dFecUltCIPlataforma { get; set; }
        public int? nIdGestionCIPlataforma { get; set; }
        public DateTime? dFecUltNoContacto { get; set; }
        public int? nIdGestionNoContacto { get; set; }
        public DateTime? dFecUltT8 { get; set; }
        public int? nIdGestionT8 { get; set; }
        public string? cClaseVigente { get; set; }
        public DateTime? dFecClaseVigente { get; set; }
        public int? nIdGestionVigente { get; set; }
        public string? AloCalculado { get; set; }
        public DateTime? dFecProximoCambio { get; set; }
        public DateTime? dFecActualizacion { get; set; }
    }
}