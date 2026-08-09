

namespace GesMgmt.Application.DTOs.UGrupo
{
    public class UGrupoRequestDto
    {
        public class GetUsuarioGrupoListadoRequestDto
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

        public class GetUsuarioGrupoObtenerRequestDto
        {
            public int nId_UGrupo { get; set; }
        }

        public class PostUsuarioGrupoCrearRequestDto
        {
            public int? nId_Usuario { get; set; }
            public int? nId_Grupo { get; set; }
            public DateTime? dUGrupo_FecIni { get; set; }
            public DateTime? dUGrupo_FecFin { get; set; }
            public bool? bEstado { get; set; }
            public bool? bActivo { get; set; }
            public bool? bGestion { get; set; }
        }

        public class PutUsuarioGrupoModificarRequestDto
        {
            public int nId_UGrupo { get; set; }
            public int? nId_Usuario { get; set; }
            public int? nId_Grupo { get; set; }
            public DateTime? dUGrupo_FecIni { get; set; }
            public DateTime? dUGrupo_FecFin { get; set; }
            public bool? bEstado { get; set; }
            public bool? bActivo { get; set; }
            public bool? bGestion { get; set; }
        }

        public class GetGruposByUsuarioRequestDto
        {
            public int nId_Usuario { get; set; }

            public int PageNumber { get; set; } = 1;
            private int _pageSize = 10;
            public int PageSize
            {
                get => _pageSize;
                set => _pageSize = value > 1000 ? 1000 : value; // Máximo 1000
            }
        }

        public class GetUsuariosGrupoRequestDto
        {
            public int nId_Cliente { get; set; }

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