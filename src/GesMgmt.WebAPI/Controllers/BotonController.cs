using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Boton;
using GesMgmt.Infraestructure.Logger;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using static GesMgmt.Application.DTOs.Boton.BotonRequestDto;
using static GesMgmt.Application.DTOs.Boton.BotonResponseDto;

namespace GesMgmt.WebAPI.Controllers
{
    [ApiController]
    [Route("v1/Boton")]
    [Produces("application/json")]
    public class BotonController : ControllerBase
    {
        private readonly IBotonService _botonService;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;
        private ValidationMessageDto _oValMsgDto;

        public BotonController(IBotonService botonService, IValidationMessageService validationMessageService, IAppLogger logger)
        {
            _botonService = botonService;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _Logger = logger;
            _Logger.LogInfo("| ** API.BS.GestionManagement ** |");
        }

        #region "LISTA DE BOTONES"
        /// <summary>
        /// Obtiene la Lista de los botones por Cliente y Contrato.
        /// </summary>
        /// <remarks>
        /// Obtiene la Lista de los botones por Cliente y Contrato.
        /// </remarks>
        /// <response code="200">Obtiene la Lista de los botones Por Cliente y Contrato.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Gestion Botones Por Cliente y Contrato")]
        [HttpGet("GetBotonesByClienteAndContrato")]
        [ProducesResponseType(typeof(ResultDto<GetGestionBotonesResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGestionBotonesAsync([FromQuery] GetGestionBotonesRequestDto gestionBotonesDto)
        {
            _Logger.LogInfo($"GetBotonesByClienteAndContrato|Begin|GetBotonesByClienteAndContratoAsync|request: {JsonSerializer.Serialize(gestionBotonesDto)}");
            var result = await _botonService.GetBotonesByClienteAndContratoAsync(gestionBotonesDto);
            _Logger.LogInfo($"GetBotonesByClienteAndContratoAsync|End|GetBotonesByClienteAndContratoAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }
        #endregion

        #region "BOTONES MAF"

        #region "+REPORTAR CASO - MAF"
        /// <summary>
        /// Lista de Reportar Casos: + REPORTAR CASO - CLIENTE MAF.
        /// </summary>
        /// <remarks>
        /// Lista de Reportar Casos: + REPORTAR CASO - CLIENTE MAF.
        /// </remarks>
        /// <response code="200">Lista de Reportar Casos: + REPORTAR CASO - CLIENTE MAF</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Lista de Reportar Casos: + REPORTAR CASO - CLIENTE MAF")]
        [HttpGet("GetReportarCasos")]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetReportarCasosResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetReportarCasosResponseDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetReportarCasosResponseDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetReportarCasosAsync([FromQuery] GetReportarCasosRequestDto reportarCasosDto)
        {
            _Logger.LogInfo($"GetReportarCasos|Begin|GetReportarCasosAsync|request: {JsonSerializer.Serialize(reportarCasosDto)}");
            var result = await _botonService.GetReportarCasosAsync(reportarCasosDto);
            _Logger.LogInfo($"GetReportarCasos|End|GetReportarCasosAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Obtiene el REPORTAR CASO - CLIENTE MAF.
        /// </summary>
        /// <remarks>
        /// Obtiene el REPORTAR CASO - CLIENTE MAF.
        /// </remarks>
        /// <response code="200">Obtiene el REPORTAR CASO - CLIENTE MAF.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Obtiene el REPORTAR CASO - CLIENTE MAF")]
        [HttpGet("GetReportarCasos/{nId_DocxCobrarOpeResult}")]
        [ProducesResponseType(typeof(ResultDto<GetReportarCasosByIdResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetReportarCasosByIdAsync(int nId_DocxCobrarOpeResult)
        {
            _Logger.LogInfo($"GetReportarCasosById|Begin|GetReportarCasosByIdAsync|request:{nId_DocxCobrarOpeResult}");
            var result = await _botonService.GetReportarCasosByIdAsync(nId_DocxCobrarOpeResult);
            _Logger.LogInfo($"GetReportarCasosById|End|GetReportarCasosByIdAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Crear registro de REPORTAR CASO - CLIENTE MAF.
        /// </summary>
        /// <remarks>
        /// Crear registro de REPORTAR CASO - CLIENTE MAF.
        /// </remarks>
        /// <response code="200">Crear registro de REPORTAR CASO - CLIENTE MAF.</response>
        [HttpPost("CreateReportarCasos")]
        [ProducesResponseType(typeof(ResultDto<CreateReportarCasosResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateReportarCasosAsync([FromBody] CreateReportarCasosRequestDto reportarCasoDto)
        {
            _Logger.LogInfo($"CreateReportarCasos|Begin|CreateReportarCasosAsync|request: {JsonSerializer.Serialize(reportarCasoDto)}");
            var result = await _botonService.CreateReportarCasosAsync(reportarCasoDto);
            _Logger.LogInfo($"CreateReportarCasos|End|CreateReportarCasosAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Editar registro de REPORTAR CASO - CLIENTE MAF.
        /// </summary>
        /// <remarks>
        /// Editar registro de REPORTAR CASO - CLIENTE MAF.
        /// </remarks>
        /// <response code="200">Editar registro de REPORTAR CASO - CLIENTE MAF.</response>
        [HttpPut("EditReportarCasos")]
        [ProducesResponseType(typeof(ResultDto<EditReportarCasosResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EditReportarCasosAsync([FromBody] EditReportarCasosRequestDto reportarCasoDto)
        {
            _Logger.LogInfo($"EditReportarCasos|Begin|EditReportarCasosAsync|request: {JsonSerializer.Serialize(reportarCasoDto)}");
            var result = await _botonService.EditReportarCasosAsync(reportarCasoDto);
            _Logger.LogInfo($"EditReportarCasos|End|EditReportarCasosAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }
        #endregion

        #region "+ADICIONAL MAF - MAF"
        /// <summary>
        /// Obtiene el ADICIONAL MAF - CLIENTE MAF.
        /// </summary>
        /// <remarks>
        /// Obtiene el ADICIONAL MAF - CLIENTE MAF.
        /// </remarks>
        /// <response code="200">Obtiene el ADICIONAL MAF - CLIENTE MAF.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Obtiene ADICIONAL MAF - CLIENTE MAF")]
        [HttpGet("GetOperativasMaf")]
        [ProducesResponseType(typeof(ResultDto<GetOperativasMafResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetOperativasMafAsync([FromQuery] GetOperativasMafRequestDto OperativasMafDto)
        {
            _Logger.LogInfo($"GetOperativasMaf|Begin|GetOperativasMafAsync|request:{OperativasMafDto}");
            var result = await _botonService.GetOperativasMafAsync(OperativasMafDto);
            _Logger.LogInfo($"GetOperativasMaf|End|GetOperativasMafAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }
        #endregion

        #endregion

        #region "BOTONES CLARO"

        #region "+ESTADO CUENTA - CLARO"
        /// <summary>
        /// Para poder realizar la descarga del archivo excel de estado de cuenta +ESTADO CUENTA - CLIENTE CLARO.
        /// </summary>
        /// <remarks>
        /// Para poder realizar la descarga del archivo excel de estado de cuenta +ESTADO CUENTA - CLIENTE CLARO.
        /// </remarks>
        /// <response code="200">Para poder realizar la descarga del archivo excel de estado de cuenta +ESTADO CUENTA - CLIENTE CLARO.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Para exportar el estado de cuenta +ESTADO CUENTA - CLIENTE CLARO")]
        [HttpGet("ExportGestionEstadoCuenta")]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetEstadoCuentaResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetEstadoCuentaResponseDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetEstadoCuentaResponseDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExportGestionEstadoCuentaAsync([FromQuery] GetEstadoCuentaRequestDto estadoCuentaDto)
        {
            _Logger.LogInfo($"ExportGestionEstadoCuenta|Begin|ExportGestionEstadoCuentaAsync|request: {JsonSerializer.Serialize(estadoCuentaDto)}");
            var excel = await _botonService.ExportGestionEstadoCuentaAsync(estadoCuentaDto);
            _Logger.LogInfo($"ExportGestionEstadoCuenta|End|ExportGestionEstadoCuentaAsync|response: {JsonSerializer.Serialize(excel)}");
            return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"EstadoCuenta_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }
        #endregion

        #endregion

        #region "BOTONES PUBLICOS"

        #region "+PAGOS - CLARO / MAF"
        /// <summary>
        /// Obtiene el listado de PAGOS, BOTÓN +PAGOS - CLIENTE CLARO.
        /// </summary>
        /// <remarks>
        /// Obtiene el listado de PAGOS, BOTÓN +PAGOS - CLIENTE CLARO.
        /// </remarks>
        /// <response code="200">Obtiene el listado de PAGOS, BOTÓN +PAGOS - CLIENTE CLARO.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado Obtiene el listado de PAGOS, BOTÓN +PAGOS - CLIENTE CLARO")]
        [HttpGet("GetPagosDeudor")]
        [ProducesResponseType(typeof(ResultDto<GetPagosResponsetDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPagosDeudorAsync([FromQuery] GetPagosRequestDto gestionPagoDto)
        {
            _Logger.LogInfo($"GetPagosDeudor|Begin|GetPagosDeudorAsync|request: {JsonSerializer.Serialize(gestionPagoDto)}");
            var result = await _botonService.GetPagosDeudorAsync(gestionPagoDto);
            _Logger.LogInfo($"GetPagosDeudor|End|GetPagosDeudorAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }
        #endregion

        #region "+EMAIL - CLARO / MAF"
        //ESTE METODO ESTA EN EL CONTROLLER DE EMAIL
        #endregion

        #region "+AGENDAS - CLARO / MAF"
        /// <summary>
        /// Obtiene el listado de AGENDAS, botón +AGENDAS - CLIENTE CLARO.
        /// </summary>
        /// <remarks>
        /// Obtiene el listado de AGENDAS, botón +AGENDAS - CLIENTE CLARO.
        /// </remarks>
        /// <response code="200">Obtiene el listado de AGENDAS, BOTÓN +AGENDAS - CLIENTE CLARO.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado Gestiones Agendadas - CLIENTE CLARO")]
        [HttpGet("GetAgendasDeudor")]
        [ProducesResponseType(typeof(ResultDto<GetAgendaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAgendasDeudorAsync([FromQuery] GetAgendaRequestDto gestionAgendaDto)
        {
            _Logger.LogInfo($"GetAgendasDeudor|Begin|GetAgendasDeudorAsync|request: {JsonSerializer.Serialize(gestionAgendaDto)}");
            var result = await _botonService.GetAgendasDeudorAsync(gestionAgendaDto);
            _Logger.LogInfo($"GetAgendasDeudor|End|GetAgendasDeudorAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }
        #endregion

        #region "+INF. DEUDOR - CLARO / MAF"
        /// <summary>
        /// Obtiene el listado de información del Deudor, BOTÓN +INF DEUDOR / FALSE = primer Registro / TRUE = segundo registro de la lista.
        /// </summary>
        /// <remarks>
        /// Obtiene el listado de información del Deudor, BOTÓN +INF DEUDOR / FALSE = primer Registro / TRUE = segundo registro de la lista.
        /// </remarks>
        /// <response code="200">Obtiene el listado de información del Deudor, BOTÓN +INF DEUDOR / FALSE = primer Registro / TRUE = segundo registro de la lista.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Obtener Cabecera de Información de Deudor")]
        [HttpGet("GetInformacionDeudor")]
        [ProducesResponseType(typeof(ResultDto<GetInformacionDeudorRespondeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetInformacionDeudorAsync([FromQuery] GetInformacionDeudorRequestDto gestionInfoDeudor)
        {
            _Logger.LogInfo($"GetInformacionDeudor|Begin|GetInformacionDeudorAsync|request: {JsonSerializer.Serialize(gestionInfoDeudor)}");
            var result = await _botonService.GetInformacionDeudorAsync(gestionInfoDeudor);
            _Logger.LogInfo($"GetInformacionDeudor|End|GetGestionInformacionDeudorAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Obtiene el listado de información del Deudor, BOTÓN +INF DEUDOR - CLIENTE CLARO / Tercer registro de la lista.
        /// </summary>
        /// <remarks>
        /// Obtiene el listado de información del Deudor, BOTÓN +INF DEUDOR - CLIENTE CLARO / Tercer registro de la lista.
        /// </remarks>
        /// <response code="200">Obtiene el listado de información del Deudor, BOTÓN +INF DEUDOR - CLIENTE CLARO / Tercer registro de la lista.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Obtener Cabecera de Información de Deudor Param - CLIENTE CLARO")]
        [HttpGet("GetInformacionDeudorParam")]
        [ProducesResponseType(typeof(ResultDto<GetInformacionDeudorParamRespondeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetInformacionDeudorParamAsync([FromQuery] GetInformacionDeudorParamRequestDto gestionInfoDeudorParam)
        {
            _Logger.LogInfo($"GetInformacionDeudorParam|Begin|GetInformacionDeudorParamAsync|request: {JsonSerializer.Serialize(gestionInfoDeudorParam)}");
            var result = await _botonService.GetInformacionDeudorParamAsync(gestionInfoDeudorParam);
            _Logger.LogInfo($"GetInformacionDeudorParam|End|GetInformacionDeudorParamAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }
        #endregion

        #endregion
    }
}