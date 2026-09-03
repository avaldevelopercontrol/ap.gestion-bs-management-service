
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
            public string? fechaUltimaGestionCALL { get; set; }
            public string? ultimaGestionCALL { get; set; }
            public int? cantidadGestionCALL { get; set; }
            public string? fechaUltimaGestionCAMPO { get; set; }
            public string? ultimaGestionCAMPO { get; set; }
            public int? cantidadGestionCAMPO { get; set; }
            public string? fechaPromesa { get; set; }
            public string mejorStatus { get; set; }
        }
    }
}