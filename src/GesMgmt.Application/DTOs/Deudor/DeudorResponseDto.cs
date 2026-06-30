using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Application.DTOs.Deudor
{
    public class DeudorResponseDto
    {
        public class GetDeudorResponseDto
        {
            public int nId_PersDeudor { get; set; }
            public int nro { get; set; }
            public string? zonaCampanna { get; set; }
            public int nId_Cliente { get; set; }
            public int nId_Contrato { get; set; }
            public int nId_Cartera { get; set; }
            public string? cartera { get; set; }
            public string? codigoCliente { get; set; }
            public string? deudor { get; set; }
            public decimal? importe { get; set; }
            public decimal? saldo { get; set; }
            public string? FechaUltimaGestionCALL { get; set; }
            public string? UltimaGestionCALL { get; set; }
            public int? cantidadGestionCALL { get; set; }
            public string? FechaUltimaGestionCAMPO { get; set; }
            public string? UltimaGestionCAMPO { get; set; }
            public int? cantidadGestionCAMPO { get; set; }
            public string? FechaPromesa { get; set; }
            public string mejorStatus { get; set; }
        }
    }
}