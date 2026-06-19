
namespace GesMgmt.Domain.Entities
{
    public class av_Contrato //: BaseEntity
    {
        public int nId_Contrato { get; set; }
        public int nId_Cliente { get; set; }
        public av_Cliente av_Cliente { get; set; }
        public DateTime? dCon_FecIniOpe { get; set; }
        public DateTime? dCon_FecFinOpe { get; set; }
        public bool? bEstado { get; set; }
        public string? cCon_FirmaAval { get; set; }
        public string? cCon_FirmaCliente { get; set; }
        public string? cCon_Detalles { get; set; }
        public int? nId_ConEstado { get; set; }
        public DateTime? dCon_FecIniNeg { get; set; }
        public DateTime? dCon_FecFirma { get; set; }
        public DateTime? dCon_FecRenov { get; set; }
        public string? cCon_Login { get; set; }
        public string? cCon_Pass { get; set; }
        public int? nId_ContratoPadre { get; set; }
        public string? cAlias_Contrato { get; set; }
        public string? cCod_ClienteAval { get; set; }
        public int? nEstad_Con_DocxCobrar { get; set; }
        public int? nEstad_Con_DocxPagar { get; set; }
        public int? nEstad_Con_DocxAFavor { get; set; }
        public int? nEstad_Con_PersDeudor { get; set; }
        public decimal? nEstad_Con_MontoSolesxCobrar { get; set; }
        public decimal? nEstad_Con_MontoDolxCobrar { get; set; }
        public decimal? nEstad_Con_MontoSolesRecupe { get; set; }
        public decimal? nEstad_Con_MontoDolRecup { get; set; }
        public int? nEstad_Con_Quejas { get; set; }
        public int? nEstad_Con_HrsGestion { get; set; }
        public int? nEstad_Con_Carteras { get; set; }
        public int? nId_Grupo { get; set; }
        public bool? bGestionEfectivaExit { get; set; }
        public int? nFormProgVisita { get; set; }
        public int? nNivelPaleta { get; set; }
        public int? nAsigZonal_Gestor { get; set; }
        public string? cRutaActionReportCobra { get; set; }
        public int? nDocGestCheck { get; set; }
        public string? cStoreListarDoc { get; set; }
        public string? cCarpetaArcGestMovil { get; set; }
        public int? nDiasGestionAntigua { get; set; }
        public string? cStoreListarPago { get; set; }
        public string? cStoreListarDocDetalle { get; set; }
        public int? nGestionHisClienteGen { get; set; }
        public string? cDocGestHeight { get; set; }
        public string? cDetGestionPerfil { get; set; }
        public string? cMantGestionPerfil { get; set; }
        public int? nProgVisitaUsuAsig { get; set; }
        public string? cRutaActionReportCondona { get; set; }
    }
}