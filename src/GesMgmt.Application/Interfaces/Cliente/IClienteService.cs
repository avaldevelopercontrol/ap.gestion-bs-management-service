using GesMgmt.Application.DTOs;
using static GesMgmt.Application.DTOs.Cliente.ClienteResponseDto;

namespace GesMgmt.Application.Interfaces.Cliente
{
    public interface IClienteService
    {
        Task<ResultListDto<IEnumerable<GetClientesActivosResponsetDto>>> GetClientesActivosAsync();
    }
}