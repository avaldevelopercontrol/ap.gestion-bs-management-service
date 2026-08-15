using GesMgmt.Application.DTOs;
using static GesMgmt.Application.DTOs.Usuario.UsuarioRequestDto;
using static GesMgmt.Application.DTOs.Usuario.UsuarioResponseDto;

namespace GesMgmt.Application.Interfaces.Usuario
{
    public interface IUsuarioService
    {
        Task<ResultListDto<IEnumerable<GetUsuariosListResponseDto>>> GetUsuariosListAsync();
        Task<ResultDto<GetUsuarioObtenerResponseDto>> GetUsuarioByIdAsync(int nId_Usuario);
        Task<ResultDto<GetUsuarioLoginResponseDto>> GetLoginUsuarioAsync(GetUsuarioLoginRequestDto usuarioLoginDto);
        Task<ResultListaDto<IEnumerable<GetSubZonaGeneralListResponseDto>>> GetSubZonasGeneralAsync();
        Task<ResultListDto<IEnumerable<GetCampannaDiscadorlListResponseDto>>> GetCampannaDiscadorByIdUsuarioAsync(GetCampannaDiscadorlListRequestDto camannaDiscadorDto);
        Task<ResultDto<CreateUsuarioResponseDto>> CreateUsuarioAsync(CreateUsuarioRequestDto usuarioCreateDto);
        Task<ResultDto<EditUsuarioResponseDto>> EditUsuarioAsync(EditUsuarioRequestDto usuarioEditDto);
        Task<ResultDto<ResetearUsuarioResponseDto>> ResetearClaveUsuarioAsync(ResetearUsuarioRequestDto usuarioResetDto);
    }
}