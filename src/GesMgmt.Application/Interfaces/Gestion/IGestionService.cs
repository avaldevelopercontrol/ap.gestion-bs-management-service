using GesMgmt.Application.DTOs;
using GesMgmt.Application.DTOs.Gestion;
using static GesMgmt.Application.DTOs.Gestion.GestionResponseDto;
using static GesMgmt.Application.DTOs.Gestion.GestionRequestDto;

namespace GesMgmt.Application.Interfaces.Gestion
{
    public interface IGestionService
    {
        Task<ResultDto<GetGestionZonaCarteraCampannaResponseDto>> GetGestionZonaCarteraCampannaAsync(GetGestionZonaCarteraCampannaRequestDto gestionZonaCartCamp);
        Task<ResultListCabeceraDto<IEnumerable<GetGestionCabeceraResponseDto>>> GetGestionDocumentosCabeceraAsync(GetGestionCabeceraRequestDto gestionCabeceraDto);
        Task<ResultListDto<IEnumerable<GetGestionDocumentoResponseDto>>> GetGestionDocumentosAsync(GetGestionDocumentoRequestDto suscriptionDto);
        Task<ResultDto<GetGestionCabeceraAdicionalResponseDto>> GetGestionDocumentosAdicionalesCabeceraAsync(GetGestionCabeceraAdicionalRequestDto gestionCabeceraAdicionalDto);
        Task<ResultListDto<IEnumerable<GetGestionAdicionalResponseDto>>> GetGestionDocumentosAdicionalesAsync(GetGestionAdicionalRequestDto gestionAdicionalDto);
        Task<ResultDto<GetGestionDeudorResponseDto>> GetGestionDeudorAsync(GetGestionDeudorRequestDto gestionDeudorDto);
        Task<ResultListDto<IEnumerable<GetGestionTelefonoResponseDto>>> GetGestionTelefonosAsync(GetGestionTelefonoRequestDto gestionTelefonoDto);
        Task<ResultListDto<IEnumerable<GetGestionDireccionResponseDto>>> GetGestionDireccionesAsync(GetGestionDireccionRequestDto gestionDireccionDto);
        Task<ResultListDto<IEnumerable<GestionCarteraDeudorHistoricaResponseDto>>> GetGestionGestionesCarteraDeudorHistoricasAsync(GestionCarteraDeudorHistoricaRequestDto gestionCarteraDeudorHisDto);
        Task<ResultListDto<IEnumerable<GetGestionGestionesCarteraDeudorResponseDto>>> GetGestionGestionesCarteraDeudorAsync(GetGestionGestionesCarteraDeudorRequestDto gestionCarteraDeudorDto);
        Task<ResultListDto<IEnumerable<GetGestionEstadoGestionCarteraDeudorResponseDto>>> GetGestionEstadosGestionesCarteraDeudorAsync(GetGestionEstadoGestionCarteraDeudorRequestDto gestionEstadosCarteraDeudorDto);
        Task<ResultListDto<IEnumerable<GestionCarteraDeudorEstadoHistoricaResponseDto>>> GetGestionEstadosGestionesCarteraDeudorHistoricaAsync(GestionCarteraDeudorEstadoHistoricaRequestDto gestionEstadosCarteraDeudorHistoricoDto);
        Task<ResultListDto<IEnumerable<GetGestionAgendaResponseDto>>> GetGestionAgendasDeudorAsync(GetGestionAgendaRequestDto gestionAgendaDto);
    }
}