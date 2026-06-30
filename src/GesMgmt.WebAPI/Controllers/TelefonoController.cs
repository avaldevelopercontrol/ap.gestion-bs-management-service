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

        /// <summary>
        /// Obtiene el listado de TELÉFONOS cargados del deudor.
        /// </summary>
        /// <remarks>
        /// Obtiene el listado de TELÉFONOS cargados del deudor.
        /// </remarks>
        /// <response code="200">Obtiene el listado de TELÉFONOS cargados del deudor.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado de Telefonos")]
        [HttpGet("GetTelefonos")]
        [ProducesResponseType(typeof(ResultDto<GetTelefonosResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTelefonosAsync([FromQuery] GetTelefonosRequestDto gestionTelefonoDto)
        {
            _Logger.LogInfo($"GetTelefonos|Begin|GetTelefonosAsync|request: {JsonSerializer.Serialize(gestionTelefonoDto)}");
            var result = await _telefonoService.GetTelefonosAsync(gestionTelefonoDto);
            _Logger.LogInfo($"GetTelefonos|End|GetTelefonosAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        /// <summary>
        /// Obtiene el TELÉFONO por ID TELEFONO.
        /// </summary>
        /// <remarks>
        /// Obtiene el TELÉFONO por ID TELEFONO.
        /// </remarks>
        /// <response code="200">Obtiene el TELÉFONO por ID TELEFONO.</response>
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

        /// <summary>
        /// Crear TELÉFONO por DEUDOR.
        /// </summary>
        /// <remarks>
        /// Crear TELÉFONO por DEUDOR.
        /// </remarks>
        /// <response code="200">Crear TELÉFONO por DEUDOR.</response>
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

        /// <summary>
        /// Editar TELÉFONO por DEUDOR.
        /// </summary>
        /// <remarks>
        /// Editar TELÉFONO por DEUDOR.
        /// </remarks>
        /// <response code="200">Editar TELÉFONO por DEUDOR.</response>
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

        /// <summary>
        /// Obtiene el listado de RESULTADOS.
        /// </summary>
        /// <remarks>
        /// Obtiene el listado de RESULTADOS.
        /// </remarks>
        /// <response code="200">Obtiene el listado de RESULTADOS.</response>
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

        /// <summary>
        /// Obtiene el listado de OPERADORES.
        /// </summary>
        /// <remarks>
        /// Obtiene el listado de OPERADORES.
        /// </remarks>
        /// <response code="200">Obtiene el listado de OPERADORES.</response>
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

        /// <summary>
        /// Obtiene el listado de UBICACIONES.
        /// </summary>
        /// <remarks>
        /// Obtiene el listado de UBICACIONES.
        /// </remarks>
        /// <response code="200">Obtiene el listado de UBICACIONES.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado Ubicaciones de Telefono")]
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

        /// <summary>
        /// Obtiene el listado de HORARIO DE GESTIÓN.
        /// </summary>
        /// <remarks>
        /// Obtiene el listado de HORARIO DE GESTIÓN.
        /// </remarks>
        /// <response code="200">Obtiene el listado de HORARIO DE GESTIÓN.</response>
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

        /// <summary>
        /// Obtiene el listado de FUENTE DE BUSQUEDA.
        /// </summary>
        /// <remarks>
        /// Obtiene el listado de FUENTE DE BUSQUEDA.
        /// </remarks>
        /// <response code="200">Obtiene el listado de FUENTE DE BUSQUEDA.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado Fuentes de Búsqueda de Telefono")]
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