using GesMgmt.Application.DTOs;
using static GesMgmt.Application.DTOs.Deudor.DeudorRequestDto;
using static GesMgmt.Application.DTOs.Deudor.DeudorResponseDto;

namespace GesMgmt.Application.Interfaces.Deudor
{
    public interface IDeudorService
    {
        Task<ResultListDto<IEnumerable<GetDeudorResponseDto>>> GetDeudorAsync(GetDeudorRequestDto deudorDto);
    }
}