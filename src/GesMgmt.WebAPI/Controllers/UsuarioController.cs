using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Usuario;
using GesMgmt.Infraestructure.Logger;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using static GesMgmt.Application.DTOs.Gestion.GestionRequestDto;
using static GesMgmt.Application.DTOs.Gestion.GestionResponseDto;
using static GesMgmt.Application.DTOs.Usuario.UsuarioRequestDto;
using static GesMgmt.Application.DTOs.Usuario.UsuarioResponseDto;

namespace GesMgmt.WebAPI.Controllers
{
    [ApiController]
    [Route("v1/Usuario")]
    [Produces("application/json")]
    public class UsuarioController : Controller
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
        /// Lista de Usuarios - UGrupos - Grupos
        /// </summary>
        /// <remarks>
        /// Lista de Usuarios - UGrupos - Grupos.
        /// </remarks>
        /// <response code="200">Lista de Usuarios - UGrupos - Grupos.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Lista de Usuarios - UGrupos - Grupos")]
        [HttpGet("GetUsuariosGrupo")]
        [ProducesResponseType(typeof(ResultDto<GetUsuariosGrupoResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUsuariosGrupoAsync([FromQuery] GetUsuariosGrupoRequestDto usuariosGrupoDto)
        {
            _Logger.LogInfo($"GetUsuariosGrupo|Begin|GetUsuariosGrupoAsync|request: {JsonSerializer.Serialize(usuariosGrupoDto)}");
            var result = await _usuarioService.GetUsuariosGrupoAsync(usuariosGrupoDto);
            _Logger.LogInfo($"GetUsuariosGrupo|End|GetUsuariosGrupoAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }
    }
}