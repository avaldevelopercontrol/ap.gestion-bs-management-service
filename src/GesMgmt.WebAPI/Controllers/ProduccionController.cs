using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Produccion;
using GesMgmt.Infraestructure.Logger;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using static GesMgmt.Application.DTOs.Gestion.GestionRequestDto;
using static GesMgmt.Application.DTOs.Gestion.GestionResponseDto;
using static GesMgmt.Application.DTOs.Produccion.ProduccionRequestDto;
using static GesMgmt.Application.DTOs.Produccion.ProduccionResponseDto;

namespace GesMgmt.WebAPI.Controllers
{
    [ApiController]
    [Route("v1/Produccion")]
    [Produces("application/json")]
    public class ProduccionController : ControllerBase
    {
        private readonly IProduccionService _produccionService;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;
        private ValidationMessageDto _oValMsgDto;

        public ProduccionController(IProduccionService produccionService, IValidationMessageService validationMessageService, IAppLogger logger)
        {
            _produccionService = produccionService;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _Logger = logger;
            _Logger.LogInfo("| ** API.BS.GestionManagement ** |");
        }

        #region "LISTA DE PROVINCIAS"
        /// <summary>
        /// Obtiene la Lista de PROVINCIAS.
        /// </summary>
        /// <remarks>
        /// Obtiene la Lista de PROVINCIAS.
        /// </remarks>
        /// <response code="200">Obtiene la Lista de PROVINCIAS.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Obtiene la Lista de PROVINCIAS.")]
        [HttpGet("GetProvinciasProduccion")]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetProvinciasProduccionResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetProvinciasProduccionResponseDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetProvinciasProduccionResponseDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProvinciasAsync()
        {
            _Logger.LogInfo($"GetProvinciasProduccion|Begin|GetProvinciasProduccionAsync|request:");
            var result = await _produccionService.GetProvinciasProduccionAsync();
            _Logger.LogInfo($"GetProvinciasProduccion|End|GetProvinciasProduccionAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }
        #endregion

        #region "LISTA DE CLIENTES ACTIVOS"
        /// <summary>
        /// Obtiene la Lista de CLIENTES ACTIVOS.
        /// </summary>
        /// <remarks>
        /// Obtiene la Lista de CLIENTES ACTIVOS.
        /// </remarks>
        /// <response code="200">Obtiene la Lista de CLIENTES ACTIVOS.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Obtiene la Lista de CLIENTES ACTIVOS.")]
        [HttpGet("GetClientesProduccionActivos")]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetClientesProduccionActivosResponsetDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetClientesProduccionActivosResponsetDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetClientesProduccionActivosResponsetDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetClientesActivosAsync()
        {
            _Logger.LogInfo($"GetClientesProduccionActivos|Begin|GetClientesProduccionActivosAsync|request:");
            var result = await _produccionService.GetClientesProduccionActivosAsync();
            _Logger.LogInfo($"GetClientesProduccionActivos|End|GetClientesProduccionActivosAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }
        #endregion

        #region "LISTA DE PERFILES"
        /// <summary>
        /// Obtiene la Lista de PERFILES ACTIVOS.
        /// </summary>
        /// <remarks>
        /// Obtiene la Lista de PERFILES ACTIVOS.
        /// </remarks>
        /// <response code="200">Obtiene la Lista de PERFILES ACTIVOS.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Obtiene la Lista de PERFILES ACTIVOS.")]
        [HttpGet("GetPerfilesProduccion")]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetPerfilesActivosResponsetDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetPerfilesActivosResponsetDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetPerfilesActivosResponsetDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPerfilesProduccionAsync()
        {
            _Logger.LogInfo($"GetPerfilesProduccion|Begin|GetPerfilesProduccionAsync|request:");
            var result = await _produccionService.GetPerfilesProduccionAsync();
            _Logger.LogInfo($"GetPerfilesProduccion|End|GetPerfilesProduccionAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }
        #endregion

        #region "LISTA DE PRODUCCION RESUMEN"
        /// <summary>
        /// Listado de Producción Resumen.
        /// </summary>
        /// <remarks>
        /// Listado de Producción Resumen.
        /// </remarks>
        /// <response code="200">Listado de Producción Resumen.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado de Producción Resumen")]
        [HttpGet("GetProduccionResumen")]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetProduccionResumenResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetProduccionResumenResponseDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetProduccionResumenResponseDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProduccionResumenAsync([FromQuery] GetProduccionResumenRequestDto produccionResumenRequestDto)
        {
            _Logger.LogInfo($"GetProduccionResumen|Begin|GetProduccionResumenAsync|request: {JsonSerializer.Serialize(produccionResumenRequestDto)}");
            var result = await _produccionService.GetProduccionResumenAsync(produccionResumenRequestDto);
            _Logger.LogInfo($"GetProduccionResumen|End|GetProduccionResumenAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }
        #endregion
    }
}