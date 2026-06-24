using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Entities
{
    public class av_PersDeudorParam
    {
        public int nId_PersDeudorParam { get; set; }
        public int nId_Cartera { get; set; }
        public av_Cartera av_Cartera { get; set; }
        public int nId_PersDeudor { get; set; }
        public av_PersDeudor av_PersDeudor { get; set; }
        public string cPers_CodCliente { get; set; }
        public int nZona { get; set; }
        public string? cDepartamento { get; set; }
        public string? cProvincia { get; set; }
        public string? cDistrito { get; set; }
        public bool bEstado { get; set; }
        public decimal nImpTotal { get; set; }
        public decimal nSaldoTotal { get; set; }
        public DateTime? dDoc_FecGes { get; set; }
        public DateTime? dDoc_FecGesCam { get; set; }
        public DateTime? dDoc_FecCompromiso { get; set; }
        public DateTime? dDoc_FecProgVisita { get; set; }
        public int? ult_status { get; set; }
        public int? mej_status { get; set; }
        public int nCantTotal { get; set; }
        public int nCantSaldo { get; set; }
        public int? ult_statusEst { get; set; }
        public string? cPDeuParam01 { get; set; }
        public string? cPDeuParam02 { get; set; }
        public string? cPDeuParam03 { get; set; }
        public string? cPDeuParam04 { get; set; }
        public string? cPDeuParam05 { get; set; }
        public string? cPDeuParam06 { get; set; }
        public string? cPDeuParam07 { get; set; }
        public string? cPDeuParam08 { get; set; }
        public string? cPDeuParam09 { get; set; }
        public string? cPDeuParam10 { get; set; }
        public string? cPDeuParam11 { get; set; }
        public string? cPDeuParam12 { get; set; }
        public string? cPDeuParam13 { get; set; }
        public string? cPDeuParam14 { get; set; }
        public string? cPDeuParam15 { get; set; }
        public string? cPDeuParam16 { get; set; }
        public string? cPDeuParam17 { get; set; }
        public string? cPDeuParam18 { get; set; }
        public string? cPDeuParam19 { get; set; }
        public string? cPDeuParam20 { get; set; }
        public string? cPDeuParam21 { get; set; }
        public string? cPDeuParam22 { get; set; }
        public string? cPDeuParam23 { get; set; }
        public string? cPDeuParam24 { get; set; }
        public string? cPDeuParam25 { get; set; }
        public string? cPDeuParam26 { get; set; }
        public string? cPDeuParam27 { get; set; }
        public DateTime? dFec_Actualiza { get; set; }
        public bool? swt_telef { get; set; }
        public DateTime? dDoc_FecGesEst { get; set; }
        public DateTime? dDoc_FecGesCamEst { get; set; }
        public int? ult_statusNp2 { get; set; }
        public int? mej_statusNp2 { get; set; }
        public bool? bvisualiza { get; set; }
    }
}