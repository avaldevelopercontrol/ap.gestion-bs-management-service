using GesMgmt.Application.DTOs;
using static GesMgmt.Application.DTOs.Usuario.UsuarioRequestDto;
using static GesMgmt.Application.DTOs.Usuario.UsuarioResponseDto;

namespace GesMgmt.Application.Interfaces.Usuario
{
    public interface IUsuarioService
    {
        Task<ResultListDto<IEnumerable<GetUsuariosListResponseDto>>> GetUsuariosListAsync();
        Task<ResultDto<GetUsuarioLoginResponseDto>> GetLoginUsuarioAsync(GetUsuarioLoginRequestDto usuarioLoginDto);
        Task<ResultListDto<IEnumerable<GetUsuariosGrupoResponseDto>>> GetUsuariosGrupoAsync(GetUsuariosGrupoRequestDto usuarioGrupoDto);
        Task<ResultListDto<IEnumerable<GetGruposByUsuarioResponseDto>>> GetGruposByIdUsuarioAsync(GetGruposByUsuarioRequestDto usuarioGrupoDto);
        Task<ResultListDto<IEnumerable<GetGruposByUsuarioResponseDto>>> GetGruposFaltantesByIdUsuarioAsync(GetGruposByUsuarioRequestDto usuarioGrupoDto);
        Task<ResultListaDto<IEnumerable<GetSubZonaGeneralListResponseDto>>> GetSubZonasGeneralAsync();
        Task<ResultListDto<IEnumerable<GetCampannaDiscadorlListResponseDto>>> GetCampannaDiscadorByIdUsuarioAsync(GetCampannaDiscadorlListRequestDto camannaDiscadorDto);
        Task<ResultDto<CreateUsuarioResponseDto>> CreateUsuarioAsync(CreateUsuarioRequestDto usuarioCreateDto);
        Task<ResultDto<EditUsuarioResponseDto>> EditUsuarioAsync(EditUsuarioRequestDto usuarioEditDto);
    }
}