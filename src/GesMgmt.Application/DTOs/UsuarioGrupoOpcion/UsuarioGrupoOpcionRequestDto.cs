
using GesMgmt.Domain.Entities;

namespace GesMgmt.Application.DTOs.UsuarioGrupoOpcion
{
    public class UsuarioGrupoOpcionRequestDto
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

        public class GetByIdUsuarioIdGrupoAsyncRequestDto
        {
            public int nId_Usuario { get; set; }
            public int nId_Grupo { get; set; }
            // 🔹 PAGINACIÓN
            public int PageNumber { get; set; } = 1;
            private int _pageSize = 10;
            public int PageSize
            {
                get => _pageSize;
                set => _pageSize = value > 1000 ? 1000 : value; // Máximo 50
            }
        }

        public class PostUsuarioGrupoOpcionCrearRequestDto
        {
            public int nId_Usuario { get; set; }
            public int nId_Grupo { get; set; }
            public int nId_Opcion { get; set; }
            public bool? bConsultar { get; set; }
            public bool? bInsertar { get; set; }
            public bool? bEditar { get; set; }
            public bool? bEliminar { get; set; }
            public bool? bExportar { get; set; }
            public bool bEstado { get; set; }
            public int nCrea { get; set; }
            public DateTime dFechaCrea { get; set; }
        }

        public class PutUsuarioGrupoOpcionEditarRequestDto
        {
            public int nId_UsuarioGrupoOpcion { get; set; }
            public int nId_Usuario { get; set; }
            public int nId_Grupo { get; set; }
            public int nId_Opcion { get; set; }
            public bool? bConsultar { get; set; }
            public bool? bInsertar { get; set; }
            public bool? bEditar { get; set; }
            public bool? bEliminar { get; set; }
            public bool? bExportar { get; set; }
            public bool bEstado { get; set; }
            public int nCrea { get; set; }
            public DateTime dFechaCrea { get; set; }
            public int nModifica { get; set; }
            public DateTime dFechaModifica { get; set; }
        }
    }
}