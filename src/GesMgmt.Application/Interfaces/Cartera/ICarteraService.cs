using GesMgmt.Application.DTOs;
using static GesMgmt.Application.DTOs.Cartera.CarteraResponseDto;

namespace GesMgmt.Application.Interfaces.Cartera
{
    public interface ICarteraService
    {
        Task<ResultListDto<IEnumerable<GetAnioByIdClienteResponseDto>>> GetAnioByIdClienteAsync(int nId_Cliente);
        Task<ResultListDto<IEnumerable<GetCarterasParametrosByIdClienteAnnioResponseDto>>> GetCarterasParametrosByIdClienteAnnioAsync(int nId_Cliente, int anio);
    }
}
