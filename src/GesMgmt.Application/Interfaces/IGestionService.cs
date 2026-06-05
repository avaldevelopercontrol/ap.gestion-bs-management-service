using GesMgmt.Application.DTOs;
using GesMgmt.Application.DTOs.Gestion;
using GesMgmt.Domain.Entities;

namespace GesMgmt.Application.Interfaces
{
    public interface IGestionService
    {
        Task<ResultListCabeceraDto<IEnumerable<GetGestionCabeceraResponseDto>>> GetGestionesCabeceraAsync(GetGestionCabeceraRequestDto gestionCabeceraDto);
        Task<ResultListDto<IEnumerable<GetGestionResponseDto>>> GetGestionesAsync(GetGestionRequestDto suscriptionDto);
        Task<ResultDto<GetGestionCabeceraAdicionalResponseDto>> GetGestionesCabeceraAdicionalesAsync(GetGestionCabeceraAdicionalRequestDto gestionCabeceraAdicionalDto);
        Task<ResultListDto<IEnumerable<GetGestionAdicionalResponseDto>>> GetGestionesAdicionalesAsync(GetGestionAdicionalRequestDto gestionAdicionalDto);
        Task<ResultDto<GetGestionDeudorResponseDto>> GetGestionesDeudorAsync(GetGestionDeudorRequestDto gestionDeudorDto);
        Task<ResultListDto<IEnumerable<GetGestionTelefonoResponseDto>>> GetTelefonoGestionAsync(GetGestionTelefonoRequestDto gestionTelefonoDto);
        Task<ResultListDto<IEnumerable<GetGestionDireccionResponseDto>>> GetGestionDireccionesAsync(GetGestionDireccionRequestDto gestionDireccionDto);
    }
}