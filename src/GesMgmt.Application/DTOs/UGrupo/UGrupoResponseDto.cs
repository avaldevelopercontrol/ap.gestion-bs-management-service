using GesMgmt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Application.DTOs.UGrupo
{
    public class UGrupoResponseDto
    {
        public class GetUsuarioGrupoListadoResponseDto
        {
            public int nId_UGrupo { get; set; }
            public int? nId_Usuario { get; set; }
            public string cUsr_Login { get; set; }
            public string? cUsr_ApePat { get; set; }
            public string? cUsr_ApeMat { get; set; }
            public string? cUsr_Nombres { get; set; }
            public int? nId_Grupo { get; set; }
            public string? cNombre_Grupo { get; set; }
            public DateTime? dUGrupo_FecIni { get; set; }
            public DateTime? dUGrupo_FecFin { get; set; }
            public bool? bEstado { get; set; }
            public bool? bActivo { get; set; }
            public bool? bGestion { get; set; }
        }
    }
}