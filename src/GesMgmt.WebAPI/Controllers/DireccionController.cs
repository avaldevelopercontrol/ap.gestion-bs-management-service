using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Direccion;
using GesMgmt.Infraestructure.Logger;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using static GesMgmt.Application.DTOs.Direccion.DireccionRequestDto;
using static GesMgmt.Application.DTOs.Direccion.DireccionResponseDto;

namespace GesMgmt.WebAPI.Controllers
{
    [ApiController]
    [Route("v1/Direccion")]
    [Produces("application/json")]
    public class DireccionController : ControllerBase
    {
        private readonly IDireccionService _direccionService;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;
        private ValidationMessageDto _oValMsgDto;

        public DireccionController(IDireccionService direccionService, IValidationMessageService validationMessageService, IAppLogger logger)
        {
            _direccionService = direccionService;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _Logger = logger;
            _Logger.LogInfo("| ** API.BS.GestionManagement ** |");
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Listado Provincias por Departamento de Ubigeo")]
        [HttpGet("{nId_PersDirecc}")]
        [ProducesResponseType(typeof(ResultDto<GetDireccionAsync>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int nId_PersDirecc)
        {
            _Logger.LogInfo($"GetDireccionByIdDireccion|Begin|GetDireccionByIdDireccionAsync|request:{nId_PersDirecc}");
            var result = await _direccionService.GetDireccionByIdDireccionAsync(nId_PersDirecc);
            _Logger.LogInfo($"GetDireccionByIdDireccion|End|GetDireccionByIdDireccionAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResultDto<CreateDireccionResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateDireccionAsync([FromBody] CreateDireccionRequestDto direccionDto)
        {
            _Logger.LogInfo($"CreateDireccion|Begin|CreateDireccionAsync|request: {JsonSerializer.Serialize(direccionDto)}");
            var result = await _direccionService.CreateDireccionAsync(direccionDto);
            _Logger.LogInfo($"CreateDireccion|End|CreateDireccionAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut]
        [ProducesResponseType(typeof(ResultDto<EditDireccionResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EditDireccionAsync([FromBody] EditDireccionRequestDto direccionDto)
        {
            _Logger.LogInfo($"EditDireccion|Begin|EditDireccionAsync|request: {JsonSerializer.Serialize(direccionDto)}");
            var result = await _direccionService.EditDireccionAsync(direccionDto);
            _Logger.LogInfo($"EditDireccion|End|EditDireccionAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Listado Departamentos de Ubigeo")]
        [HttpGet("GetDireccionDepartamentos")]
        [ProducesResponseType(typeof(ResultDto<GetDireccionDepartamentos>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUbigeoDepartamentosAsync()
        {
            _Logger.LogInfo($"GetDireccionDepartamentos|Begin|GetDireccionDepartamentosAsync|request:");
            var result = await _direccionService.GetDireccionDepartamentosAsync();
            _Logger.LogInfo($"GetDireccionDepartamentos|End|GetDireccionDepartamentosAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Obtener Dirección")]
        [HttpGet("GetDireccionProvincias")]
        [ProducesResponseType(typeof(ResultDto<GetDireccionProvincias>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDireccionProvinciasAsync([FromHeader] int nId_Departamento)
        {
            _Logger.LogInfo($"GetDireccionProvincias|Begin|GetDireccionProvinciasAsync|request:{nId_Departamento}");
            var result = await _direccionService.GetDireccionProvinciasAsync(nId_Departamento);
            _Logger.LogInfo($"GetDireccionProvincias|End|GetDireccionProvinciasAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Listado Distritos por Provincias y Departamentos de Ubigeo")]
        [HttpGet("GetDireccionDistritos")]
        [ProducesResponseType(typeof(ResultDto<GetDireccionDistritos>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDireccionDistritosAsync([FromHeader] int nId_Departamento, int nId_Provincia)
        {
            _Logger.LogInfo($"GetDireccionDistritos|Begin|GetDireccionDistritosAsync|request:{nId_Departamento}");
            var result = await _direccionService.GetDireccionDistritosAsync(nId_Departamento, nId_Provincia);
            _Logger.LogInfo($"GetDireccionDistritos|End|GetDireccionDistritosAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Listado Ubicaciones de Direcciones")]
        [HttpGet("GetDireccionUbicaciones")]
        [ProducesResponseType(typeof(ResultDto<GetDireccionUbicaciones>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDireccionUbicacionesAsync()
        {
            _Logger.LogInfo($"GetDireccionUbicaciones|Begin|GetDireccionUbicacionesAsync|request:");
            var result = await _direccionService.GetDireccionUbicacionesAsync();
            _Logger.LogInfo($"GetDireccionUbicaciones|End|GetDireccionUbicacionesAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }
    }
}