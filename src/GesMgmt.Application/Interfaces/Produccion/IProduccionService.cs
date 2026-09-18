using GesMgmt.Application.DTOs;
using static GesMgmt.Application.DTOs.Produccion.ProduccionRequestDto;
using static GesMgmt.Application.DTOs.Produccion.ProduccionResponseDto;

namespace GesMgmt.Application.Interfaces.Produccion
{
    public interface IProduccionService
    {
        Task<ResultListDto<IEnumerable<GetProvinciasProduccionResponseDto>>> GetProvinciasProduccionAsync();
        Task<ResultListDto<IEnumerable<GetClientesProduccionActivosResponsetDto>>> GetClientesProduccionActivosAsync();
        Task<ResultListDto<IEnumerable<GetPerfilesActivosResponsetDto>>> GetPerfilesProduccionAsync();
        Task<ResultListDto<IEnumerable<GetProduccionResumenResponseDto>>> GetProduccionResumenAsync(GetProduccionResumenRequestDto request);
    }
}