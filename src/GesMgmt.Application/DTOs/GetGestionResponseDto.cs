using System.ComponentModel.DataAnnotations.Schema;

namespace GesMgmt.Application.DTOs
{
    public class GetGestionResponseDto
    {
        public int nId_DocxCobrar { get; set; } //1 Cabecera principal
        public int? Mejor_Status { get; set; } //2 Cabecera principal
        public int? nId_Moneda { get; set; } //2 Cabecera principal
        public int? bEstado { get; set; } //3 Cabecera principal
        public string? nZona { get; set; } //3 Cabecera principal
        public bool bSelected { get; set; } //4 Cabecera principal
        public int? nId_Estrategia { get; set; } //5 Cabecera principal
        public int nId_Cartera { get; set; } //6 Cabecera principal

        //-- Campos adicionales para la gestión
        //-- 95 CLARO
        public int Nro { get; set; }
        public string? Numero_Documento { get; set; }
        public string? Estado { get; set; }
        public string? Fecha_Vencimiento { get; set; }
        public string? Sigla_Moneda { get; set; }
        public decimal? Importe_Total { get; set; }
        public decimal? Importe_Saldo { get; set; }
        public decimal Deuda_Vencida { get; set; }
        public int Dias_Atrazo { get; set; }
        public string? Servicio { get; set; }
        public string? Comentario { get; set; }
        public string? Codigo_Cliente { get; set; }
        public string? Estado_Documento { get; set; }
        public string? Fecha_Estado_Documento { get; set; }
        public string? Estado_Pago { get; set; }
        public string? Status_Documento { get; set; }
        public string? Fecha_StatusDocumento { get; set; }
        public string? Gestor_Call { get; set; }
        public string? Baja_Provabilidad { get; set; }
    }
}