using GesMgmt.Application.DTOs;
using static GesMgmt.Application.DTOs.PerfilOpcion.PerfilOpcionRequestDto;
using static GesMgmt.Application.DTOs.PerfilOpcion.PerfilOpcionResponseDto;

namespace GesMgmt.Application.Interfaces.PerfilOpcion
{
    public interface IPerfilOpcionService
    {
        Task<ResultListaDto<IEnumerable<GetPerfilOpcionResponseDto>>> GetPerfilOptionsCountAsync();
        Task<ResultListaDto<IEnumerable<GetOpcionesPorPerfilResponseDto>>> GetOpcionesPorPerfilAsync(int nId_Perfil);
        Task<ResultDto<CreatePerfilOpcionResponseDto>> CreatePerfilOpcionAsync(CreatePerfilOpcionRequestDto perfilOpcionCreateDto);
        Task<ResultDto<EditPerfilOpcionResponseDto>> EditPerfilOpcionAsync(EditPerfilOpcionRequestDto perfilOpcionEditDto);
    }
}