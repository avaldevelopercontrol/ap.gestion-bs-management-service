using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Cliente;
using GesMgmt.Infraestructure.Logger;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using static GesMgmt.Application.DTOs.Cliente.ClienteResponseDto;

namespace GesMgmt.WebAPI.Controllers
{
    [ApiController]
    [Route("v1/Cliente")]
    [Produces("application/json")]

    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;
        private ValidationMessageDto _oValMsgDto;

        public ClienteController(IClienteService clienteService, IValidationMessageService validationMessageService, IAppLogger logger)
        {
            _clienteService = clienteService;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _Logger = logger;
            _Logger.LogInfo("| ** API.BS.GestionManagement ** |");
        }

        /// <summary>
        /// Listado de Clientes Activos.
        /// </summary>
        /// <remarks>
        /// Listado de Clientes Activos.
        /// </remarks>
        /// <response code="200">Listado de Clientes Activos.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado de Clientes Activos")]
        [HttpGet("GetClientesActivos")]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetClientesActivosResponsetDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetClientesActivosResponsetDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetClientesActivosResponsetDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetClientesActivosAsync()
        {
            _Logger.LogInfo($"GetClientesActivos|Begin|GetClientesActivosAsync|request:");
            var result = await _clienteService.GetClientesActivosAsync();
            _Logger.LogInfo($"GetClientesActivos|End|GetClientesActivosAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }
    }
}
