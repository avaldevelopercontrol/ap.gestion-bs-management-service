
namespace GesMgmt.Application.DTOs.Usuario
{
    public class UsuarioRequestDto
    {
        public class GetUsuarioLoginRequestDto
        {
            public string cUsr_Login { get; set; }
            public string cUsr_Pass { get; set; }
        }
    }
}