using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Application.DTOs.Gestion
{
    public class GetGestionDireRequestDto
    {
        public GetGestionDireRequestDto()
        {
            nId_Cliente = 0;
            nId_Persdeudor = 0;
        }

        public int nId_Cliente { get; set; } //ID_CLIENTE
        public int nId_Persdeudor { get; set; } //ID_DEUDOR

        // 🔹 PAGINACIÓN
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > 50 ? 50 : value; // Máximo 50
        }
    }
}