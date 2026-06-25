using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Application.DTOs.Deudor
{
    public class DeudorRequestDto
    {
        public class GetDeudorRequestDto
        {
            public int nId_Cliente { get; set; }
            public string busqueda { get; set; }
            // 🔹 PAGINACIÓN
            public int PageNumber { get; set; } = 1;
            private int _pageSize = 10;
            public int PageSize
            {
                get => _pageSize;
                set => _pageSize = value > 1000 ? 1000 : value; // Máximo 1000
            }
        }
    }
}