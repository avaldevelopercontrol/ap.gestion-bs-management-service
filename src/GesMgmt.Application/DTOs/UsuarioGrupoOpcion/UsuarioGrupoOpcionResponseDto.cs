using System;
using System.Collections.Generic;
using System.Text;

namespace GesMgmt.Application.DTOs.UsuarioGrupoOpcion
{
    public class UsuarioGrupoOpcionResponseDto
    {
        public class GetUsuarioGrupoOpcionListadoRequestDto
        {

            // 🔹 PAGINACIÓN
            public int PageNumber { get; set; } = 1;
            private int _pageSize = 10;
            public int PageSize
            {
                get => _pageSize;
                set => _pageSize = value > 1000 ? 1000 : value; // Máximo 50
            }
        }

        public class PostUsuarioGrupoOpcionCrearResponseDto
        {
            public int nId_UsuarioGrupoOpcion { get; set; }
            public int? nId_Usuario { get; set; }
            public int? nId_Grupo { get; set; }
            public int? nId_Opcion { get; set; }
        }

        public class PutUsuarioGrupoOpcionModificarResponseDto
        {
            public int nId_UsuarioGrupoOpcion { get; set; }
            public int? nId_Usuario { get; set; }
            public int? nId_Grupo { get; set; }
            public int? nId_Opcion { get; set; }
        }

    }
}