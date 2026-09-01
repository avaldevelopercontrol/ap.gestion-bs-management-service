using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Opcion;
using GesMgmt.Infraestructure.Logger;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using static GesMgmt.Application.DTOs.Opcion.OpcionRequestDto;
using static GesMgmt.Application.DTOs.Opcion.OpcionResponseDto;

namespace GesMgmt.WebAPI.Controllers
{
    [ApiController]
    [Route("v1/Opcion")]
    [Produces("application/json")]
    public class OpcionController : ControllerBase
    {
        private readonly IOpcionService _opcionService;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;
        private ValidationMessageDto _oValMsgDto;

        public OpcionController(IOpcionService opcionService, IValidationMessageService validationMessageService, IAppLogger logger)
        {
            _opcionService = opcionService;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _Logger = logger;
            _Logger.LogInfo("| ** API.BS.GestionManagement ** |");
        }

        /// <summary>
        /// Listado de Opciones.
        /// </summary>
        /// <remarks>
        /// Listado de Opciones.
        /// </remarks>
        /// <response code="200">Listado de Opciones.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado de Opciones")]
        [HttpGet("GetOpciones")]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetOpcionesResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetOpcionesResponseDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetOpcionesResponseDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetOpcionesAsync()
        {
            _Logger.LogInfo($"GetOpciones|Begin|GetOpcionesAsync|request:");
            var result = await _opcionService.GetOpcionesAsync();
            _Logger.LogInfo($"GetOpciones|End|GetOpcionesAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Obtiene la OPCION registrado.
        /// </summary>
        /// <remarks>
        /// Obtiene la OPCION registrado.
        /// </remarks>
        /// <response code="200">Obtiene la OPCION registrado.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Obtiene la OPCION registrado")]
        [HttpGet("{nId_Opcion}")]
        [ProducesResponseType(typeof(ResultDto<GetOpcionByIdResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int nId_Opcion)
        {
            _Logger.LogInfo($"GetOpcionById|Begin|GetOpcionByIdAsync|request:{nId_Opcion}");
            var result = await _opcionService.GetOpcionByIdAsync(nId_Opcion);
            _Logger.LogInfo($"GetOpcionById|End|GetOpcionByIdAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Crear OPCION.
        /// </summary>
        /// <remarks>
        /// Crear OPCION.
        /// </remarks>
        /// <response code="200">Crear OPCION.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ResultDto<CreateOpcionResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateOpcionAsync([FromBody] CreateOpcionRequestDto opcionDto)
        {
            _Logger.LogInfo($"CreateOpcion|Begin|CreateOpcionAsync|request: {JsonSerializer.Serialize(opcionDto)}");
            var result = await _opcionService.CreateOpcionAsync(opcionDto);
            _Logger.LogInfo($"CreateOpcion|End|CreateOpcionAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Editar OPCION.
        /// </summary>
        /// <remarks>
        /// Editar OPCION.
        /// </remarks>
        /// <response code="200">Editar OPCION.</response>
        [HttpPut]
        [ProducesResponseType(typeof(ResultDto<EditOpcionResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EditOpcionAsync([FromBody] EditOpcionRequestDto opcionDto)
        {
            _Logger.LogInfo($"EditOpcion|Begin|EditOpcionAsync|request: {JsonSerializer.Serialize(opcionDto)}");
            var result = await _opcionService.EditOpcionAsync(opcionDto);
            _Logger.LogInfo($"EditOpcion|End|EditOpcionAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }
    }
}