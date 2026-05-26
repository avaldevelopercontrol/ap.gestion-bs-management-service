using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Domain.Entities
{
    public class av_Cliente //: BaseEntity
    {
        public int nId_Cliente { get; set; }
        public string? cCli_NroDoc { get; set; }
        public string? cCli_Nombre { get; set; }
        public string? cCli_Siglas { get; set; }
        public string? cCli_Direccion { get; set; }
        public int? nId_Ubigeo { get; set; }
        public string? cCli_Telefonos { get; set; }
        public string? cCli_Fax { get; set; }
        public bool? bEstado { get; set; }
        public DateTime? dFecIngreso { get; set; }
        public DateTime? dFecCierre { get; set; }
        public string? cCli_Coment { get; set; }
        public int? nId_CliEstado { get; set; }
        public string? cCli_Web { get; set; }
        public int? nEstad_Cli_Contratos { get; set; }
        public int? nEstad_Cli_DocxCobrar { get; set; }
        public int? nEstad_Cli_DocxPagar { get; set; }
        public int? nEstad_Cli_DocxAFavor { get; set; }
        public int? nEstad_Cli_PersDeudor { get; set; }
        public decimal? nEstad_Cli_MontoSolesxCobrar { get; set; }
        public decimal? nEstad_Cli_MontoDolxCobrar { get; set; }
        public decimal? nEstad_Cli_MontoSolesRecup { get; set; }
        public decimal? nEstad_Cli_MontoDolRecup { get; set; }
        public int? nEstad_Cli_Quejas { get; set; }
        public int? nEstad_Cli_HrsGestion { get; set; }
        public int? nEstad_Cli_Carteras { get; set; }
        public bool? bPagoDirecto { get; set; }
        public string? cCli_CodAnterior { get; set; }
        public int? ntip_campanna { get; set; }
        public int? ntip_cliente { get; set; }
        public int? swt_add { get; set; }
        public int? swt_detalle { get; set; }
        public int? swt_estadoGest { get; set; }
        public string? swt_OpeContacto { get; set; }
        public int nId_TipoCliente { get; set; }
        public int nId_ClienteGen { get; set; }
        public string? cTipoArchivoBaseUp { get; set; }
        public int? nEquivDiscadorIVR { get; set; }
        public string? cCli_SiglasOpe { get; set; }
        public int? nCalifTelefonoGes { get; set; }
        public string? cCli_Correos { get; set; }
        public string? cCli_OCMPredictivo { get; set; }
        public string? cCli_GTELCOMPredictivo { get; set; }

        //public av_DocxCobrar av_DocxCobrar { get; set; }
    }
}
