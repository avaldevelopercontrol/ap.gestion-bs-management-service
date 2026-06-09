using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Application.DTOs.Gestion
{
    public class GetGestionGestCartDeudResponseDto
    {
        public int nId_DocxCobrarOpe { get; set; }
        public int nro { get; set; }
        public string? fechaGestion { get; set; }
        public string? gestor { get; set; }
        public string? documento { get; set; }
        public string? operacion { get; set; }
        public string? respuesta { get; set; }
        public string? comentario { get; set; }
    }
}