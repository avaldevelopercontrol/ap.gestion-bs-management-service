using Microsoft.AspNetCore.Mvc;
using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Utils;
using GesMgmt.Domain.Constants;
using GesMgmt.Infraestructure.Logger;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using GesMgmt.Application.DTOs.Gestion;
using GesMgmt.Application.Interfaces.Gestion;

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

        [SwaggerOperation(Summary = "[API]: Endpoint Listado Gestiones Documentos Cabecera")]
        [HttpGet("GetGestionDocumentosCabecera")]
        [ProducesResponseType(typeof(ResultDto<GetGestionCabeResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGestionDocumentosCabeceraAsync([FromQuery] GetGestionCabeRequestDto gestionCabeceraDto)
        {
            _Logger.LogInfo($"GetGestionCabecera|Begin|GetGestionDocumentosCabeceraAsync|request: {JsonSerializer.Serialize(gestionCabeceraDto)}");
            var result = await _gestionService.GetGestionDocumentosCabeceraAsync(gestionCabeceraDto);
            _Logger.LogInfo($"GetGestionCabecera|End|GetGestionDocumentosCabeceraAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Listado Gestiones Documentos")]
        [HttpGet("GetGestionDocumentos")]
        [ProducesResponseType(typeof(ResultDto<GetGestionDocuResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGestionDocumentosAsync([FromQuery] GetGestionDocuRequestDto gestionDto)
        {
            _Logger.LogInfo($"GetGestionDocumentos|Begin|GetGestionesDocumentosAsync|request: {JsonSerializer.Serialize(gestionDto)}");
            var result = await _gestionService.GetGestionDocumentosAsync(gestionDto);
            _Logger.LogInfo($"GetGestionDocumentos|End|GetGestionesDocumentosAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Gestiones Deudores")]
        [HttpGet("GetGestionDeudor")]
        [ProducesResponseType(typeof(ResultDto<GetGestionDeudResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGestionDeudorAsync([FromQuery] GetGestionDeudRequestDto gestionDeudorDto)
        {
            _Logger.LogInfo($"GetGestionDeudor|Begin|GetGestionDeudorAsync|request: {JsonSerializer.Serialize(gestionDeudorDto)}");
            var result = await _gestionService.GetGestionDeudorAsync(gestionDeudorDto);
            _Logger.LogInfo($"GetGestionDeudor|End|GetGestionDeudorAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Listado Cabecera Gestiones Documentos Adicionales")]
        [HttpGet("GetGestionDocumentosAdicionalesCabecera")]
        [ProducesResponseType(typeof(ResultDto<GetGestionCabeAdicResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGestionDocumentosAdicionalesCabeceraAsync([FromQuery] GetGestionCabeAdicRequestDto gestionCabeceraAdicionalDto)
        {
            _Logger.LogInfo($"GetGestionDocumentosAdicionalesCabecera|Begin|GetGestionDocumentosAdicionalesCabeceraAsync|request: {JsonSerializer.Serialize(gestionCabeceraAdicionalDto)}");
            var result = await _gestionService.GetGestionDocumentosAdicionalesCabeceraAsync(gestionCabeceraAdicionalDto);
            _Logger.LogInfo($"GetGestionDocumentosAdicionalesCabecera|End|GetGestionDocumentosAdicionalesCabeceraAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Listado Gestiones Documentos Adicionales")]
        [HttpGet("GetGestionDocumentosAdicionales")]
        [ProducesResponseType(typeof(ResultDto<GetGestionAdicResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGestionDocumentosAdicionalesAsync([FromQuery] GetGestionAdicRequestDto gestionAdicionalDto)
        {
            _Logger.LogInfo($"GetGestionDocumentosAdicionales|Begin|GetGestionDocumentosAdicionalesAsync|request: {JsonSerializer.Serialize(gestionAdicionalDto)}");
            var result = await _gestionService.GetGestionDocumentosAdicionalesAsync(gestionAdicionalDto);
            _Logger.LogInfo($"GetGestionDocumentosAdicionales|End|GetGestionDocumentosAdicionalesAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Listado Gestiones Telefonos")]
        [HttpGet("GetGestionTelefonos")]
        [ProducesResponseType(typeof(ResultDto<GetGestionTeleResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGestionTelefonosAsync([FromQuery] GetGestionTeleRequestDto gestionTelefonoDto)
        {
            _Logger.LogInfo($"GetGestionTelefonos|Begin|GetGestionTelefonosAsync|request: {JsonSerializer.Serialize(gestionTelefonoDto)}");
            var result = await _gestionService.GetGestionTelefonosAsync(gestionTelefonoDto);
            _Logger.LogInfo($"GetGestionTelefonos|End|GetGestionTelefonosAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Listado Gestiones Direcciones")]
        [HttpGet("GetGestionDirecciones")]
        [ProducesResponseType(typeof(ResultDto<GetGestionDireResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGestionesDireccionAsync([FromQuery] GetGestionDireRequestDto gestionDireccionDto)
        {
            _Logger.LogInfo($"GetGestionDirecciones|Begin|GetGestionDireccionesAsync|request: {JsonSerializer.Serialize(gestionDireccionDto)}");
            var result = await _gestionService.GetGestionDireccionesAsync(gestionDireccionDto);
            _Logger.LogInfo($"GetGestionDirecciones|End|GetGestionDireccionesAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Listado Gestiones")]
        [HttpGet("GetGestionGestionesCarteraDeudor")]
        [ProducesResponseType(typeof(ResultDto<GetGestionGestCartDeudResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGestionGestionesCarteraDeudorAsync([FromQuery] GetGestionGestCartDeudRequestDto gestionCarteraDeudorDto)
        {
            _Logger.LogInfo($"GetGestionGestionesCarteraDeudor|Begin|GetGestionGestionesCarteraDeudorAsync|request: {JsonSerializer.Serialize(gestionCarteraDeudorDto)}");
            var result = await _gestionService.GetGestionGestionesCarteraDeudorAsync(gestionCarteraDeudorDto);
            _Logger.LogInfo($"GetGestionGestionesCarteraDeudor|End|GetGestionGestionesCarteraDeudorAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Listado Estado de Gestiones")]
        [HttpGet("GetGestionEstadosGestionesCarteraDeudor")]
        [ProducesResponseType(typeof(ResultDto<GetGestionEstaGestCartDeudResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGestionEstadosGestionesCarteraDeudorAsync([FromQuery] GetGestionEstaGestCartDeudRequestDto gestionEstadoCarteraDeudorDto)
        {
            _Logger.LogInfo($"GetGestionEstadosGestionesCarteraDeudor|Begin|GetGestionEstadosGestionesCarteraDeudorAsync|request: {JsonSerializer.Serialize(gestionEstadoCarteraDeudorDto)}");
            var result = await _gestionService.GetGestionEstadosGestionesCarteraDeudorAsync(gestionEstadoCarteraDeudorDto);
            _Logger.LogInfo($"GetGestionEstadosGestionesCarteraDeudor|End|GetGestionEstadosGestionesCarteraDeudorAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }
    }
}