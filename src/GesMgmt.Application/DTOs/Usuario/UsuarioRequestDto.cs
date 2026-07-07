
namespace GesMgmt.Application.DTOs.Usuario
{
    public class UsuarioRequestDto
    {
        public class GetUsuarioLoginRequestDto
        {
            public string cUsr_Login { get; set; }
            public string cUsr_Pass { get; set; }
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