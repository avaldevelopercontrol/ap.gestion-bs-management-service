using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Opcion;
using GesMgmt.Application.Logger;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using static GesMgmt.Application.DTOs.Opcion.OpcionResponseDto;

namespace GesMgmt.WebAPI.Controllers
{
    [ApiController]
    [Route("v1/Opcion")]
    [Produces("application/json")]
    public class OpcionController : Controller
    {
        private readonly IOpcionService _opcionService;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;
        private ValidationMessageDto _oValMsgDto;
        public OpcionController(IOpcionService opcionService, IValidationMessageService validationMessageService, IAppLogger logger)
        {
            _opcionService = opcionService;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _Logger = logger;
            _Logger.LogInfo("| ** API.BS.GestionManagement ** |");
        }

        /// <summary>
        /// Listado de Opciones.
        /// </summary>
        /// <remarks>
        /// Listado de Opciones.
        /// </remarks>
        /// <response code="200">Listado de Opciones.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado de Opciones")]
        [HttpGet("GetOpciones")]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetOpcionesResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetOpcionesResponseDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetOpcionesResponseDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetOpcionesAsync()
        {
            _Logger.LogInfo($"GetOpciones|Begin|GetOpcionesAsync|request:");
            var result = await _opcionService.GetOpcionesAsync();
            _Logger.LogInfo($"GetOpciones|End|GetOpcionesAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }
    }
}