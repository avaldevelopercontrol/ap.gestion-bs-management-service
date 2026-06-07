using GesMgmt.Application.DTOs;
using GesMgmt.Application.DTOs.Gestion;
using GesMgmt.Domain.Entities;

namespace GesMgmt.Application.Interfaces
{
    public interface IGestionService
    {
        Task<ResultListCabeceraDto<IEnumerable<GetGestionCabeceraResponseDto>>> GetGestionDocumentosCabeceraAsync(GetGestionCabeceraRequestDto gestionCabeceraDto);
        Task<ResultListDto<IEnumerable<GetGestionDocumentoResponseDto>>> GetGestionDocumentosAsync(GetGestionDocumentoRequestDto suscriptionDto);
        Task<ResultDto<GetGestionCabeceraAdicionalResponseDto>> GetGestionDocumentosAdicionalesCabeceraAsync(GetGestionCabeceraAdicionalRequestDto gestionCabeceraAdicionalDto);
        Task<ResultListDto<IEnumerable<GetGestionAdicionalResponseDto>>> GetGestionDocumentosAdicionalesAsync(GetGestionAdicionalRequestDto gestionAdicionalDto);
        Task<ResultDto<GetGestionDeudorResponseDto>> GetGestionDeudorAsync(GetGestionDeudorRequestDto gestionDeudorDto);
        Task<ResultListDto<IEnumerable<GetGestionTelefonoResponseDto>>> GetGestionTelefonosAsync(GetGestionTelefonoRequestDto gestionTelefonoDto);
        Task<ResultListDto<IEnumerable<GetGestionDireccionResponseDto>>> GetGestionDireccionesAsync(GetGestionDireccionRequestDto gestionDireccionDto);
        Task<ResultListDto<IEnumerable<GetGestionGestionesCarteraDeudorResponseDto>>> GetGestionGestionesCarteraDeudorAsync(GetGestionGestionesCarteraDeudorRequestDto gestionCarteraDeudorDto);
    }
}