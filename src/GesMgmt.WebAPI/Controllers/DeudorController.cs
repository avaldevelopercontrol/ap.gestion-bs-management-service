using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Deudor;
using GesMgmt.Infraestructure.Logger;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using static GesMgmt.Application.DTOs.Deudor.DeudorRequestDto;
using static GesMgmt.Application.DTOs.Deudor.DeudorResponseDto;

namespace GesMgmt.WebAPI.Controllers
{
    [ApiController]
    [Route("v1/Deudor")]
    [Produces("application/json")]
    public class DeudorController : ControllerBase
    {
        private readonly IDeudorService _deudorService;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;
        private ValidationMessageDto _oValMsgDto;

        public DeudorController(IDeudorService deudorService, IValidationMessageService validationMessageService, IAppLogger logger)
        {
            _deudorService = deudorService;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _Logger = logger;
            _Logger.LogInfo("| ** API.BS.GestionManagement ** |");
        }

        [SwaggerOperation(Summary = "[API]: Endpoint Listado Deudores")]
        [HttpGet("GetDeudorAsync")]
        [ProducesResponseType(typeof(ResultDto<GetDeudorResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDeudorAsync([FromQuery] GetDeudorRequestDto deduroDto)
        {
            _Logger.LogInfo($"GetDeudor|Begin|GetDeudorAsync|request: {JsonSerializer.Serialize(deduroDto)}");
            var result = await _deudorService.GetDeudorAsync(deduroDto);
            _Logger.LogInfo($"GetDeudor|End|GetDeudorAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }
    }
}
