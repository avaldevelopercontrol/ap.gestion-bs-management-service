using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Email;
using GesMgmt.Infraestructure.Logger;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using static GesMgmt.Application.DTOs.Email.EmailRequestDto;
using static GesMgmt.Application.DTOs.Email.EmailResponseDto;

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

        /// <summary>
        /// Obtiene el listado de EMAILS, del Deudor BOTÓN +EMAIL.
        /// </summary>
        /// <remarks>
        /// Obtiene el listado de EMAILS, del Deudor +EMAIL.
        /// </remarks>
        /// <response code="200">Obtiene el listado de EMAILS, del Deudor +EMAIL.</response>
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

        /// <summary>
        /// Obtiene el listado de EMAILS.
        /// </summary>
        /// <remarks>
        /// Obtiene el listado de EMAILS.
        /// </remarks>
        /// <response code="200">Obtiene el listado de EMAILS.</response>
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

        /// <summary>
        /// Crear registro de EMAILS por IDDEUDOR.
        /// </summary>
        /// <remarks>
        /// Crear registro de EMAILS por IDDEUDOR.
        /// </remarks>
        /// <response code="200">Crear registro de EMAILS por IDDEUDOR.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ResultDto<CreateEmailResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateEmailAsync([FromBody] CreateEmailRequestDto emailDto)
        {
            _Logger.LogInfo($"CreateEmail|Begin|CreateEmailAsync|request: {JsonSerializer.Serialize(emailDto)}");
            var result = await _emailService.CreateEmailAsync(emailDto);
            _Logger.LogInfo($"CreateEmail|End|CreateEmailAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Editar registro de EMAILS por IDDEUDOR.
        /// </summary>
        /// <remarks>
        /// Editar registro de EMAILS por IDDEUDOR.
        /// </remarks>
        /// <response code="200">Editar registro de EMAILS por IDDEUDOR.</response>
        [HttpPut]
        [ProducesResponseType(typeof(ResultDto<EditEmailResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EditDireccionAsync([FromBody] EditEmailRequestDto emailDto)
        {
            _Logger.LogInfo($"EditEmail|Begin|EditEmailAsync|request: {JsonSerializer.Serialize(emailDto)}");
            var result = await _emailService.EditEmailAsync(emailDto);
            _Logger.LogInfo($"EditEmail|End|EditEmailAsync|response: {JsonSerializer.Serialize(result)}");
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Obtiene el listado de Status.
        /// </summary>
        /// <remarks>
        /// Obtiene el listado de Status.
        /// </remarks>
        /// <response code="200">Obtiene el listado de Status.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado Status")]
        [HttpGet("GetStatus")]
        [ProducesResponseType(typeof(ResultDto<GetStatus>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStatusAsync()
        {
            _Logger.LogInfo($"GetStatus|Begin|GetStatusAsync|request:");
            var result = await _emailService.GetStatusAsync();
            _Logger.LogInfo($"GetStatus|End|GetStatusAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }
    }
}
