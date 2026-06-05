using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Application.DTOs
{
    public class GetTelefonoResponseDto
    {
        public int? prioridad { get; set; }
        public string? nroTelefono { get; set; }
        public string? horario { get; set; }
        public string? referenciaUbicacion { get; set; }
        public string? estado { get; set; }
        public string? fechaEstado { get; set; }
        public string? fechaBase { get; set; }
        public string? contactados { get; set; }
        public int? noContactados { get; set; }
        public int? cantidadIvr { get; set; }
        public string? fuente { get; set; }
        public string? ordenSearch { get; set; }
    }
}