using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.PerfilOpcion;
using GesMgmt.Infraestructure.Logger;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using static GesMgmt.Application.DTOs.PerfilOpcion.PerfilOpcionRequestDto;
using static GesMgmt.Application.DTOs.PerfilOpcion.PerfilOpcionResponseDto;

namespace GesMgmt.WebAPI.Controllers
{
    [ApiController]
    [Route("v1/PerfilOpcion")]
    [Produces("application/json")]
    public class PerfilOpcionController : ControllerBase
    {
        private readonly IPerfilOpcionService _perfilOpcionService;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;
        private ValidationMessageDto _oValMsgDto;

        public PerfilOpcionController(IPerfilOpcionService perfilOpcionService, IValidationMessageService validationMessageService, IAppLogger logger)
        {
            _perfilOpcionService = perfilOpcionService;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _Logger = logger;
            _Logger.LogInfo("| ** API.BS.GestionManagement ** |");
        }

        /// <summary>
        /// Listado de Perfil - Opción.
        /// </summary>
        /// <remarks>
        /// Listado de Perfil - Opción.
        /// </remarks>
        /// <response code="200">Listado de Perfil - Opción.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado de Perfil - Opción")]
        [HttpGet("GetPerfilOptionsCount")]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetPerfilOpcionResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetPerfilOpcionResponseDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetPerfilOpcionResponseDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPerfilOptionsCountAsync()
        {
            _Logger.LogInfo($"GetPerfilOptionsCount|Begin|GetPerfilOptionsCountAsync|request:");
            var result = await _perfilOpcionService.GetPerfilOptionsCountAsync();
            _Logger.LogInfo($"GetPerfilOptionsCount|End|GetPerfilOptionsCountAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        /// <summary>
        /// Listado de Opciones por Perfil.
        /// </summary>
        /// <remarks>
        /// Listado de Opciones por Perfil.
        /// </remarks>
        /// <response code="200">Listado de Opciones por Perfil.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado de Opciones por Perfil")]
        [HttpGet("GetOpcionesPorPerfil")]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetOpcionesPorPerfilResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetOpcionesPorPerfilResponseDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetOpcionesPorPerfilResponseDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetOpcionesPorPerfilAsync([FromHeader] int nId_Perfil)
        {
            _Logger.LogInfo($"GetOpcionesPorPerfil|Begin|GetOpcionesPorPerfilAsync|request:");
            var result = await _perfilOpcionService.GetOpcionesPorPerfilAsync(nId_Perfil);
            _Logger.LogInfo($"GetOpcionesPorPerfil|End|GetOpcionesPorPerfilAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        /// <summary>
        /// Crear PERFIL - OPCION.
        /// </summary>
        /// <remarks>
        /// Crear PERFIL - OPCION.
        /// </remarks>
        /// <response code="200">Crear PERFIL - OPCION.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ResultDto<CreatePerfilOpcionResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePerfilOpcionAsync([FromBody] CreatePerfilOpcionRequestDto perfilOpcionDto)
        {
            _Logger.LogInfo($"CreatePerfilOpcion|Begin|CreatePerfilOpcionAsync|request: {JsonSerializer.Serialize(perfilOpcionDto)}");
            var result = await _perfilOpcionService.CreatePerfilOpcionAsync(perfilOpcionDto);
            _Logger.LogInfo($"CreatePerfilOpcion|End|CreatePerfilOpcionAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Editar PERFIL - OPCION.
        /// </summary>
        /// <remarks>
        /// Editar PERFIL - OPCION.
        /// </remarks>
        /// <response code="200">Editar PERFIL - OPCION.</response>
        [HttpPut]
        [ProducesResponseType(typeof(ResultDto<EditPerfilOpcionResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EditPerfilOpcionAsync([FromBody] EditPerfilOpcionRequestDto perfilOpcionDto)
        {
            _Logger.LogInfo($"EditPerfilOpcion|Begin|EditPerfilOpcionAsync|request: {JsonSerializer.Serialize(perfilOpcionDto)}");
            var result = await _perfilOpcionService.EditPerfilOpcionAsync(perfilOpcionDto);
            _Logger.LogInfo($"EditPerfilOpcion|End|EditPerfilOpcionAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }
    }
}
