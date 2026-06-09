using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Application.DTOs.Gestion
{
    public class GetGestionAdicResponseDto
    {
        public int nId_DocxCobrarAd { get; set; } //1 Cabecera principal
        public int? nId_DocxCobrar { get; set; } //2 Cabecera principal
        public int? nId_PersDeudor { get; set; } //2 Cabecera principal
        public int? nId_Cartera { get; set; } //3 Cabecera principal
        public int? nId_Cliente { get; set; } //5 Cabecera principal
        //-- Campos adicionales para la gestión
        //-- 95 CLARO
        public string? recibo { get; set; }
        public string? telefono { get; set; }
        public string? servicio { get; set; }
        public string? estadoServicio { get; set; }
        public string? motivo { get; set; }
        public string? codigoCliente { get; set; }
    }
}