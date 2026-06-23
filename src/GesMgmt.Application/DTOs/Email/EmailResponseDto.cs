using GesMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Application.DTOs.Email
{
    public class EmailResponseDto
    {
        public class GetEmailsPersDeudorResponseDto
        {
            public int nId_PersEmail { get; set; }
            public string? email { get; set; }
            public DateTime? fechaActivacion { get; set; }
            public string? estado { get; set; }
            public string? status { get; set; }
            public string? fuente { get; set; }
            public string? baseCliente { get; set; }
            public string? contacto { get; set; }
            public int? prioridad { get; set; }
            public string? comentario { get; set; }
        }

        public class GetPersEmailsResponseDto {
            public int nId_PersEmail { get; set; }
            public int nId_PersDeudor { get; set; }
            public string cPers_Email { get; set; }
            public bool bEstado { get; set; }
            public string? cEmail_Coment { get; set; }
            public string? cEmail_Contacto { get; set; }
            public int nId_Cliente { get; set; }
            public bool bBaseCliente { get; set; }
            public DateTime dFecRegistro { get; set; }
            public int nId_UsuarioAct { get; set; }
            public DateTime dFecActualizacion { get; set; }
            public int? nEmail_Prioridad { get; set; }
            public int? nId_EstadoEnvioEmail { get; set; }
            public string? cEstado { get; set; }
            public DateTime? dFecEstadoEnvio { get; set; }
            public int? nId_EstadoEnvioEmailGen { get; set; }
            public DateTime? dFecBaseCliente { get; set; }
            public int? nId_PersEmailOpe { get; set; }
        }
    }
}
