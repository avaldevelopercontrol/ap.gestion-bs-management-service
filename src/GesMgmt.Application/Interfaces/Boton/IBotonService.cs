using GesMgmt.Application.DTOs;
using static GesMgmt.Application.DTOs.Boton.BotonRequestDto;
using static GesMgmt.Application.DTOs.Boton.BotonResponseDto;

namespace GesMgmt.Application.Interfaces.Boton
{
    public interface IBotonService
    {
        Task<ResultListDto<IEnumerable<GetGestionBotonesResponseDto>>> GetBotonesByClienteAndContratoAsync(GetGestionBotonesRequestDto gestionBotonesDto);

        #region "BOTONES MAF"

        #region "+REPORTAR CASO - MAF"
        Task<ResultListDto<IEnumerable<GetReportarCasosResponseDto>>> GetReportarCasosAsync(GetReportarCasosRequestDto gestionZonaCartCamp);
        Task<ResultDto<GetReportarCasosByIdResponseDto>> GetReportarCasosByIdAsync(int nId_DocxCobrarOpeResult);
        Task<ResultDto<CreateReportarCasosResponseDto>> CreateReportarCasosAsync(CreateReportarCasosRequestDto reportarCasosCreateDto);
        Task<ResultDto<EditReportarCasosResponseDto>> EditReportarCasosAsync(EditReportarCasosRequestDto reportarCasosUpdateDto);
        #endregion

        #region "+ADICIONAL MAF - MAF"
        Task<ResultDto<GetOperativasMafResponseDto>> GetOperativasMafAsync(GetOperativasMafRequestDto request);
        #endregion

        #endregion

        #region "BOTONES CLARO"

        #region "+ESTADO CUENTA - CLARO"
        Task<byte[]> ExportGestionEstadoCuentaAsync(GetEstadoCuentaRequestDto dto);
        #endregion
        
        #endregion

        #region "BOTONES PUBLICOS"

        #region "+PAGOS - CLARO / MAF"
        Task<ResultListDto<IEnumerable<GetPagosResponsetDto>>> GetPagosDeudorAsync(GetPagosRequestDto gestionPagosDto);
        #endregion

        #region "+EMAIL - CLARO / MAF"
        //ESTE METODO ESTA EN EL CONTROLLER DE EMAIL
        #endregion

        #region "+AGENDA - CLARO / MAF"
        Task<ResultListDto<IEnumerable<GetAgendaResponseDto>>> GetAgendasDeudorAsync(GetAgendaRequestDto gestionAgendaDto);
        #endregion

        #region "+INF. DEUDOR - CLARO / MAF"
        Task<ResultDto<GetInformacionDeudorRespondeDto>> GetInformacionDeudorAsync(GetInformacionDeudorRequestDto gestionInformacionDeudorDto);
        Task<ResultDto<GetInformacionDeudorParamRespondeDto>> GetInformacionDeudorParamAsync(GetInformacionDeudorParamRequestDto gestionInformacionDeudorParamDto);
        #endregion

        #endregion
    }
}