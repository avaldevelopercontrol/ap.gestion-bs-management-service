using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Application.DTOs.Perfil
{
    public class PerfilResponseDto
    {
        public class GetPerfilListResponseDto
        {
            public int nid_perfil { get; set; }
            public string per_Nombre { get; set; }
        }
    }
}