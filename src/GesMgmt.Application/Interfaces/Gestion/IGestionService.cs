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
        Task<ResultListDto<IEnumerable<GestionCarteraDeudorHistoricaResponseDto>>> GetGestionGestionesCarteraDeudorHistoricasAsync(GestionCarteraDeudorHistoricaRequestDto gestionCarteraDeudorHisDto);
        Task<ResultListDto<IEnumerable<GetGestionGestionesCarteraDeudorResponseDto>>> GetGestionGestionesCarteraDeudorAsync(GetGestionGestionesCarteraDeudorRequestDto gestionCarteraDeudorDto);
        Task<ResultListDto<IEnumerable<GetGestionEstadoGestionCarteraDeudorResponseDto>>> GetGestionEstadosGestionesCarteraDeudorAsync(GetGestionEstadoGestionCarteraDeudorRequestDto gestionEstadosCarteraDeudorDto);
        Task<ResultListDto<IEnumerable<GestionCarteraDeudorEstadoHistoricaResponseDto>>> GetGestionEstadosGestionesCarteraDeudorHistoricaAsync(GestionCarteraDeudorEstadoHistoricaRequestDto gestionEstadosCarteraDeudorHistoricoDto);
        Task<ResultListDto<IEnumerable<GetGestionAgendaResponseDto>>> GetGestionAgendasDeudorAsync(GetGestionAgendaRequestDto gestionAgendaDto);
        Task<ResultListDto<IEnumerable<GetGestionPagosResponsetDto>>> GetGestionPagosDeudorAsync(GetGestionPagosRequestDto gestionPagosDto);
        Task<ResultDto<GetGestionInformacionDeudorRespondeDto>> GetGestionInformacionDeudorAsync(GetGestionInformacionDeudorRequestDto gestionInformacionDeudorDto);
        Task<ResultDto<GetGestionInformacionDeudorParamRespondeDto>> GetGestionInformacionDeudorParamAsync(GetGestionInformacionDeudorParamRequestDto gestionInformacionDeudorParamDto);
        Task<ResultListDto<IEnumerable<GetGestionTipoGestionResponseDto>>> GetGestionTipoGestionAsync();
        Task<ResultListDto<IEnumerable<GetGestionEstadoGestionResponseDto>>> GetGestionEstadoGestionAsync(GetGestionEstadoGestionRequestDto estadoGestionDto);
        Task<ResultListDto<IEnumerable<GetGestionPaletaRespuestaResponseDto>>> GetGestionPaletaRespuestaAsync(GetGestionPaletaRespuestaRequestDto paletaGestionDto);
        Task<ResultListDto<IEnumerable<GetGestionEstadoGestionClaroResponseDto>>> GetGestionEstadoGestionClaroAsync(GetGestionEstadoGestionClaroRequestDto estadoGestionClaroDto);
        Task<ResultListDto<IEnumerable<GetGestionMotivoNoPagoResponseDto>>> GetGestionMotivoNoPagoAsync(GetGestionMotivoNoPagoRequestDto motivoNoPagoDto);
        Task<ResultListDto<IEnumerable<CreateGestionOpeGesResponseDto>>> CreateGestionOpeGesContratosAsync(CreateGestionOpeGesRequestDto OpeGesCreateDto);
        //Task<ResultListDto<IEnumerable<GetGestionEstadoCuentaResponseDto>>> GetGestionEstadoCuentaAsync(GetGestionEstadoCuentaRequestDto estadoCuentaDto);
        Task<byte[]> ExportGestionEstadoCuentaAsync(GetGestionEstadoCuentaRequestDto dto);
    }
}