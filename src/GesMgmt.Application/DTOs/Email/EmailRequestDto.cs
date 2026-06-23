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
    }
}