using Microsoft.AspNetCore.Mvc;
using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Utils;
using GesMgmt.Domain.Constants;
using GesMgmt.Infraestructure.Logger;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;

namespace GesMgmt.WebAPI.Controllers
{
    [ApiController]
    [Route("v1/Gestion")]
    [Produces("application/json")]
    public class GestionController : ControllerBase
    {
        private readonly IGestionService _subscriptionService;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;
        private ValidationMessageDto _oValMsgDto;

        public GestionController(IGestionService suscriptionService, IValidationMessageService validationMessageService, IAppLogger logger)
        {
            _subscriptionService = suscriptionService;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _Logger = logger;
            _Logger.LogInfo("| ** API.BS.GestionManagement ** |");
        }

        [SwaggerOperation(Summary = "[API]: HU: Endpoint Listado Gestiones")]
        [HttpGet]
        public async Task<IActionResult> GetGestionesAsync([FromQuery] GetGestionRequestDto gestionDto)
        {
            _Logger.LogInfo($"GetGestion|Begin|GetGestionAsync|request: {JsonSerializer.Serialize(gestionDto)}");
            var result = await _subscriptionService.GetGestionesAsync(gestionDto);
            _Logger.LogInfo($"GetGestion|End|GetGestionAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

    }
}