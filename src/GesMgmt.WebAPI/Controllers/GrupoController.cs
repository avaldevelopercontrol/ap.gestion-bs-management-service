using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Grupo;
using GesMgmt.Infraestructure.Logger;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using static GesMgmt.Application.DTOs.Grupo.GrupoRequestDto;
using static GesMgmt.Application.DTOs.Grupo.GrupoResponseDto;

namespace GesMgmt.WebAPI.Controllers
{
    [ApiController]
    [Route("v1/Grupo")]
    [Produces("application/json")]
    public class GrupoController : ControllerBase
    {
        private readonly IGrupoService _grupoService;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;
        private ValidationMessageDto _oValMsgDto;

        public GrupoController(IGrupoService grupoService, IValidationMessageService validationMessageService, IAppLogger logger)
        {
            _grupoService = grupoService;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _Logger = logger;
            _Logger.LogInfo("| ** API.BS.GestionManagement ** |");
        }

        /// <summary>
        /// Listado de Grupos.
        /// </summary>
        /// <remarks>
        /// Listado de Grupos.
        /// </remarks>
        /// <response code="200">Listado de Grupos.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado de Grupos")]
        [HttpGet("GetGrupos")]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetGrupoListResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetGrupoListResponseDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetGrupoListResponseDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGruposAsync()
        {
            _Logger.LogInfo($"GetGrupos|Begin|GetGruposAsync|request:");
            var result = await _grupoService.GetGruposAsync();
            _Logger.LogInfo($"GetGrupos|End|GetGruposAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        /// <summary>
        /// Listado de Grupos - Mantenimiento.
        /// </summary>
        /// <remarks>
        /// Listado de Grupos - Mantenimiento.
        /// </remarks>
        /// <response code="200">Listado de Grupos - Mantenimiento.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado de Grupos - Mantenimiento")]
        [HttpGet("GetGruposListado")]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetGruposResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetGruposResponseDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetGruposResponseDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGruposListadoAsync()
        {
            _Logger.LogInfo($"GetGruposListado|Begin|GetGruposListadoAsync|request:");
            var result = await _grupoService.GetGruposListadoAsync();
            _Logger.LogInfo($"GetGruposListado|End|GetGruposListadoAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        /// <summary>
        /// Obtiene el GRUPO registrado.
        /// </summary>
        /// <remarks>
        /// Obtiene el GRUPO registrado.
        /// </remarks>
        /// <response code="200">Obtiene el GRUPO registrado.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Obtiene el GRUPO registrado")]
        [HttpGet("{nId_Grupo}")]
        [ProducesResponseType(typeof(ResultDto<GetGrupoByIdResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int nId_Grupo)
        {
            _Logger.LogInfo($"GetGrupoById|Begin|GetGrupoByIdAsync|request:{nId_Grupo}");
            var result = await _grupoService.GetGrupoByIdAsync(nId_Grupo);
            _Logger.LogInfo($"GetGrupoById|End|GetGrupoByIdAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        /// <summary>
        /// Crear GRUPO.
        /// </summary>
        /// <remarks>
        /// Crear GRUPO.
        /// </remarks>
        /// <response code="200">Crear GRUPO.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ResultDto<CreateGrupoResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateGrupoAsync([FromBody] CreateGrupoRequestDto grupoDto)
        {
            _Logger.LogInfo($"CreateGrupo|Begin|CreateGrupoAsync|request: {JsonSerializer.Serialize(grupoDto)}");
            var result = await _grupoService.CreateGrupoAsync(grupoDto);
            _Logger.LogInfo($"CreateGrupo|End|CreateGrupoAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Modificar GRUPO.
        /// </summary>
        /// <remarks>
        /// Modificar GRUPO.
        /// </remarks>
        /// <response code="200">Modificar GRUPO.</response>
        [HttpPut]
        [ProducesResponseType(typeof(ResultDto<EditGrupoResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EditGrupoAsync([FromBody] EditGrupoRequestDto grupoDto)
        {
            _Logger.LogInfo($"EditGrupo|Begin|EditGrupoAsync|request: {JsonSerializer.Serialize(grupoDto)}");
            var result = await _grupoService.EditGrupoAsync(grupoDto);
            _Logger.LogInfo($"EditGrupo|End|EditGrupoAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Listado de Grupo - Cliente Inicia.
        /// </summary>
        /// <remarks>
        /// Listado de Grupo - Cliente Inicia.
        /// </remarks>
        /// <response code="200">Listado de Grupo - Cliente Inicia.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado de Grupos - Cliente Inicia")]
        [HttpGet("GetGruposClienteInicial")]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetGruposClienteInicialResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetGruposClienteInicialResponseDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetGruposClienteInicialResponseDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGruposClienteInicialAsync(int nId_Usuario)
        {
            _Logger.LogInfo($"GetGruposClienteInicial|Begin|GetGruposClienteInicialAsync|request:");
            var result = await _grupoService.GetGruposClienteInicialAsync(nId_Usuario);
            _Logger.LogInfo($"GetGruposClienteInicial|End|GetGruposClienteInicialAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }
    }
}