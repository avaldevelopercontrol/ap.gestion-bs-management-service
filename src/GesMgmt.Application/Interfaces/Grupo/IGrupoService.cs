using GesMgmt.Application.DTOs;
using static GesMgmt.Application.DTOs.Grupo.GrupoResponseDto;

namespace GesMgmt.Application.Interfaces.Grupo
{
    public interface IGrupoService
    {
        Task<ResultListaDto<IEnumerable<GetGrupoListResponseDto>>> GetGruposAsync();
    }
}
