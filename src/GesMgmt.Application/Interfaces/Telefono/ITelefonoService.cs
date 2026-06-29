using GesMgmt.Application.DTOs;
using static GesMgmt.Application.DTOs.Telefono.TelefonoRequestDto;
using static GesMgmt.Application.DTOs.Telefono.TelefonoResponseDto;

namespace GesMgmt.Application.Interfaces.Telefono
{
    public interface ITelefonoService
    {
        Task<ResultListDto<IEnumerable<GetTelefonosResponseDto>>> GetTelefonosAsync(GetTelefonosRequestDto gestionTelefonosDto);
        Task<ResultDto<GetTelefonoAsync>> GetTelefonoByIdTelefonoAsync(int nId_PersTelef);
        Task<ResultListaDto<IEnumerable<GetTelefonoResultados>>> GetTelefonoResultadosAsync();
        Task<ResultListaDto<IEnumerable<GetTelefonoOperadores>>> GetTelefonoOperadoresAsync();
        Task<ResultListaDto<IEnumerable<GetTelefonoUbicaciones>>> GetTelefonoUbicacionesAsync();
        Task<ResultListaDto<IEnumerable<GetTelefonoHorarioGestion>>> GetTelefonoHorarioGestionAsync();
        Task<ResultListaDto<IEnumerable<GetTelefonoFuenteBusqueda>>> GetTelefonoFuenteBusquedaAsync();
        Task<ResultDto<CreateTelefonoResponseDto>> CreateTelefonoAsync(CreateTelefonoRequestDto telefonoDto);
        Task<ResultDto<EditTelefonoResponseDto>> EditTelefonoAsync(EditTelefonoRequestDto telefonoEditDto);
    }
}