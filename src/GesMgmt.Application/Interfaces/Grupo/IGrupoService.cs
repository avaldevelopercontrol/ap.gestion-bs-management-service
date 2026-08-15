using GesMgmt.Application.DTOs;
using static GesMgmt.Application.DTOs.Grupo.GrupoRequestDto;
using static GesMgmt.Application.DTOs.Grupo.GrupoResponseDto;

namespace GesMgmt.Application.Interfaces.Grupo
{
    public interface IGrupoService
    {
        Task<ResultListaDto<IEnumerable<GetGrupoListResponseDto>>> GetGruposAsync();
        Task<ResultListaDto<IEnumerable<GetGruposResponseDto>>> GetGruposListadoAsync();
        Task<ResultDto<GetGrupoByIdResponseDto>> GetGrupoByIdAsync(int nId_Grupo);
        Task<ResultDto<CreateGrupoResponseDto>> CreateGrupoAsync(CreateGrupoRequestDto grupoCreateDto);
        Task<ResultDto<EditGrupoResponseDto>> EditGrupoAsync(EditGrupoRequestDto grupoEditDto);
        Task<ResultListDto<IEnumerable<GetGruposClienteInicialResponseDto>>> GetGruposClienteInicialAsync(int nId_Usuario);
    }
}