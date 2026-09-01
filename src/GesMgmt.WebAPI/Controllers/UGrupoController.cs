using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.UGrupo;
using GesMgmt.Infraestructure.Logger;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using static GesMgmt.Application.DTOs.UGrupo.UGrupoRequestDto;
using static GesMgmt.Application.DTOs.UGrupo.UGrupoResponseDto;

namespace GesMgmt.WebAPI.Controllers
{
    [ApiController]
    [Route("v1/UGrupo")]
    [Produces("application/json")]
    public class UGrupoController : ControllerBase
    {
        private readonly IUGrupoService _ugrupoService;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;
        private ValidationMessageDto _oValMsgDto;

        public UGrupoController(IUGrupoService ugrupoService, IValidationMessageService validationMessageService, IAppLogger logger)
        {
            _ugrupoService = ugrupoService;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _Logger = logger;
            _Logger.LogInfo("| ** API.BS.GestionManagement ** |");
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
            var result = await _ugrupoService.GetUsuariosGrupoAsync(usuariosGrupoDto);
            _Logger.LogInfo($"GetUsuariosGrupo|End|GetUsuariosGrupoAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Lista de Grupos x Usuario.
        /// </summary>
        /// <remarks>
        /// Lista de Grupos x Usuario.
        /// </remarks>
        /// <response code="200">Lista de Grupos x Usuario.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Lista de Grupos x Usuario")]
        [HttpGet("GetGruposByIdUsuario")]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetGruposByUsuarioResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetGruposByUsuarioResponseDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetGruposByUsuarioResponseDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGruposByIdUsuarioAsync([FromQuery] GetGruposByUsuarioRequestDto usuariosGrupoDto)
        {
            _Logger.LogInfo($"GetGruposByIdUsuario|Begin|GetGruposByIdUsuarioAsync|request: {JsonSerializer.Serialize(usuariosGrupoDto)}");
            var result = await _ugrupoService.GetGruposByIdUsuarioAsync(usuariosGrupoDto);
            _Logger.LogInfo($"GetGruposByIdUsuario|End|GetGruposByIdUsuarioAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Lista de Grupos Faltantes x Usuario.
        /// </summary>
        /// <remarks>
        /// Lista de Grupos Faltantes x Usuario.
        /// </remarks>
        /// <response code="200">Lista de Grupos Faltantes x Usuario.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Lista de Grupos Faltantes x Usuario")]
        [HttpGet("GetGruposFaltantesByIdUsuario")]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetGruposByUsuarioResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetGruposByUsuarioResponseDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetGruposByUsuarioResponseDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGruposFaltantesByIdUsuarioAsync([FromQuery] GetGruposByUsuarioRequestDto usuariosGrupoDto)
        {
            _Logger.LogInfo($"GetGruposFaltantesByIdUsuario|Begin|GetGruposFaltantesByIdUsuarioAsync|request: {JsonSerializer.Serialize(usuariosGrupoDto)}");
            var result = await _ugrupoService.GetGruposFaltantesByIdUsuarioAsync(usuariosGrupoDto);
            _Logger.LogInfo($"GetGruposFaltantesByIdUsuario|End|GetGruposFaltantesByIdUsuarioAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Lista de Usuario y Grupos.
        /// </summary>
        /// <remarks>
        /// Lista de Usuario y Grupos.
        /// </remarks>
        /// <response code="200">Lista de Usuario y Grupos.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Lista de Usuario y Grupos")]
        [HttpGet("GetUsuarioGrupoListadoAsync")]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetUsuarioGrupoListadoResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetUsuarioGrupoListadoResponseDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetUsuarioGrupoListadoResponseDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUsuarioGrupoListadoAsync([FromQuery] GetUsuarioGrupoListadoRequestDto uGrupoDto)
        {
            _Logger.LogInfo($"GetUsuarioGrupoListado|Begin|GetUsuarioGrupoListadoAsync|request: {JsonSerializer.Serialize(uGrupoDto)}");
            var result = await _ugrupoService.GetUsuarioGrupoListadoAsync(uGrupoDto);
            _Logger.LogInfo($"GetUsuarioGrupoListado|End|GetUsuarioGrupoListadoAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Obtiene USUARIO GRUPO registrado.
        /// </summary>
        /// <remarks>
        /// Obtiene USUARIO GRUPO registrado.
        /// </remarks>
        /// <response code="200">Obtiene USUARIO GRUPO registrado.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Obtiene USUARIO GRUPO registrado")]
        [HttpGet("{nId_UGrupo}")]
        [ProducesResponseType(typeof(ResultDto<GetUsuarioGrupoObtenerResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int nId_UGrupo)
        {
            _Logger.LogInfo($"GetUsuarioGrupoObtenerId|Begin|GetUsuarioGrupoObtenerIdAsync|request:{nId_UGrupo}");
            var result = await _ugrupoService.GetUsuarioGrupoObtenerIdAsync(nId_UGrupo);
            _Logger.LogInfo($"GetUsuarioGrupoObtenerId|End|GetUsuarioGrupoObtenerIdAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Crear USUARIO GRUPO.
        /// </summary>
        /// <remarks>
        /// Crear USUARIO GRUPO.
        /// </remarks>
        /// <response code="200">Crear USUARIO GRUPO.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ResultDto<PostUsuarioGrupoCrearResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostUsuarioGrupoCrearAsync([FromBody] PostUsuarioGrupoCrearRequestDto ugrupoDto)
        {
            _Logger.LogInfo($"PostUsuarioGrupoCrear|Begin|PostUsuarioGrupoCrearAsync|request: {JsonSerializer.Serialize(ugrupoDto)}");
            var result = await _ugrupoService.PostUsuarioGrupoCrearAsync(ugrupoDto);
            _Logger.LogInfo($"PostUsuarioGrupoCrear|End|PostUsuarioGrupoCrearAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Editar USUARIO GRUPO.
        /// </summary>
        /// <remarks>
        /// Editar USUARIO GRUPO.
        /// </remarks>
        /// <response code="200">Editar USUARIO GRUPO.</response>
        [HttpPut]
        [ProducesResponseType(typeof(ResultDto<PutUsuarioGrupoModificarResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutUsuarioGrupoModificarAsync([FromBody] PutUsuarioGrupoModificarRequestDto ugrupoDto)
        {
            _Logger.LogInfo($"PutUsuarioGrupoModificar|Begin|PutUsuarioGrupoModificarAsync|request: {JsonSerializer.Serialize(ugrupoDto)}");
            var result = await _ugrupoService.PutUsuarioGrupoModificarAsync(ugrupoDto);
            _Logger.LogInfo($"PutUsuarioGrupoModificar|End|PutUsuarioGrupoModificarAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }
    }
}