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

        [SwaggerOperation(Summary = "[API]: Endpoint Listado Cabecera de Gestiones")]
        [HttpGet("GetGestionesCabecera")]
        public async Task<IActionResult> GetGestionesCabeceraAsync([FromQuery] GetGestionCabeceraRequestDto gestionCabeceraDto)
        {
            _Logger.LogInfo($"GetGestionesCabecera|Begin|GetGestionesCabeceraAsync|request: {JsonSerializer.Serialize(gestionCabeceraDto)}");
            var result = await _gestionService.GetGestionesCabeceraAsync(gestionCabeceraDto);
            _Logger.LogInfo($"GetGestionesCabecera|End|GetGestionesCabeceraAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Listado Gestiones")]
        [HttpGet("GetGestiones")]
        public async Task<IActionResult> GetGestionesAsync([FromQuery] GetGestionRequestDto gestionDto)
        {
            _Logger.LogInfo($"GetGestiones|Begin|GetGestionesAsync|request: {JsonSerializer.Serialize(gestionDto)}");
            var result = await _gestionService.GetGestionesAsync(gestionDto);
            _Logger.LogInfo($"GetGestiones|End|GetGestionesAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Gestiones Deudores")]
        [HttpGet("GetGestionesDeudor")]
        public async Task<IActionResult> GetGestionesDeudorAsync([FromQuery] GetGestionDeudorRequestDto gestionDeudorDto)
        {
            _Logger.LogInfo($"GetGestionesDeudor|Begin|GetGestionesDeudorAsync|request: {JsonSerializer.Serialize(gestionDeudorDto)}");
            var result = await _gestionService.GetGestionesDeudorAsync(gestionDeudorDto);
            _Logger.LogInfo($"GetGestionesDeudor|End|GetGestionesDeudorAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Listado Cabecera Gestiones Adicionales")]
        [HttpGet("GetGestionesCabeceraAdicionales")]
        public async Task<IActionResult> GetGestionesCabeceraAdicionalesAsync([FromQuery] GetGestionCabeceraAdicionalRequestDto gestionCabeceraAdicionalDto)
        {
            _Logger.LogInfo($"GetGestionesCabeceraAdicionales|Begin|GetGestionesCabeceraAdicionalesAsync|request: {JsonSerializer.Serialize(gestionCabeceraAdicionalDto)}");
            var result = await _gestionService.GetGestionesCabeceraAdicionalesAsync(gestionCabeceraAdicionalDto);
            _Logger.LogInfo($"GetGestionesCabeceraAdicionales|End|GetGestionesCabeceraAdicionalesAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Listado Gestiones Adicionales")]
        [HttpGet("GetGestionesAdicionales")]
        public async Task<IActionResult> GetGestionesAdicionalesAsync([FromQuery] GetGestionAdicionalRequestDto gestionAdicionalDto)
        {
            _Logger.LogInfo($"GetGestionesAdicionales|Begin|GetGestionesAdicionalesAsync|request: {JsonSerializer.Serialize(gestionAdicionalDto)}");
            var result = await _gestionService.GetGestionesAdicionalesAsync(gestionAdicionalDto);
            _Logger.LogInfo($"GetGestionesAdicionales|End|GetGestionesAdicionalesAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Listado Gestiones Telefonos")]
        [HttpGet("GetGestionesTelefonos")]
        public async Task<IActionResult> GetGestionesTelefonosAsync([FromQuery] GetGestionTelefonoRequestDto gestionTelefonoDto)
        {
            _Logger.LogInfo($"GetGestionesTelefonos|Begin|GetGestionesTelefonosAsync|request: {JsonSerializer.Serialize(gestionTelefonoDto)}");
            var result = await _gestionService.GetTelefonoGestionAsync(gestionTelefonoDto);
            _Logger.LogInfo($"GetGestionesTelefonos|End|GetGestionesTelefonosAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Listado Gestiones Direcciones")]
        [HttpGet("GetGestionesDirecciones")]
        public async Task<IActionResult> GetGestionesDireccionAsync([FromQuery] GetGestionDireccionRequestDto gestionDireccionDto)
        {
            _Logger.LogInfo($"GetGestionesDirecciones|Begin|GetGestionesDireccionesAsync|request: {JsonSerializer.Serialize(gestionDireccionDto)}");
            var result = await _gestionService.GetGestionDireccionesAsync(gestionDireccionDto);
            _Logger.LogInfo($"GetGestionesDirecciones|End|GetGestionesDireccionesAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }
    }
}