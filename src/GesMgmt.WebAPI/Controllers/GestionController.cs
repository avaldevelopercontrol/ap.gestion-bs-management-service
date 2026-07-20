using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Gestion;
using GesMgmt.Infraestructure.Logger;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using static GesMgmt.Application.DTOs.Gestion.GestionRequestDto;
using static GesMgmt.Application.DTOs.Gestion.GestionResponseDto;

namespace GesMgmt.WebAPI.Controllers
{
    [ApiController]
    [Route("v1/Gestion")]
    [Produces("application/json")]
    public class GestionController : ControllerBase
    {
        private readonly IGestionService _gestionService;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;
        private ValidationMessageDto _oValMsgDto;

        public GestionController(IGestionService gestionService, IValidationMessageService validationMessageService, IAppLogger logger)
        {
            _gestionService = gestionService;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _Logger = logger;
            _Logger.LogInfo("| ** API.BS.GestionManagement ** |");
        }

        /// <summary>
        /// Obtiene la información de ZONA / CARTERA / CAMPAÑA para la cabecera de la pantalla.
        /// </summary>
        /// <remarks>
        /// Obtiene la información de ZONA / CARTERA / CAMPAÑA para la cabecera de la pantalla.
        /// </remarks>
        /// <response code="200">Obtiene la información de ZONA / CARTERA / CAMPAÑA para la cabecera de la pantalla</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Obtener Información Zona - Cartera - Campanna")]
        [HttpGet("GetGestionZonaCarteraCampanna")]
        [ProducesResponseType(typeof(ResultDto<GetGestionZonaCarteraCampannaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGestionZonaCarteraCampannaAsync([FromQuery] GetGestionZonaCarteraCampannaRequestDto gestionZonaCartCamp)
        {
            _Logger.LogInfo($"GetGestionZonaCarteraCampanna|Begin|GetGestionZonaCarteraCampannaAsync|request: {JsonSerializer.Serialize(gestionZonaCartCamp)}");
            var result = await _gestionService.GetGestionZonaCarteraCampannaAsync(gestionZonaCartCamp);
            _Logger.LogInfo($"GetGestionZonaCarteraCampanna|End|GetGestionZonaCarteraCampannaAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        /// <summary>
        /// Obtiene el listado que se usa para armar la cabecera de las gestiones de Documentos por cobrar, ordenado por la columna ORDEN.
        /// </summary>
        /// <remarks>
        /// Obtiene el listado que se usa para armar la cabecera de las gestiones de Documentos por cobrar, ordenado por la columna ORDEN.
        /// </remarks>
        /// <response code="200">Obtiene el listado que se usa para armar la cabecera de las gestiones de Documentos por cobrar, ordenado por la columna ORDEN.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado Gestiones Documentos Cabecera")]
        [HttpGet("GetGestionDocumentosCabecera")]
        [ProducesResponseType(typeof(ResultDto<GetGestionCabeceraResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGestionDocumentosCabeceraAsync([FromQuery] GetGestionCabeceraRequestDto gestionCabeceraDto)
        {
            _Logger.LogInfo($"GetGestionCabecera|Begin|GetGestionDocumentosCabeceraAsync|request: {JsonSerializer.Serialize(gestionCabeceraDto)}");
            var result = await _gestionService.GetGestionDocumentosCabeceraAsync(gestionCabeceraDto);
            _Logger.LogInfo($"GetGestionCabecera|End|GetGestionDocumentosCabeceraAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        /// <summary>
        /// Obtiene el listado de documentos por cobrar av_docxobrar.
        /// </summary>
        /// <remarks>
        /// Obtiene el listado de documentos por cobrar av_docxobrar.
        /// </remarks>
        /// <response code="200">Obtiene el listado de documentos por cobrar av_docxobrar.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado Gestiones Documentos")]
        [HttpGet("GetGestionDocumentos")]
        [ProducesResponseType(typeof(ResultDto<GetGestionDocumentoResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGestionDocumentosAsync([FromQuery] GetGestionDocumentoRequestDto gestionDto)
        {
            _Logger.LogInfo($"GetGestionDocumentos|Begin|GetGestionesDocumentosAsync|request: {JsonSerializer.Serialize(gestionDto)}");
            var result = await _gestionService.GetGestionDocumentosAsync(gestionDto);
            _Logger.LogInfo($"GetGestionDocumentos|End|GetGestionesDocumentosAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        /// <summary>
        /// Obtiene la información del deudor de la cabecera de la pantalla gestión.
        /// </summary>
        /// <remarks>
        /// Obtiene la información del deudor de la cabecera de la pantalla gestión.
        /// </remarks>
        /// <response code="200">Obtiene la información del deudor de la cabecera de la pantalla gestión.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Gestion Deudor")]
        [HttpGet("GetGestionDeudor")]
        [ProducesResponseType(typeof(ResultDto<GetGestionDeudorResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGestionDeudorAsync([FromQuery] GetGestionDeudorRequestDto gestionDeudorDto)
        {
            _Logger.LogInfo($"GetGestionDeudor|Begin|GetGestionDeudorAsync|request: {JsonSerializer.Serialize(gestionDeudorDto)}");
            var result = await _gestionService.GetGestionDeudorAsync(gestionDeudorDto);
            _Logger.LogInfo($"GetGestionDeudor|End|GetGestionDeudorAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        /// <summary>
        /// Obtiene el listado con las columnas para la cabecera de DATOS ADICIONALES.
        /// </summary>
        /// <remarks>
        /// Obtiene el listado con las columnas para la cabecera de DATOS ADICIONALES.
        /// </remarks>
        /// <response code="200">Obtiene el listado con las columnas para la cabecera de DATOS ADICIONALES.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado Cabecera Gestiones Documentos Adicionales")]
        [HttpGet("GetGestionDocumentosAdicionalesCabecera")]
        [ProducesResponseType(typeof(ResultDto<GetGestionCabeceraAdicionalResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGestionDocumentosAdicionalesCabeceraAsync([FromQuery] GetGestionCabeceraAdicionalRequestDto gestionCabeceraAdicionalDto)
        {
            _Logger.LogInfo($"GetGestionDocumentosAdicionalesCabecera|Begin|GetGestionDocumentosAdicionalesCabeceraAsync|request: {JsonSerializer.Serialize(gestionCabeceraAdicionalDto)}");
            var result = await _gestionService.GetGestionDocumentosAdicionalesCabeceraAsync(gestionCabeceraAdicionalDto);
            _Logger.LogInfo($"GetGestionDocumentosAdicionalesCabecera|End|GetGestionDocumentosAdicionalesCabeceraAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        /// <summary>
        /// Obtiene el listado de DATOS ADICIONALES.
        /// </summary>
        /// <remarks>
        /// Obtiene el listado de DATOS ADICIONALES.
        /// </remarks>
        /// <response code="200">Obtiene el listado con las columnas para la cabecera de DATOS ADICIONALES.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado Gestiones Documentos Adicionales")]
        [HttpGet("GetGestionDocumentosAdicionales")]
        [ProducesResponseType(typeof(ResultDto<GetGestionAdicionalResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGestionDocumentosAdicionalesAsync([FromQuery] GetGestionAdicionalRequestDto gestionAdicionalDto)
        {
            _Logger.LogInfo($"GetGestionDocumentosAdicionales|Begin|GetGestionDocumentosAdicionalesAsync|request: {JsonSerializer.Serialize(gestionAdicionalDto)}");
            var result = await _gestionService.GetGestionDocumentosAdicionalesAsync(gestionAdicionalDto);
            _Logger.LogInfo($"GetGestionDocumentosAdicionales|End|GetGestionDocumentosAdicionalesAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }
        
        /// <summary>
        /// Obtiene el listado de GESTIONES del deudor.
        /// </summary>
        /// <remarks>
        /// Obtiene el listado de GESTIONES del deudor.
        /// </remarks>
        /// <response code="200">Obtiene el listado de GESTIONES del deudor.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado Gestiones")]
        [HttpGet("GetGestionGestionesCarteraDeudor")]
        [ProducesResponseType(typeof(ResultDto<GetGestionGestionesCarteraDeudorResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGestionGestionesCarteraDeudorAsync([FromQuery] GetGestionGestionesCarteraDeudorRequestDto gestionCarteraDeudorDto)
        {
            _Logger.LogInfo($"GetGestionGestionesCarteraDeudor|Begin|GetGestionGestionesCarteraDeudorAsync|request: {JsonSerializer.Serialize(gestionCarteraDeudorDto)}");
            var result = await _gestionService.GetGestionGestionesCarteraDeudorAsync(gestionCarteraDeudorDto);
            _Logger.LogInfo($"GetGestionGestionesCarteraDeudor|End|GetGestionGestionesCarteraDeudorAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        /// <summary>
        /// Obtiene el listado de GESTIONES HISTÓRICAS del deudor.
        /// </summary>
        /// <remarks>
        /// Obtiene el listado de GESTIONES HISTÓRICAS del deudor.
        /// </remarks>
        /// <response code="200">Obtiene el listado de GESTIONES HISTÓRICAS del deudor.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado Gestiones Historicas")]
        [HttpGet("GetGestionGestionesCarteraDeudorHistoricas")]
        [ProducesResponseType(typeof(ResultDto<GestionCarteraDeudorHistoricaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGestionGestionesCarteraDeudorHistoricasAsync([FromQuery] GestionCarteraDeudorHistoricaRequestDto gestionCarteraDeudorDto)
        {
            _Logger.LogInfo($"GetGestionGestionesCarteraDeudorHistoricas|Begin|GetGestionGestionesCarteraDeudorHistoricasAsync|request: {JsonSerializer.Serialize(gestionCarteraDeudorDto)}");
            var result = await _gestionService.GetGestionGestionesCarteraDeudorHistoricasAsync(gestionCarteraDeudorDto);
            _Logger.LogInfo($"GetGestionGestionesCarteraDeudorHistoricas|End|GetGestionGestionesCarteraDeudorHistoricasAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        /// <summary>
        /// Obtiene el listado de ESTADO DE GESTIÓN REALIZADA del deudor.
        /// </summary>
        /// <remarks>
        /// Obtiene el listado de ESTADO DE GESTIÓN REALIZADA del deudor.
        /// </remarks>
        /// <response code="200">Obtiene el listado de ESTADO DE GESTIÓN REALIZADA del deudor.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado Estado de Gestiones")]
        [HttpGet("GetGestionEstadosGestionesCarteraDeudor")]
        [ProducesResponseType(typeof(ResultDto<GetGestionEstadoGestionCarteraDeudorResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGestionEstadosGestionesCarteraDeudorAsync([FromQuery] GetGestionEstadoGestionCarteraDeudorRequestDto gestionEstadoCarteraDeudorDto)
        {
            _Logger.LogInfo($"GetGestionEstadosGestionesCarteraDeudor|Begin|GetGestionEstadosGestionesCarteraDeudorAsync|request: {JsonSerializer.Serialize(gestionEstadoCarteraDeudorDto)}");
            var result = await _gestionService.GetGestionEstadosGestionesCarteraDeudorAsync(gestionEstadoCarteraDeudorDto);
            _Logger.LogInfo($"GetGestionEstadosGestionesCarteraDeudor|End|GetGestionEstadosGestionesCarteraDeudorAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        /// <summary>
        /// Obtiene el listado de ESTADO DE GESTIÓN HISTORICA REALIZADA del deudor.
        /// </summary>
        /// <remarks>
        /// Obtiene el listado de ESTADO DE GESTIÓN HISTORICA REALIZADA del deudor.
        /// </remarks>
        /// <response code="200">Obtiene el listado de ESTADO DE GESTIÓN HISTORICA REALIZADA del deudor.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado Estado de Gestiones Historicas")]
        [HttpGet("GetGestionEstadosGestionesCarteraDeudorHistorica")]
        [ProducesResponseType(typeof(ResultDto<GestionCarteraDeudorEstadoHistoricaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGestionEstadosGestionesCarteraDeudorHistoricaAsync([FromQuery] GestionCarteraDeudorEstadoHistoricaRequestDto gestionEstadoCarteraDeudorHistoricaDto)
        {
            _Logger.LogInfo($"GetGestionEstadosGestionesCarteraDeudorHistorica|Begin|GetGestionEstadosGestionesCarteraDeudorHistoricaAsync|request: {JsonSerializer.Serialize(gestionEstadoCarteraDeudorHistoricaDto)}");
            var result = await _gestionService.GetGestionEstadosGestionesCarteraDeudorHistoricaAsync(gestionEstadoCarteraDeudorHistoricaDto);
            _Logger.LogInfo($"GetGestionEstadosGestionesCarteraDeudorHistorica|End|GetGestionEstadosGestionesCarteraDeudorHistoricaAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        /// <summary>
        /// Obtiene el listado de AGENDAS, botón +AGENDAS.
        /// </summary>
        /// <remarks>
        /// Obtiene el listado de AGENDAS, botón +AGENDAS.
        /// </remarks>
        /// <response code="200">Obtiene el listado de AGENDAS, BOTÓN +AGENDAS.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado Gestiones Agendadas")]
        [HttpGet("GetGestionAgendasDeudor")]
        [ProducesResponseType(typeof(ResultDto<GetGestionAgendaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGestionAgendasDeudorAsync([FromQuery] GetGestionAgendaRequestDto gestionAgendaDto)
        {
            _Logger.LogInfo($"GetGestionAgendasDeudor|Begin|GetGestionAgendasDeudorAsync|request: {JsonSerializer.Serialize(gestionAgendaDto)}");
            var result = await _gestionService.GetGestionAgendasDeudorAsync(gestionAgendaDto);
            _Logger.LogInfo($"GetGestionAgendasDeudor|End|GetGestionAgendasDeudorAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        /// <summary>
        /// Obtiene el listado de PAGOS, BOTÓN +PAGOS.
        /// </summary>
        /// <remarks>
        /// Obtiene el listado de PAGOS, BOTÓN +PAGOS.
        /// </remarks>
        /// <response code="200">Obtiene el listado de PAGOS, BOTÓN +PAGOS.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado Gestiones Pagos")]
        [HttpGet("GetGestionPagosDeudor")]
        [ProducesResponseType(typeof(ResultDto<GetGestionPagosResponsetDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGestionPagosDeudorAsync([FromQuery] GetGestionPagosRequestDto gestionPagoDto)
        {
            _Logger.LogInfo($"GetGestionPagosDeudor|Begin|GetGestionPagosDeudorAsync|request: {JsonSerializer.Serialize(gestionPagoDto)}");
            var result = await _gestionService.GetGestionPagosDeudorAsync(gestionPagoDto);
            _Logger.LogInfo($"GetGestionPagosDeudor|End|GetGestionPagosDeudorAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        /// <summary>
        /// Obtiene el listado de información del Deudor, BOTÓN +INF DEUDOR / FALSE = primer Registro / TRUE = segundo registro de la lista.
        /// </summary>
        /// <remarks>
        /// Obtiene el listado de información del Deudor, BOTÓN +INF DEUDOR / FALSE = primer Registro / TRUE = segundo registro de la lista.
        /// </remarks>
        /// <response code="200">Obtiene el listado de información del Deudor, BOTÓN +INF DEUDOR / FALSE = primer Registro / TRUE = segundo registro de la lista.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Obtener Cabecera de Información de Deudor")]
        [HttpGet("GetGestionInformacionDeudor")]
        [ProducesResponseType(typeof(ResultDto<GetGestionInformacionDeudorRespondeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGestionInformacionDeudorAsync([FromQuery] GetGestionInformacionDeudorRequestDto gestionInfoDeudor)
        {
            _Logger.LogInfo($"GetGestionInformacionDeudor|Begin|GetGestionInformacionDeudorAsync|request: {JsonSerializer.Serialize(gestionInfoDeudor)}");
            var result = await _gestionService.GetGestionInformacionDeudorAsync(gestionInfoDeudor);
            _Logger.LogInfo($"GetGestionInformacionDeudor|End|GetGestionInformacionDeudorAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        /// <summary>
        /// Obtiene el listado de información del Deudor, BOTÓN +INF DEUDOR / Tercer registro de la lista.
        /// </summary>
        /// <remarks>
        /// Obtiene el listado de información del Deudor, BOTÓN +INF DEUDOR / Tercer registro de la lista.
        /// </remarks>
        /// <response code="200">Obtiene el listado de información del Deudor, BOTÓN +INF DEUDOR / Tercer registro de la lista.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Obtener Cabecera de Información de Deudor Param")]
        [HttpGet("GetGestionInformacionDeudorParam")]
        [ProducesResponseType(typeof(ResultDto<GetGestionInformacionDeudorParamRespondeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGestionInformacionDeudorParamAsync([FromQuery] GetGestionInformacionDeudorParamRequestDto gestionInfoDeudorParam)
        {
            _Logger.LogInfo($"GetGestionInformacionDeudorParam|Begin|GetGestionInformacionDeudorParamAsync|request: {JsonSerializer.Serialize(gestionInfoDeudorParam)}");
            var result = await _gestionService.GetGestionInformacionDeudorParamAsync(gestionInfoDeudorParam);
            _Logger.LogInfo($"GetGestionInformacionDeudorParam|End|GetGestionInformacionDeudorParamAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        /// <summary>
        /// Obtiene el Listado de Tipo de Gestiones.
        /// </summary>
        /// <remarks>
        /// Obtiene el listado de Listado Tipo de Gestiones.
        /// </remarks>
        /// <response code="200">Obtiene el listado de Listado Tipo de Gestiones.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado Tipo de Gestiones")]
        [HttpGet("GetGestionTipoGestion")]
        [ProducesResponseType(typeof(ResultDto<GetGestionTipoGestionResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGestionTipoGestionAsync()
        {
            _Logger.LogInfo($"GetGestionTipoGestion|Begin|GetGestionTipoGestionAsync|request:");
            var result = await _gestionService.GetGestionTipoGestionAsync();
            _Logger.LogInfo($"GetGestionTipoGestion|End|GetGestionTipoGestionAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        /// <summary>
        /// Obtiene el Listado de Estado de Gestiones.
        /// </summary>
        /// <remarks>
        /// Obtiene el listado de Listado Estado de Gestiones.
        /// </remarks>
        /// <response code="200">Obtiene el listado de Listado Estado de Gestiones.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado Estado de Gestiones")]
        [HttpGet("GetGestionEstadoGestion")]
        [ProducesResponseType(typeof(ResultDto<GetGestionEstadoGestionResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGestionEstadoGestionAsync([FromQuery] GetGestionEstadoGestionRequestDto gestionEstadoDto)
        {
            _Logger.LogInfo($"GetGestionEstadoGestion|Begin|GetGestionEstadoGestionAsync|request: {JsonSerializer.Serialize(gestionEstadoDto)}");
            var result = await _gestionService.GetGestionEstadoGestionAsync(gestionEstadoDto);
            _Logger.LogInfo($"GetGestionEstadoGestion|End|GetGestionEstadoGestionAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        /// <summary>
        /// Obtiene el Listado Paleta de Respuesta.
        /// </summary>
        /// <remarks>
        /// Obtiene el Listado de Paleta de Respuesta.
        /// </remarks>
        /// <response code="200">Obtiene el Listado de Paleta de Respuesta.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado Paleta de Respuesta")]
        [HttpGet("GetGestionPaletaRespuesta")]
        [ProducesResponseType(typeof(ResultDto<GetGestionPaletaRespuestaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGestionPaletaRespuesAsync([FromQuery] GetGestionPaletaRespuestaRequestDto gestionPaletaDto)
        {
            _Logger.LogInfo($"GetGestionPaletaRespuesta|Begin|GetGestionPaletaRespuestaAsync|request: {JsonSerializer.Serialize(gestionPaletaDto)}");
            var result = await _gestionService.GetGestionPaletaRespuestaAsync(gestionPaletaDto);
            _Logger.LogInfo($"GetGestionPaletaRespuesta|End|GetGestionPaletaRespuestaAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        /// <summary>
        /// Obtiene el Listado Estado Gestion Claro.
        /// </summary>
        /// <remarks>
        /// Obtiene el listado de Estado Gestion Claro.
        /// </remarks>
        /// <response code="200">Obtiene el listado de Estado Gestion Claro.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado Paleta de Estado Gestion Claro")]
        [HttpGet("GetGestionEstadoGestionClaro")]
        [ProducesResponseType(typeof(ResultDto<GetGestionEstadoGestionClaroResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGestionEstadoGestionClaroAsync([FromQuery] GetGestionEstadoGestionClaroRequestDto estadoGestionClaroDto)
        {
            _Logger.LogInfo($"GetGestionEstadoGestionClaro|Begin|GetGestionEstadoGestionClaroAsync|request: {JsonSerializer.Serialize(estadoGestionClaroDto)}");
            var result = await _gestionService.GetGestionEstadoGestionClaroAsync(estadoGestionClaroDto);
            _Logger.LogInfo($"GetGestionEstadoGestionClaro|End|GetGestionEstadoGestionClaroAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        /// <summary>
        /// Obtiene el Listado de Motivo No Pago.
        /// </summary>
        /// <remarks>
        /// Obtiene el listado de Motivo No Pago.
        /// </remarks>
        /// <response code="200">Obtiene el listado de Motivo No Pago.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado Paleta de Motivo No Pago")]
        [HttpGet("GetGestionMotivoNoPago")]
        [ProducesResponseType(typeof(ResultDto<GetGestionMotivoNoPagoResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGestionMotivoNoPagoAsync([FromQuery] GetGestionMotivoNoPagoRequestDto motivoNoPagoDto)
        {
            _Logger.LogInfo($"GetGestionMotivoNoPago|Begin|GetGestionMotivoNoPagoAsync|request: {JsonSerializer.Serialize(motivoNoPagoDto)}");
            var result = await _gestionService.GetGestionMotivoNoPagoAsync(motivoNoPagoDto);
            _Logger.LogInfo($"GetGestionMotivoNoPago|End|GetGestionMotivoNoPagoAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        /// <summary>
        /// Para poder realizar la descarga del archivo excel de estado de cuenta.
        /// </summary>
        /// <remarks>
        /// Para poder realizar la descarga del archivo excel de estado de cuenta.
        /// </remarks>
        /// <response code="200">Para poder realizar la descarga del archivo excel de estado de cuenta.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Para exportar el estado de cuenta")]
        [HttpGet("ExportGestionEstadoCuenta")]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetGestionEstadoCuentaRequestDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetGestionEstadoCuentaResponseDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetGestionEstadoCuentaResponseDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExportGestionEstadoCuentaAsync([FromQuery] GetGestionEstadoCuentaRequestDto estadoCuentaDto)
        {
            _Logger.LogInfo($"ExportGestionEstadoCuenta|Begin|ExportGestionEstadoCuentaAsync|request: {JsonSerializer.Serialize(estadoCuentaDto)}");
            var excel = await _gestionService.ExportGestionEstadoCuentaAsync(estadoCuentaDto);
            _Logger.LogInfo($"ExportGestionEstadoCuenta|End|ExportGestionEstadoCuentaAsync|response: {JsonSerializer.Serialize(excel)}");
            return File(excel, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"EstadoCuenta_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }

        [HttpPost("CreateGestionOpeGesContratos")]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<CreateGestionOpeGesResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<CreateGestionOpeGesResponseDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<CreateGestionOpeGesResponseDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateGestionOpeGesContratosAsync([FromBody] CreateGestionOpeGesRequestDto gestionOpeGesDto)
        {
            _Logger.LogInfo($"CreateGestionOpeGesContratos|Begin|CreateGestionOpeGesContratosAsync|request: {JsonSerializer.Serialize(gestionOpeGesDto)}");
            var result = await _gestionService.CreateGestionOpeGesContratosAsync(gestionOpeGesDto);
            _Logger.LogInfo($"CreateGestionOpeGesContratos|End|CreateGestionOpeGesContratosAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Listado de Gestiones Diarias.
        /// </summary>
        /// <remarks>
        /// Listado de Gestiones Diarias.
        /// </remarks>
        /// <response code="200">Listado de Gestiones Diarias.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado de Gestiones Diarias")]
        [HttpGet("GetGestionToDay")]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetGestionToDayResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetGestionToDayResponseDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetGestionToDayResponseDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGestionToDayAsync([FromQuery] GetGestionToDayRequestDto gestionToDayDto)
        {
            _Logger.LogInfo($"GetGestionToDay|Begin|GetGestionToDayAsync|request: {JsonSerializer.Serialize(gestionToDayDto)}");
            var result = await _gestionService.GetGestionToDayAsync(gestionToDayDto);
            _Logger.LogInfo($"GetGestionToDay|End|GetGestionToDayAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }
    }
}