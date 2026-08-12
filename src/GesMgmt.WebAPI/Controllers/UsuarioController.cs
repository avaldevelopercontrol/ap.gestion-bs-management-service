using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Usuario;
using GesMgmt.Infraestructure.Logger;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using static GesMgmt.Application.DTOs.Usuario.UsuarioRequestDto;
using static GesMgmt.Application.DTOs.Usuario.UsuarioResponseDto;

namespace GesMgmt.WebAPI.Controllers
{
    [ApiController]
    [Route("v1/Usuario")]
    [Produces("application/json")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;
        private ValidationMessageDto _oValMsgDto;

        public UsuarioController(IUsuarioService usuarioService, IValidationMessageService validationMessageService, IAppLogger logger)
        {
            _usuarioService = usuarioService;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _Logger = logger;
            _Logger.LogInfo("| ** API.BS.GestionManagement ** |");
        }

        /// <summary>
        /// Lista de Usuarios.
        /// </summary>
        /// <remarks>
        /// Lista de Usuarios.
        /// </remarks>
        /// <response code="200">Lista de Usuarios.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Lista de Usuarios")]
        [HttpGet("GetUsuariosList")]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetUsuariosListResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetUsuariosListResponseDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetUsuariosListResponseDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUsuariosListAsync()
        {
            _Logger.LogInfo($"GetUsuariosList|Begin|GetUsuariosListAsync|request:");
            var result = await _usuarioService.GetUsuariosListAsync();
            _Logger.LogInfo($"GetUsuariosList|End|GetUsuariosListAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Valida el Login Usuario y Clave.
        /// </summary>
        /// <remarks>
        /// Valida el Login Usuario y Clave.
        /// </remarks>
        /// <response code="200">Valida el Login Usuario y Clave.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Login de Usuario y Clave")]
        [HttpGet("GetLoginUsuario")]
        [ProducesResponseType(typeof(ResultDto<GetUsuarioLoginResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetLoginUsuarioAsync([FromQuery] GetUsuarioLoginRequestDto usuarioDeudorDto)
        {
            _Logger.LogInfo($"GetLoginUsuario|Begin|GetLoginUsuarioAsync|request: {JsonSerializer.Serialize(usuarioDeudorDto)}");
            var result = await _usuarioService.GetLoginUsuarioAsync(usuarioDeudorDto);
            _Logger.LogInfo($"GetLoginUsuario|End|GetLoginUsuarioAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        /// <summary>
        /// Lista de Sub Zonas Generales.
        /// </summary>
        /// <remarks>
        /// Lista de Sub Zonas Generales.
        /// </remarks>
        /// <response code="200">Lista de Sub Zonas Generales.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Lista de Sub Zonas Generales")]
        [HttpGet("GetSubZonasGeneral")]
        [ProducesResponseType(typeof(ResultListaDto<IEnumerable<GetSubZonaGeneralListResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultListaDto<IEnumerable<GetSubZonaGeneralListResponseDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultListaDto<IEnumerable<GetSubZonaGeneralListResponseDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSubZonasGeneralAsync()
        {
            _Logger.LogInfo($"GetSubZonasGeneral|Begin|GetSubZonasGeneralAsync|request: {JsonSerializer.Serialize(new { })}");
            var result = await _usuarioService.GetSubZonasGeneralAsync();
            _Logger.LogInfo($"GetSubZonasGeneral|End|GetSubZonasGeneralAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Lista de Campañas Discador x Usuario.
        /// </summary>
        /// <remarks>
        /// Lista de Campañas Discador x Usuario.
        /// </remarks>
        /// <response code="200">Lista de Campañas Discador x Usuario.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Lista de Campañas Discador x Usuario")]
        [HttpGet("GetCampannaDiscadorByIdUsuario")]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetCampannaDiscadorlListResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetCampannaDiscadorlListResponseDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetCampannaDiscadorlListResponseDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCampannaDiscadorByIdUsuarioAsync([FromQuery] GetCampannaDiscadorlListRequestDto camannaDiscadorDto)
        {
            _Logger.LogInfo($"GetCampannaDiscadorByIdUsuario|Begin|GetCampannaDiscadorByIdUsuarioAsync|request: {JsonSerializer.Serialize(camannaDiscadorDto)}");
            var result = await _usuarioService.GetCampannaDiscadorByIdUsuarioAsync(camannaDiscadorDto);
            _Logger.LogInfo($"GetCampannaDiscadorByIdUsuario|End|GetCampannaDiscadorByIdUsuarioAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Crear registro de USUARIO.
        /// </summary>
        /// <remarks>
        /// Crear registro de USUARIO.
        /// </remarks>
        /// <response code="200">Crear registro de USUARIO.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ResultDto<CreateUsuarioResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateUsuarioAsync([FromBody] CreateUsuarioRequestDto usuarioDto)
        {
            _Logger.LogInfo($"CreateUsuario|Begin|CreateUsuarioAsync|request: {JsonSerializer.Serialize(usuarioDto)}");
            var result = await _usuarioService.CreateUsuarioAsync(usuarioDto);
            _Logger.LogInfo($"CreateUsuario|End|CreateUsuarioAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Resetear clave de USUARIO.
        /// </summary>
        /// <remarks>
        /// Resetear clave de USUARIO.
        /// </remarks>
        /// <response code="200">Resetear clave de USUARIO.</response>
        [HttpPut("ResetearUsuarioAsync")]
        [ProducesResponseType(typeof(ResultDto<ResetearUsuarioResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ResetearUsuarioAsync([FromBody] ResetearUsuarioRequestDto usuarioDto)
        {
            _Logger.LogInfo($"ResetearUsuario|Begin|ResetearUsuarioAsync|request: {JsonSerializer.Serialize(usuarioDto)}");
            var result = await _usuarioService.ResetearUsuarioAsync(usuarioDto);
            _Logger.LogInfo($"ResetearUsuario|End|ResetearUsuarioAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }
    }
}