using GesMgmt.Application.DTOs;
using static GesMgmt.Application.DTOs.Opcion.OpcionRequestDto;
using static GesMgmt.Application.DTOs.Opcion.OpcionResponseDto;

namespace GesMgmt.Application.Interfaces.Opcion
{
    public interface IOpcionService
    {
        Task<ResultListaDto<IEnumerable<GetOpcionesResponseDto>>> GetOpcionesAsync();
        Task<ResultDto<GetOpcionByIdResponseDto>> GetOpcionByIdAsync(int nId_Opcion);
        Task<ResultDto<CreateOpcionResponseDto>> CreateOpcionAsync(CreateOpcionRequestDto opcionCreateDto);
        Task<ResultDto<EditOpcionResponseDto>> EditOpcionAsync(EditOpcionRequestDto opcionEditDto);
    }
}