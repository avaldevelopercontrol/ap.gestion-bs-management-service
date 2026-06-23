using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Application.DTOs.Email
{
    public class EmailRequestDto
    {
        public class GetEmailsPersDeudorRequestDto
        {
            public int nId_Cliente { get; set; } //ID_CLIENTE
            public int nId_Persdeudor { get; set; } //ID_DEUDOR

            // 🔹 PAGINACIÓN
            public int PageNumber { get; set; } = 1;
            private int _pageSize = 10;

            public int PageSize
            {
                get => _pageSize;
                set => _pageSize = value > 1000 ? 1000 : value; // Máximo 50
            }
        }

        public class CreateEmailRequestDto
        {
            public int nId_PersDeudor { get; set; }
            public string cPers_Email { get; set; }
            public bool bEstado { get; set; }
            public string? cEmail_Coment { get; set; }
            public string? cEmail_Contacto { get; set; }
            public int nId_Cliente { get; set; }
            public bool bBaseCliente { get; set; }
            public int nId_UsuarioAct { get; set; }
            public DateTime dFecRegistro { get; set; }
            public DateTime dFecActualizacion { get; set; }
            public int? nEmail_Prioridad { get; set; }
            public int? nId_PersEmailOpe { get; set; }
        }

        public class EditEmailRequestDto
        {
            public int nId_PersEmail { get; set; }
            public int nId_PersDeudor { get; set; }
            public string cPers_Email { get; set; }
            public bool bEstado { get; set; }
            public string? cEmail_Coment { get; set; }
            public string? cEmail_Contacto { get; set; }
            public int nId_Cliente { get; set; }
            public bool bBaseCliente { get; set; }
            public int nId_UsuarioAct { get; set; }
            public DateTime dFecRegistro { get; set; }
            public DateTime dFecActualizacion { get; set; }
            public int? nEmail_Prioridad { get; set; }
            public int? nId_PersEmailOpe { get; set; }
        }
    }
}