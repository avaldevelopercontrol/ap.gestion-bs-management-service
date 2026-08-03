using GesMgmt.Application.DTOs;
using static GesMgmt.Application.DTOs.PerfilOpcion.PerfilOpcionResponseDto;

namespace GesMgmt.Application.Interfaces.PerfilOpcion
{
    public interface IPerfilOpcionService
    {
        Task<ResultListaDto<IEnumerable<GetPerfilOpcionResponseDto>>> GetPerfilOpcionesAsync();
    }
}