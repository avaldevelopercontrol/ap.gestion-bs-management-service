using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Application.DTOs.Gestion
{
    public class GetGestionCabeAdicResponseDto
    {
        public int idCab { get; set; } //1 Cabecera principal
        public int? nId_Cliente { get; set; } //1 Cabecera principal
        public int? pantalla { get; set; } //1 Cabecera principal
        public string? recibo { get; set; }
        public string? telefono { get; set; }
        public string? servicio { get; set; }
        public string? estadoServicio { get; set; }
        public string? motivo { get; set; }
        public string? codigoCliente { get; set; }
    }
}