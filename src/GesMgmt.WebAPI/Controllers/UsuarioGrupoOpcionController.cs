using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.UsuarioGrupoOpcion;
using GesMgmt.Infraestructure.Logger;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using static GesMgmt.Application.DTOs.UsuarioGrupoOpcion.UsuarioGrupoOpcionRequestDto;
using static GesMgmt.Application.DTOs.UsuarioGrupoOpcion.UsuarioGrupoOpcionResponseDto;

namespace GesMgmt.WebAPI.Controllers
{
    [ApiController]
    [Route("v1/UsuarioGrupoOpcion")]
    [Produces("application/json")]
    public class UsuarioGrupoOpcionController : Controller
    {
        private readonly IUsuarioGrupoOpcionService _usuariogrupoopcionService;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;
        private ValidationMessageDto _oValMsgDto;

        public UsuarioGrupoOpcionController(IUsuarioGrupoOpcionService usuariogrupoopcionService, IValidationMessageService validationMessageService, IAppLogger logger)
        {
            _usuariogrupoopcionService = usuariogrupoopcionService;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _Logger = logger;
            _Logger.LogInfo("| ** API.BS.GestionManagement ** |");
        }

        /// <summary>
        /// Lista de Usuarios - Grupos - Opciones
        /// </summary>
        /// <remarks>
        /// Lista de Usuarios - Grupos - Opciones.
        /// </remarks>
        /// <response code="200">Lista de Usuarios - Grupos - Opciones.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Lista de Usuarios - Grupos - Opciones")]
        [HttpGet("GetUsuarioGrupoOpcionListado")]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetUsuarioGrupoOpcionListadoResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetUsuarioGrupoOpcionListadoResponseDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetUsuarioGrupoOpcionListadoResponseDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUsuarioGrupoOpcionListadoAsync([FromQuery] GetUsuarioGrupoOpcionListadoRequestDto usuariosGrupoOpcionDto)
        {
            _Logger.LogInfo($"GetUsuarioGrupoOpcionListado|Begin|GetUsuarioGrupoOpcionListadoAsync|request: {JsonSerializer.Serialize(usuariosGrupoOpcionDto)}");
            var result = await _usuariogrupoopcionService.GetUsuarioGrupoOpcionListadoAsync(usuariosGrupoOpcionDto);
            _Logger.LogInfo($"GetUsuarioGrupoOpcionListado|End|GetUsuarioGrupoOpcionListadoAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Lista de Usuarios - Grupos - Opciones por Id Usuario y Id Grupo
        /// </summary>
        /// <remarks>
        /// Lista de Usuarios - Grupos - Opciones por Id Usuario y Id Grupo
        /// </remarks>
        /// <response code="200">Lista de Usuarios - Grupos - Opciones por Id Usuario y Id Grupo.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Lista de Usuarios - Grupos - Opciones por Id Usuario y Id Grupo")]
        [HttpGet("GetByIdUsuarioIdGrupo")]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetUsuarioGrupoOpcionListadoResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetUsuarioGrupoOpcionListadoResponseDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetUsuarioGrupoOpcionListadoResponseDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByIdUsuarioIdGrupoAsync([FromQuery] GetByIdUsuarioIdGrupoAsyncRequestDto usuariosGrupoOpcionDto)
        {
            _Logger.LogInfo($"GetByIdUsuarioIdGrupo|Begin|GetByIdUsuarioIdGrupoAsync|request: {JsonSerializer.Serialize(usuariosGrupoOpcionDto)}");
            var result = await _usuariogrupoopcionService.GetByIdUsuarioIdGrupoAsync(usuariosGrupoOpcionDto);
            _Logger.LogInfo($"GetByIdUsuarioIdGrupo|End|GetByIdUsuarioIdGrupoAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Obtiene Usuario - Grupo - Opción.
        /// </summary>
        /// <remarks>
        /// Obtiene Usuario - Grupo - Opción.
        /// </remarks>
        /// <response code="200">Obtiene Usuario - Grupo - Opción.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Obtiene Usuario - Grupo - Opción")]
        [HttpGet("{nId_UsuarioGrupoOpcion}")]
        [ProducesResponseType(typeof(ResultDto<GetUsuarioGrupoOpcionObtenerResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int nId_UsuarioGrupoOpcion)
        {
            _Logger.LogInfo($"GetUsuarioGrupoOpcionObtenerId|Begin|GetUsuarioGrupoOpcionObtenerIdAsync|request:{nId_UsuarioGrupoOpcion}");
            var result = await _usuariogrupoopcionService.GetUsuarioGrupoOpcionObtenerIdAsync(nId_UsuarioGrupoOpcion);
            _Logger.LogInfo($"  GetUsuarioGrupoOpcionObtenerId|End|GetUsuarioGrupoOpcionObtenerIdAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Crear USUARIO - GRUPO - OPCIÓN.
        /// </summary>
        /// <remarks>
        /// Crear USUARIO - GRUPO - OPCIÓN.
        /// </remarks>
        /// <response code="200">Crear USUARIO - GRUPO - OPCIÓN.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ResultDto<PostUsuarioGrupoOpcionCrearResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PostUsuarioGrupoOpcionCrearAsync([FromBody] PostUsuarioGrupoOpcionCrearRequestDto opcionDto)
        {
            _Logger.LogInfo($"PostUsuarioGrupoOpcionCrear|Begin|PostUsuarioGrupoOpcionCrearAsync|request: {JsonSerializer.Serialize(opcionDto)}");
            var result = await _usuariogrupoopcionService.PostUsuarioGrupoOpcionCrearAsync(opcionDto);
            _Logger.LogInfo($"PostUsuarioGrupoOpcionCrear|End|PostUsuarioGrupoOpcionCrearAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Editar USUARIO - GRUPO - OPCIÓN.
        /// </summary>
        /// <remarks>
        /// Editar USUARIO - GRUPO - OPCIÓN.
        /// </remarks>
        /// <response code="200">Editar USUARIO - GRUPO - OPCIÓN.</response>
        [HttpPut]
        [ProducesResponseType(typeof(ResultDto<PutUsuarioGrupoOpcionModificarResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PutUsuarioGrupoOpcionModificarAsync([FromBody] PutUsuarioGrupoOpcionEditarRequestDto opcionDto)
        {
            _Logger.LogInfo($"PutUsuarioGrupoOpcionModificar|Begin|PutUsuarioGrupoOpcionModificarAsync|request: {JsonSerializer.Serialize(opcionDto)}");
            var result = await _usuariogrupoopcionService.PutUsuarioGrupoOpcionModificarAsync(opcionDto);
            _Logger.LogInfo($"PutUsuarioGrupoOpcionModificar|End|PutUsuarioGrupoOpcionModificarAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }
    }
}