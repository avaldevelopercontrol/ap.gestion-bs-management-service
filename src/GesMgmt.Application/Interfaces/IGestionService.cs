using GesMgmt.Application.DTOs;
using GesMgmt.Domain.Entities;

namespace GesMgmt.Application.Interfaces
{
    public interface IGestionService
    {
        Task<ResultListDto<IEnumerable<GetGestionResponseDto>>> GetGestionesAsync(GetGestionRequestDto suscriptionDto);
        Task<ResultListCabeceraDto<IEnumerable<GetGestionCabeceraResponseDto>>> GetCabeceraGestionesAsync(GetGestionCabeceraRequestDto gestionCabeceraDto);
        Task<ResultDto<GetDeudorResponseDto>> GetDeudorGestionAsync(GetDeudorRequestDto gestionDeudorDto);
    }
}