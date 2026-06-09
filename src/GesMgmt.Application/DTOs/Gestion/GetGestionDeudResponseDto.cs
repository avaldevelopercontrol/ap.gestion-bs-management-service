using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Application.DTOs.Gestion
{
    public class GetGestionDeudResponseDto
    {
        public int nId_PersDeudor { get; set; }
        public string? dni { get; set; }
        public string? ruc { get; set; }
        public string? nombre { get; set; }
        public string? nombreCompleto { get; set; }
        public string? gradoInstruccion { get; set; }
        public string? edad { get; set; }
        public string? correo { get; set; }
        public bool? informacionAdicional { get; set; }
        public bool? pagos { get; set; }
        public bool? agendas { get; set; }
        public bool? llamadas { get; set; }
        public DateTime fechaConsulta { get; set; }
        public string? codigo { get; set; }
        public string? asesorPostVenta { get; set; }
        public string? correoAsesorPostVenta { get; set; }
        public string? asesorComercial { get; set; }
        public string? correoAsesorComercial { get; set; }
        public bool? validaCronograma { get; set; }
        public string? clientePorVision { get; set; }
        public string? clienteListaBlanca { get; set; }
        public string? clienteConSinPe { get; set; }
    }
}