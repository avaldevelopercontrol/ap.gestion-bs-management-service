using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Application.DTOs.Gestion
{
    public class GetGestionCabeceraAdicionalRequestDto
    {
        public GetGestionCabeceraAdicionalRequestDto()
        {
            nId_Cliente = 0;
            pantalla = 0;
        }

        public int nId_Cliente { get; set; }
        public int pantalla { get; set; }
    }
}