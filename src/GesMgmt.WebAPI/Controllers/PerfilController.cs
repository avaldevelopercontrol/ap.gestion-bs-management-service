using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Perfil;
using GesMgmt.Infraestructure.Logger;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using static GesMgmt.Application.DTOs.Perfil.PerfilRequestDto;
using static GesMgmt.Application.DTOs.Perfil.PerfilResponseDto;

namespace GesMgmt.WebAPI.Controllers
{
    [ApiController]
    [Route("v1/Perfil")]
    [Produces("application/json")]
    public class PerfilController : ControllerBase
    {
        private readonly IPerfilService _perfilService;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;
        private ValidationMessageDto _oValMsgDto;

        public PerfilController(IPerfilService perfilService, IValidationMessageService validationMessageService, IAppLogger logger)
        {
            _perfilService = perfilService;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _Logger = logger;
            _Logger.LogInfo("| ** API.BS.GestionManagement ** |");
        }

        /// <summary>
        /// Listado de Perfiles.
        /// </summary>
        /// <remarks>
        /// Listado de Perfiles.
        /// </remarks>
        /// <response code="200">Listado de Perfiles.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado de Perfiles")]
        [HttpGet("GetPerfiles")]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetPerfilesResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetPerfilesResponseDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetPerfilesResponseDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPerfilesAsync()
        {
            _Logger.LogInfo($"GetPerfiles|Begin|GetPerfilesAsync|request:");
            var result = await _perfilService.GetPerfilesAsync();
            _Logger.LogInfo($"GetPerfiles|End|GetPerfilesAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Listado de Perfiles - Mantenimiento.
        /// </summary>
        /// <remarks>
        /// Listado de Perfiles - Mantenimiento.
        /// </remarks>
        /// <response code="200">Listado de Perfiles - Mantenimiento.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado de Perfiles - Mantenimiento")]
        [HttpGet]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetPerfilesListadoResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetPerfilesListadoResponseDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetPerfilesListadoResponseDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPerfilesListadoAsync([FromQuery] GetPerfilesListadoRequestDto perfilDto)
        {
            _Logger.LogInfo($"GetPerfilesListado|Begin|GetPerfilesListadoAsync|request: {JsonSerializer.Serialize(perfilDto)}");
            var result = await _perfilService.GetPerfilesListadoAsync(perfilDto);
            _Logger.LogInfo($"GetPerfilesListado|End|GetPerfilesListadoAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Obtiene el PERFIL registrado.
        /// </summary>
        /// <remarks>
        /// Obtiene el PERFIL registrado.
        /// </remarks>
        /// <response code="200">Obtiene el PERFIL registrado.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Obtiene el PERFIL registrado")]
        [HttpGet("{nId_Perfil}")]
        [ProducesResponseType(typeof(ResultDto<GetPerfilByIdResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int nId_Perfil)
        {
            _Logger.LogInfo($"GetPerfilById|Begin|GetPerfilByIdAsync|request:{nId_Perfil}");
            var result = await _perfilService.GetPerfilByIdAsync(nId_Perfil);
            _Logger.LogInfo($"GetPerfilById|End|GetPerfilByIdAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Crear PERFIL.
        /// </summary>
        /// <remarks>
        /// Crear PERFIL.
        /// </remarks>
        /// <response code="200">Crear PERFIL.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ResultDto<CreatePerfilResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePerfilAsync([FromBody] CreatePerfilRequestDto perfilDto)
        {
            _Logger.LogInfo($"CreatePerfil|Begin|CreatePerfilAsync|request: {JsonSerializer.Serialize(perfilDto)}");
            var result = await _perfilService.CreatePerfilAsync(perfilDto);
            _Logger.LogInfo($"CreatePerfil|End|CreatePerfilAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Actualizar PERFIL.
        /// </summary>
        /// <remarks>
        /// Actualizar PERFIL.
        /// </remarks>
        /// <response code="200">Actualizar PERFIL.</response>
        [HttpPut]
        [ProducesResponseType(typeof(ResultDto<EditPerfilResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EditPerfilAsync([FromBody] EditPerfilRequestDto perfilDto)
        {
            _Logger.LogInfo($"EditPerfil|Begin|EditPerfilAsync|request: {JsonSerializer.Serialize(perfilDto)}");
            var result = await _perfilService.EditPerfilAsync(perfilDto);
            _Logger.LogInfo($"EditPerfil|End|EditPerfilAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }
    }
}