using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Agenda;
using GesMgmt.Infraestructure.Logger;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using static GesMgmt.Application.DTOs.Agenda.AgendaRequestDto;
using static GesMgmt.Application.DTOs.Agenda.AgendaResponseDto;

namespace GesMgmt.WebAPI.Controllers
{
    [ApiController]
    [Route("v1/Agenda")]
    [Produces("application/json")]
    public class AgendaController : Controller
    {
        private readonly IAgendaService _agendaService;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;
        private ValidationMessageDto _oValMsgDto;

        public AgendaController(IAgendaService agendaService, IValidationMessageService validationMessageService, IAppLogger logger)
        {
            _agendaService = agendaService;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _Logger = logger;
            _Logger.LogInfo("| ** API.BS.GestionManagement ** |");
        }

        /// <summary>
        /// Crear registro de AGENDA.
        /// </summary>
        /// <remarks>
        /// Crear registro de AGENDA.
        /// </remarks>
        /// <response code="200">Crear registro de EMAILS por IDDEUDOR.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ResultDto<CreateAgendaResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateEmailAsync([FromBody] CreateAgendaRequestDto agendaDto)
        {
            _Logger.LogInfo($"CreateAgenda|Begin|CreateAgendaAsync|request: {JsonSerializer.Serialize(agendaDto)}");
            var result = await _agendaService.CreateAgendaAsync(agendaDto);
            _Logger.LogInfo($"CreateAgenda|End|CreateAgendaAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }
    }
}
