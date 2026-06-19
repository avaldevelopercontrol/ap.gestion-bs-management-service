using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Application.DTOs.Gestion
{
    public class GetGestionZonaCartCampRequestDto
    {
        public GetGestionZonaCartCampRequestDto()
        {
            nId_Cliente = 0;
            nId_Cartera = 0;
        }

        public int nId_Cliente { get; set; }
        public int nId_Cartera { get; set; }
    }
}