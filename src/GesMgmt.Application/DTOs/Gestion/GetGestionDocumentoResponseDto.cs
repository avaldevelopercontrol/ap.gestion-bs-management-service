using System.ComponentModel.DataAnnotations.Schema;

namespace GesMgmt.Application.DTOs.Gestion
{
    public class GetGestionDocumentoResponseDto
    {
        public int nId_DocxCobrar { get; set; } //1 Cabecera principal
        public int? mejorStatus { get; set; } //2 Cabecera principal
        public int nId_Moneda { get; set; } //2 Cabecera principal
        public int? bEstado { get; set; } //3 Cabecera principal
        public string? nZona { get; set; } //3 Cabecera principal
        public bool bSelected { get; set; } //4 Cabecera principal
        public int? nId_Estrategia { get; set; } //5 Cabecera principal
        public int nId_Cartera { get; set; } //6 Cabecera principal

        //-- Campos adicionales para la gestión
        //-- 95 CLARO
        public int nro { get; set; }
        public string? numeroDocumento { get; set; }
        public string? estado { get; set; }
        public string? fechaVencimiento { get; set; }
        public string? siglaMoneda { get; set; }
        public decimal? importeTotal { get; set; }
        public decimal? importeSaldo { get; set; }
        //public decimal deudaVencida { get; set; }
        public int diasAtrazo { get; set; }
        public string? servicio { get; set; }
        public string? comentario { get; set; }
        public string? codigoCliente { get; set; }
        public string? estadoDocumento { get; set; }
        public string? fechaEstadoDocumento { get; set; }
        public string? estadoPago { get; set; }
        public string? statusDocumento { get; set; }
        public string? fechaStatusDocumento { get; set; }
        public string? gestorCall { get; set; }
        public string? bajaProvabilidad { get; set; }
    }
}