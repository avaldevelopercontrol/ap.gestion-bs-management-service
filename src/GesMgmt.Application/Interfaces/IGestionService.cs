using GesMgmt.Application.DTOs;
using GesMgmt.Domain.Entities;

namespace GesMgmt.Application.Interfaces
{
    public interface IGestionService
    {
        Task<ResultListCabeceraDto<IEnumerable<GetGestionCabeceraResponseDto>>> GetCabeceraGestionesAsync(GetGestionCabeceraRequestDto gestionCabeceraDto);
        Task<ResultListDto<IEnumerable<GetGestionResponseDto>>> GetGestionesAsync(GetGestionRequestDto suscriptionDto);
        Task<ResultDto<GetGestionCabeceraAdicionalResponseDto>> GetCabeceraGestionesAdicionalesAsync(GetGestionCabeceraAdicionalRequestDto gestionCabeceraAdicionalDto);
        Task<ResultListDto<IEnumerable<GetGestionAdicionalResponseDto>>> GetGestionesAdicionalesAsync(GetGestionAdicionalRequestDto gestionAdicionalDto);
        Task<ResultDto<GetDeudorResponseDto>> GetDeudorGestionAsync(GetDeudorRequestDto gestionDeudorDto);
        Task<ResultListDto<IEnumerable<GetTelefonoResponseDto>>> GetTelefonoGestionAsync(GetTelefonoRequestDto gestionTelefonoDto);
    }
}