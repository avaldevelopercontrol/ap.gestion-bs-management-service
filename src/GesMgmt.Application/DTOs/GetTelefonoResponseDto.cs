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
        public DateTime? fechaEstado { get; set; }
        public DateTime? fechaBase { get; set; }
        public string? contactados { get; set; }
        public string? noContactados { get; set; }
        public string? cantidadIvr { get; set; }
        public string? fuente { get; set; }
        public string? ordenSearch { get; set; }
    }
}