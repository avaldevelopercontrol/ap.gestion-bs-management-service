using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Email;
using GesMgmt.Infraestructure.Logger;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using static GesMgmt.Application.DTOs.Email.EmailRequestDto;
using static GesMgmt.Application.DTOs.Email.EmailResponseDto;
using static GesMgmt.Application.DTOs.Gestion.GestionRequestDto;
using static GesMgmt.Application.DTOs.Gestion.GestionResponseDto;
using static GesMgmt.Application.DTOs.Telefono.TelefonoResponseDto;

namespace GesMgmt.WebAPI.Controllers
{
    [ApiController]
    [Route("v1/Email")]
    [Produces("application/json")]
    public class EmailController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;
        private ValidationMessageDto _oValMsgDto;

        public EmailController(IEmailService telefonoService, IValidationMessageService validationMessageService, IAppLogger logger)
        {
            _emailService = telefonoService;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _Logger = logger;
            _Logger.LogInfo("| ** API.BS.GestionManagement ** |");
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Email Por Id")]
        [HttpGet("{nId_PersEmails}")]
        [ProducesResponseType(typeof(ResultDto<GetPersEmailsResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int nId_PersEmails)
        {
            _Logger.LogInfo($"GetEmailsByIdEmailPers|Begin|GetEmailsByIdEmailPersAsync|request:");
            var result = await _emailService.GetEmailsByIdEmailPersAsync(nId_PersEmails);
            _Logger.LogInfo($"GetEmailsByIdEmailPers|End|GetEmailsByIdEmailPersAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Listado Emails de Deudor")]
        [HttpGet("GetEmailsByIdDeudor")]
        [ProducesResponseType(typeof(ResultDto<GetEmailsPersDeudorResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEmailsByIdDeudorAsync([FromQuery] GetEmailsPersDeudorRequestDto gestionEmailsDto)
        {
            _Logger.LogInfo($"GetEmailsByIdDeudor|Begin|GetEmailsByIdDeudorAsync|request: {JsonSerializer.Serialize(gestionEmailsDto)}");
            var result = await _emailService.GetEmailsByIdDeudorAsync(gestionEmailsDto);
            _Logger.LogInfo($"GetEmailsByIdDeudor|End|GetEmailsByIdDeudorAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

    }
}
