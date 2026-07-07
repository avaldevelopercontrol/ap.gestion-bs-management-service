using GesMgmt.Application.DTOs;
using static GesMgmt.Application.DTOs.Usuario.UsuarioRequestDto;
using static GesMgmt.Application.DTOs.Usuario.UsuarioResponseDto;

namespace GesMgmt.Application.Interfaces.Usuario
{
    public interface IUsuarioService
    {
        Task<ResultDto<GetUsuarioLoginResponseDto>> GetLoginUsuarioAsync(GetUsuarioLoginRequestDto usuarioLoginDto);
        Task<ResultListDto<IEnumerable<GetUsuariosGrupoResponseDto>>> GetUsuariosGrupoAsync(GetUsuariosGrupoRequestDto usuarioGrupoDto);
    }
}