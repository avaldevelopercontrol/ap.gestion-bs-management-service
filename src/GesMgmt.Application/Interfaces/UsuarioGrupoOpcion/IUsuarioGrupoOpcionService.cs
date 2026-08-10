using GesMgmt.Application.DTOs;
using static GesMgmt.Application.DTOs.UsuarioGrupoOpcion.UsuarioGrupoOpcionRequestDto;
using static GesMgmt.Application.DTOs.UsuarioGrupoOpcion.UsuarioGrupoOpcionResponseDto;

namespace GesMgmt.Application.Interfaces.UsuarioGrupoOpcion
{
    public interface IUsuarioGrupoOpcionService
    {
        Task<ResultListDto<IEnumerable<GetUsuarioGrupoOpcionListadoResponseDto>>> GetUsuarioGrupoOpcionListadoAsync(GetUsuarioGrupoOpcionListadoRequestDto usuarioGrupoOpcionDto);
        Task<ResultDto<GetUsuarioGrupoOpcionObtenerResponseDto>> GetUsuarioGrupoOpcionObtenerIdAsync(int nId_UsuarioGrupoOpcion);
        Task<ResultDto<PostUsuarioGrupoOpcionCrearResponseDto>> PostUsuarioGrupoOpcionCrearAsync(PostUsuarioGrupoOpcionCrearRequestDto usuarioGrupoOpcionCrearDto);
        Task<ResultDto<PutUsuarioGrupoOpcionModificarResponseDto>> PutUsuarioGrupoOpcionModificarAsync(PutUsuarioGrupoOpcionEditarRequestDto usuarioGrupoOpcionEditarDto);
    }
}