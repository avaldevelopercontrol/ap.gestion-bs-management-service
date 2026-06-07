using Microsoft.AspNetCore.Mvc;
using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Utils;
using GesMgmt.Domain.Constants;
using GesMgmt.Infraestructure.Logger;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using GesMgmt.Application.DTOs.Gestion;

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
        public async Task<IActionResult> GetGestionDocumentosCabeceraAsync([FromQuery] GetGestionCabeceraRequestDto gestionCabeceraDto)
        {
            _Logger.LogInfo($"GetGestionCabecera|Begin|GetGestionDocumentosCabeceraAsync|request: {JsonSerializer.Serialize(gestionCabeceraDto)}");
            var result = await _gestionService.GetGestionDocumentosCabeceraAsync(gestionCabeceraDto);
            _Logger.LogInfo($"GetGestionCabecera|End|GetGestionDocumentosCabeceraAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Listado Gestiones Documentos")]
        [HttpGet("GetGestionDocumentos")]
        public async Task<IActionResult> GetGestionDocumentosAsync([FromQuery] GetGestionDocumentoRequestDto gestionDto)
        {
            _Logger.LogInfo($"GetGestionDocumentos|Begin|GetGestionesDocumentosAsync|request: {JsonSerializer.Serialize(gestionDto)}");
            var result = await _gestionService.GetGestionDocumentosAsync(gestionDto);
            _Logger.LogInfo($"GetGestionDocumentos|End|GetGestionesDocumentosAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Gestiones Deudores")]
        [HttpGet("GetGestionDeudor")]
        public async Task<IActionResult> GetGestionDeudorAsync([FromQuery] GetGestionDeudorRequestDto gestionDeudorDto)
        {
            _Logger.LogInfo($"GetGestionDeudor|Begin|GetGestionDeudorAsync|request: {JsonSerializer.Serialize(gestionDeudorDto)}");
            var result = await _gestionService.GetGestionDeudorAsync(gestionDeudorDto);
            _Logger.LogInfo($"GetGestionDeudor|End|GetGestionDeudorAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Listado Cabecera Gestiones Documentos Adicionales")]
        [HttpGet("GetGestionDocumentosAdicionalesCabecera")]
        public async Task<IActionResult> GetGestionDocumentosAdicionalesCabeceraAsync([FromQuery] GetGestionCabeceraAdicionalRequestDto gestionCabeceraAdicionalDto)
        {
            _Logger.LogInfo($"GetGestionDocumentosAdicionalesCabecera|Begin|GetGestionDocumentosAdicionalesCabeceraAsync|request: {JsonSerializer.Serialize(gestionCabeceraAdicionalDto)}");
            var result = await _gestionService.GetGestionDocumentosAdicionalesCabeceraAsync(gestionCabeceraAdicionalDto);
            _Logger.LogInfo($"GetGestionDocumentosAdicionalesCabecera|End|GetGestionDocumentosAdicionalesCabeceraAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Listado Gestiones Documentos Adicionales")]
        [HttpGet("GetGestionDocumentosAdicionales")]
        public async Task<IActionResult> GetGestionDocumentosAdicionalesAsync([FromQuery] GetGestionAdicionalRequestDto gestionAdicionalDto)
        {
            _Logger.LogInfo($"GetGestionDocumentosAdicionales|Begin|GetGestionDocumentosAdicionalesAsync|request: {JsonSerializer.Serialize(gestionAdicionalDto)}");
            var result = await _gestionService.GetGestionDocumentosAdicionalesAsync(gestionAdicionalDto);
            _Logger.LogInfo($"GetGestionDocumentosAdicionales|End|GetGestionDocumentosAdicionalesAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Listado Gestiones Telefonos")]
        [HttpGet("GetGestionTelefonos")]
        public async Task<IActionResult> GetGestionTelefonosAsync([FromQuery] GetGestionTelefonoRequestDto gestionTelefonoDto)
        {
            _Logger.LogInfo($"GetGestionTelefonos|Begin|GetGestionTelefonosAsync|request: {JsonSerializer.Serialize(gestionTelefonoDto)}");
            var result = await _gestionService.GetGestionTelefonosAsync(gestionTelefonoDto);
            _Logger.LogInfo($"GetGestionTelefonos|End|GetGestionTelefonosAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Listado Gestiones Direcciones")]
        [HttpGet("GetGestionDirecciones")]
        public async Task<IActionResult> GetGestionesDireccionAsync([FromQuery] GetGestionDireccionRequestDto gestionDireccionDto)
        {
            _Logger.LogInfo($"GetGestionDirecciones|Begin|GetGestionDireccionesAsync|request: {JsonSerializer.Serialize(gestionDireccionDto)}");
            var result = await _gestionService.GetGestionDireccionesAsync(gestionDireccionDto);
            _Logger.LogInfo($"GetGestionDirecciones|End|GetGestionDireccionesAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Listado Gestiones Direcciones")]
        [HttpGet("GetGestionGestionesCarteraDeudor")]
        public async Task<IActionResult> GetGestionGestionesCarteraDeudorAsync([FromQuery] GetGestionGestionesCarteraDeudorRequestDto gestionCarteraDeudorDto)
        {
            _Logger.LogInfo($"GetGestionGestionesCarteraDeudor|Begin|GetGestionGestionesCarteraDeudorAsync|request: {JsonSerializer.Serialize(gestionCarteraDeudorDto)}");
            var result = await _gestionService.GetGestionGestionesCarteraDeudorAsync(gestionCarteraDeudorDto);
            _Logger.LogInfo($"GetGestionGestionesCarteraDeudor|End|GetGestionGestionesCarteraDeudorAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }
    }
}