using GesMgmt.Application.DTOs;
using GesMgmt.Application.DTOs.Gestion;
using GesMgmt.Domain.Entities;

namespace GesMgmt.Application.Interfaces
{
    public interface IGestionService
    {
        Task<ResultListCabeceraDto<IEnumerable<GetGestionCabeResponseDto>>> GetGestionDocumentosCabeceraAsync(GetGestionCabeRequestDto gestionCabeceraDto);
        Task<ResultListDto<IEnumerable<GetGestionDocuResponseDto>>> GetGestionDocumentosAsync(GetGestionDocuRequestDto suscriptionDto);
        Task<ResultDto<GetGestionCabeAdicResponseDto>> GetGestionDocumentosAdicionalesCabeceraAsync(GetGestionCabeAdicRequestDto gestionCabeceraAdicionalDto);
        Task<ResultListDto<IEnumerable<GetGestionAdicResponseDto>>> GetGestionDocumentosAdicionalesAsync(GetGestionAdicRequestDto gestionAdicionalDto);
        Task<ResultDto<GetGestionDeudResponseDto>> GetGestionDeudorAsync(GetGestionDeudRequestDto gestionDeudorDto);
        Task<ResultListDto<IEnumerable<GetGestionTeleResponseDto>>> GetGestionTelefonosAsync(GetGestionTeleRequestDto gestionTelefonoDto);
        Task<ResultListDto<IEnumerable<GetGestionDireResponseDto>>> GetGestionDireccionesAsync(GetGestionDireRequestDto gestionDireccionDto);
        Task<ResultListDto<IEnumerable<GetGestionGestCartDeudResponseDto>>> GetGestionGestionesCarteraDeudorAsync(GetGestionGestCartDeudRequestDto gestionCarteraDeudorDto);
        Task<ResultListDto<IEnumerable<GetGestionEstaGestCartDeudResponseDto>>> GetGestionEstadosGestionesCarteraDeudorAsync(GetGestionEstaGestCartDeudRequestDto gestionEstadosCarteraDeudorDto);
    }
}