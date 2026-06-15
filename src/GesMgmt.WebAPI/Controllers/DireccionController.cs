using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Direccion;
using GesMgmt.Application.Interfaces.Telefono;
using GesMgmt.Application.Services.Telefono;
using GesMgmt.Infraestructure.Logger;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
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

        [SwaggerOperation(Summary = "[API]: Endpoint Listado Departamentos de Ubigeo")]
        [HttpGet("GetUbigeoDepartamentos")]
        [ProducesResponseType(typeof(ResultDto<GetUbigeoDepartamentos>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUbigeoDepartamentosAsync()
        {
            _Logger.LogInfo($"GetUbigeoDepartamentos|Begin|GetUbigeoDepartamentosAsync|request:");
            var result = await _direccionService.GetUbigeoDepartamentosAsync();
            _Logger.LogInfo($"GetUbigeoDepartamentos|End|GetUbigeoDepartamentosAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Obtener Dirección")]
        [HttpGet("GetUbigeoProvincias")]
        [ProducesResponseType(typeof(ResultDto<GetUbigeoProvincias>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUbigeoProvinciasAsync([FromHeader] int nId_Departamento)
        {
            _Logger.LogInfo($"GetUbigeoProvincias|Begin|GetUbigeoProvinciasAsync|request:{nId_Departamento}");
            var result = await _direccionService.GetUbigeoProvinciasAsync(nId_Departamento);
            _Logger.LogInfo($"GetUbigeoProvinciasAsync|End|GetUbigeoProvinciasAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Listado Distritos por Provincias y Departamentos de Ubigeo")]
        [HttpGet("GetUbigeoDistritos")]
        [ProducesResponseType(typeof(ResultDto<GetUbigeoDistritos>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUbigeoDistritosAsync([FromHeader] int nId_Departamento, int nId_Provincia)
        {
            _Logger.LogInfo($"GetUbigeoDistritos|Begin|GetUbigeoDistritosAsync|request:{nId_Departamento}");
            var result = await _direccionService.GetUbigeoDistritosAsync(nId_Departamento, nId_Provincia);
            _Logger.LogInfo($"GetUbigeoDistritos|End|GetUbigeoDistritosAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }
    }
}