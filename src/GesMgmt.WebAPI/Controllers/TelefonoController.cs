using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Telefono;
using GesMgmt.Infraestructure.Logger;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using static GesMgmt.Application.DTOs.Telefono.TelefonoRequestDto;
using static GesMgmt.Application.DTOs.Telefono.TelefonoResponseDto;

namespace GesMgmt.WebAPI.Controllers
{
    [ApiController]
    [Route("v1/Telefono")]
    [Produces("application/json")]
    public class TelefonoController : ControllerBase
    {
        private readonly ITelefonoService _telefonoService;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;
        private ValidationMessageDto _oValMsgDto;

        public TelefonoController(ITelefonoService telefonoService, IValidationMessageService validationMessageService, IAppLogger logger)
        {
            _telefonoService = telefonoService;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _Logger = logger;
            _Logger.LogInfo("| ** API.BS.GestionManagement ** |");
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Telefono Por Id")]
        //[HttpGet("GetTelefonoByIdTelefono")]
        [HttpGet("{nId_PersTelef}")]
        [ProducesResponseType(typeof(ResultDto<GetTelefonoAsync>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int nId_PersTelef)
        {
            _Logger.LogInfo($"GetTelefonoByIdTelefono|Begin|GetTelefonoByIdTelefonoAsync|request:");
            var result = await _telefonoService.GetTelefonoByIdTelefonoAsync(nId_PersTelef);
            _Logger.LogInfo($"GetTelefonoByIdTelefono|End|GetTelefonoByIdTelefonoAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultDto<CreateTelefonoResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateTelefonoAsync([FromBody] CreateTelefonoRequestDto telefonoDto)
        {
            _Logger.LogInfo($"CreateTelefono|Begin|CreateTelefonoAsync|request: {JsonSerializer.Serialize(telefonoDto)}");
            var result = await _telefonoService.CreateTelefonoAsync(telefonoDto);
            _Logger.LogInfo($"CreateTelefono|End|CreateTelefonoAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut]
        [ProducesResponseType(typeof(ResultDto<EditTelefonoResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EditTelefonoAsync([FromBody] EditTelefonoRequestDto telefonoEditDto)
        {
            _Logger.LogInfo($"EditTelefono|Begin|EditTelefonoAsync|request: {JsonSerializer.Serialize(telefonoEditDto)}");
            var result = await _telefonoService.EditTelefonoAsync(telefonoEditDto);
            _Logger.LogInfo($"EditTelefono|End|EditTelefonoAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Listado Resultados de Telefono")]
        [HttpGet("GetTelefonoResultados")]
        [ProducesResponseType(typeof(ResultDto<GetTelefonoResultados>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTelefonoResultadosAsync()
        {
            _Logger.LogInfo($"GetTelefonoResultados|Begin|GetTelefonoResultadosAsync|request:");
            var result = await _telefonoService.GetTelefonoResultadosAsync();
            _Logger.LogInfo($"GetTelefonoResultados|End|GetTelefonoResultadosAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Listado Operadores de Telefono")]
        [HttpGet("GetTelefonoOperadores")]
        [ProducesResponseType(typeof(ResultDto<GetTelefonoOperadores>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTelefonoOperadoresAsync()
        {
            _Logger.LogInfo($"GetTelefonoOperadores|Begin|GetTelefonoOperadoresAsync|request:");
            var result = await _telefonoService.GetTelefonoOperadoresAsync();
            _Logger.LogInfo($"GetTelefonoOperadores|End|GetTelefonoOperadoresAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Listado Operadores de Telefono")]
        [HttpGet("GetTelefonoUbicaciones")]
        [ProducesResponseType(typeof(ResultDto<GetTelefonoUbicaciones>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTelefonoUbicacionesAsync()
        {
            _Logger.LogInfo($"GetTelefonoUbicaciones|Begin|GetTelefonoUbicacionesAsync|request:");
            var result = await _telefonoService.GetTelefonoUbicacionesAsync();
            _Logger.LogInfo($"GetTelefonoUbicaciones|End|GetTelefonoUbicacionesAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Listado Horario de Gestion de Telefono")]
        [HttpGet("GetTelefonoHorarioGestion")]
        [ProducesResponseType(typeof(ResultDto<GetTelefonoHorarioGestion>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTelefonoHorarioGestionAsync()
        {
            _Logger.LogInfo($"GetTelefonoHorarioGestion|Begin|GetTelefonoHorarioGestionAsync|request:");
            var result = await _telefonoService.GetTelefonoHorarioGestionAsync();
            _Logger.LogInfo($"GetTelefonoHorarioGestion|End|GetTelefonoHorarioGestionAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Listado Horario de Gestion de Telefono")]
        [HttpGet("GetTelefonoFuenteBusqueda")]
        [ProducesResponseType(typeof(ResultDto<GetTelefonoFuenteBusqueda>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTelefonoFuenteBusquedaAsync()
        {
            _Logger.LogInfo($"GetTelefonoFuenteBusqueda|Begin|GetTelefonoFuenteBusquedaAsync|request:");
            var result = await _telefonoService.GetTelefonoFuenteBusquedaAsync();
            _Logger.LogInfo($"GetTelefonoFuenteBusqueda|End|GetTelefonoFuenteBusquedaAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }
    }
}