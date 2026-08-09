using GesMgmt.Application.DTOs;
using static GesMgmt.Application.DTOs.UGrupo.UGrupoRequestDto;
using static GesMgmt.Application.DTOs.UGrupo.UGrupoResponseDto;

namespace GesMgmt.Application.Interfaces.UGrupo
{
    public interface IUGrupoService
    {
        Task<ResultListDto<IEnumerable<GetUsuariosGrupoResponseDto>>> GetUsuariosGrupoAsync(GetUsuariosGrupoRequestDto usuarioGrupoDto);
        Task<ResultListDto<IEnumerable<GetGruposByUsuarioResponseDto>>> GetGruposByIdUsuarioAsync(GetGruposByUsuarioRequestDto usuarioGrupoDto);
        Task<ResultListDto<IEnumerable<GetGruposByUsuarioResponseDto>>> GetGruposFaltantesByIdUsuarioAsync(GetGruposByUsuarioRequestDto usuarioGrupoDto);
        Task<ResultListDto<IEnumerable<GetUsuarioGrupoListadoResponseDto>>> GetUsuarioGrupoListadoAsync(GetUsuarioGrupoListadoRequestDto uGrupoDto);
        Task<ResultDto<GetUsuarioGrupoObtenerResponseDto>> GetUsuarioGrupoObtenerIdAsync(int nId_UGrupo);
        Task<ResultDto<PostUsuarioGrupoCrearResponseDto>> PostUsuarioGrupoCrearAsync(PostUsuarioGrupoCrearRequestDto usuarioGrupoCrearDto);
        Task<ResultDto<PutUsuarioGrupoModificarResponseDto>> PutUsuarioGrupoModificarAsync(PutUsuarioGrupoModificarRequestDto usuarioGrupoModificarDto);
    }
}