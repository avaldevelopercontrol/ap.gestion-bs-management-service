using System;
using System.Collections.Generic;

namespace GesMgmt.Domain.Entities
{
    public class av_DocxCobrar
    {
        public int nId_DocxCobrar { get; set; }

        public int nId_Cliente { get; set; }
        public int nId_Cartera { get; set; }
        public int nId_PersDeudor { get; set; }
        public int? nId_Moneda { get; set; }
        public int? nId_Usuario { get; set; }

        public virtual av_Cliente av_Cliente { get; set; }
        public virtual av_Cartera av_Cartera { get; set; }
        public virtual av_PersDeudor av_PersDeudor { get; set; }
        public virtual av_Moneda av_Moneda { get; set; }
        public virtual av_Usuario av_Usuario { get; set; }
        //public virtual av_DocxCobrarParam av_DocxCobrarParam { get; set; }

        //public DateTime? dDoc_FecIngreso { get; set; }
        //public int? nId_DocTipo { get; set; }
        public string? cDoc_Numero { get; set; }
        //public DateTime? dDoc_FecEmision { get; set; }
        public DateTime? dDoc_FecVenc { get; set; }
        public decimal? nDoc_ImpTotal { get; set; }
        //public decimal? nDoc_ImpInafecto { get; set; }
        //public decimal? nDoc_ImpIGV { get; set; }
        //public decimal? nDoc_ImpISC { get; set; }
        public decimal? nDoc_ImpSaldo { get; set; }
        //public decimal? nDoc_Intereses { get; set; }
        //public decimal? nDoc_Seguros { get; set; }
        //public decimal? nDoc_Portes { get; set; }
        //public DateTime? dDoc_PagoComprom { get; set; }
        //public DateTime? dDoc_PagoReclamo { get; set; }
        public int? bEstado { get; set; }
        //public string? cDoc_NroOpeCli { get; set; }
        //public string? cDoc_NroCuota { get; set; }
        //public string? cDoc_NroCtaCargo { get; set; }
        public string? cPers_CodCliente { get; set; }
        //public int? nDoc_NroLote { get; set; }


        public string? cDoc_Coment { get; set; }
        public int? nDoc_DiasAtrazo { get; set; }
        //public int? nDoc_Tramo { get; set; }
        //public string? cDoc_numero_pre { get; set; }
        //public string? cDoc_numero_post { get; set; }
        //public int? nId_DocxCobrarEst { get; set; }
        //public DateTime? dDoc_FecCancel { get; set; }
        //public int? nDoc_NroGes { get; set; }
        //public int? nDoc_HrsGes { get; set; }
        //public DateTime? dDoc_FecGes { get; set; }


        //public int? nId_OpeCampo { get; set; }
        //public decimal? nDoc_ImpPagoD { get; set; }
        //public decimal? nDoc_ImpPagoF { get; set; }
        //public DateTime? dDoc_NueFecGesTelef { get; set; }
        //public bool? nDoc_FlagSeleccion { get; set; }
        //public int? nDoc_TipoDeuda { get; set; }
        //public bool? nDoc_FlagControl { get; set; }
        //public bool? nDoc_FlagCancelado { get; set; }
        //public int? nId_Ubigeo { get; set; }
        //public string? campo_filtro { get; set; }
        //public DateTime? dDoc_FecGesCam { get; set; }
        //public int? ult_status { get; set; }
        public int? nid_estrategia { get; set; }
        //public bool? marcador { get; set; }
        //public int? nId_OpeCam { get; set; }
        //public bool? swt_telef { get; set; }
        //public int? nid_estrategiaCam { get; set; }
        //public string? nid_opeGes { get; set; }
        //public string? cCampo1 { get; set; }
        //public string? cCampo2 { get; set; }
        //public string? cCampo3 { get; set; }
        //public string? cCampo4 { get; set; }
        //public DateTime? dDoc_FecCompromiso { get; set; }
        //public decimal? nSaldoTotal { get; set; }
        //public decimal? nImpTotal { get; set; }
        //public int? ult_statusEst { get; set; }
        //public DateTime? dDoc_FecGesEst { get; set; }
        //public DateTime? dDoc_FecGesCamEst { get; set; }
        //public DateTime? dFec_bEstado { get; set; }
        public int? mej_status { get; set; }
        //public int? nId_TipIngreso { get; set; }
        //public int? nId_CarteraOrig { get; set; }
        //public DateTime? dDoc_FecProgVisita { get; set; }
        //public int? nCantTotal { get; set; }
        //public int? nCantSaldo { get; set; }
        //public DateTime? dDoc_FecActual { get; set; }
        //public DateTime? dDoc_FecRetiro { get; set; }
        //public int? nId_TramoCli { get; set; }
        //public int? nId_SubZonaGen { get; set; }
        //public int? nId_CliCartera { get; set; }
        //public int? nId_ZonaCli { get; set; }
        //public int? nId_SubZonaAsig { get; set; }
        //public decimal? nDocObjetivoP { get; set; }
        //public decimal? nDocObjetivoM { get; set; }
        //public decimal? nDocComisionP { get; set; }
        //public decimal? nDocComisionM { get; set; }
        //public DateTime? dFec_SubZonaAsig { get; set; }
        //public int? nCierre_Temp { get; set; }
        //public int? nid_docxcobrar_orig { get; set; }

        // Cambiado a colección para que coincida con la relación One(av_DocxCobrar) - Many(av_DocxCobrarParam)

        //public ICollection<av_DocxCobrarOpe> av_DocxCobrarOpes { get; set; }
    }
}