using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Cartera;
using GesMgmt.Infraestructure.Logger;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using static GesMgmt.Application.DTOs.Cartera.CarteraResponseDto;

namespace GesMgmt.WebAPI.Controllers
{
    [ApiController]
    [Route("v1/Cartera")]
    [Produces("application/json")]

    public class CarteraController : ControllerBase
    {
        private readonly ICarteraService _carteraService;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;
        private ValidationMessageDto _oValMsgDto;

        public CarteraController(ICarteraService carteraService, IValidationMessageService validationMessageService, IAppLogger logger)
        {
            _carteraService = carteraService;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _Logger = logger;
            _Logger.LogInfo("| ** API.BS.GestionManagement ** |");
        }

        /// <summary>
        /// Listado de Anios de Carteras x Cliente.
        /// </summary>
        /// <remarks>
        /// Listado de Anios de Carteras x Cliente.
        /// </remarks>
        /// <response code="200">Listado de Anios de Carteras x Cliente.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado de Anios de Carteras x Cliente")]
        [HttpGet("GetAnioByIdCliente")]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetAnioByIdClienteResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetAnioByIdClienteResponseDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetAnioByIdClienteResponseDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAnioByIdClienteAsync(int nId_Cliente)
        {
            _Logger.LogInfo($"GetAnioByIdCliente|Begin|GetAnioByIdClienteAsync|request:");
            var result = await _carteraService.GetAnioByIdClienteAsync(nId_Cliente);
            _Logger.LogInfo($"GetAnioByIdCliente|End|GetAnioByIdClienteAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        /// <summary>
        /// Listado de Carteras Parametros x Cliente y Anio.
        /// </summary>
        /// <remarks>
        /// Listado de Carteras Parametros x Cliente y Anio.
        /// </remarks>
        /// <response code="200">Listado de Carteras Parametros x Cliente y Anio.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado de Carteras Parametros x Cliente y Anio")]
        [HttpGet("GetCarterasParametrosByIdClienteAnnio")]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetCarterasParametrosByIdClienteAnnioResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetCarterasParametrosByIdClienteAnnioResponseDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetCarterasParametrosByIdClienteAnnioResponseDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCarterasParametrosByIdClienteAnnioAsync(int nId_Cliente, int anio)
        {
            _Logger.LogInfo($"GetCarterasParametrosByIdClienteAnnio|Begin|GetCarterasParametrosByIdClienteAnnioAsync|request:");
            var result = await _carteraService.GetCarterasParametrosByIdClienteAnnioAsync(nId_Cliente, anio);
            _Logger.LogInfo($"GetCarterasParametrosByIdClienteAnnio|End|GetCarterasParametrosByIdClienteAnnioAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }
    }
}
