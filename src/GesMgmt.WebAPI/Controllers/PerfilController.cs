using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Perfil;
using GesMgmt.Infraestructure.Logger;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using static GesMgmt.Application.DTOs.Perfil.PerfilResponseDto;

namespace GesMgmt.WebAPI.Controllers
{
    [ApiController]
    [Route("v1/Perfil")]
    [Produces("application/json")]
    public class PerfilController : ControllerBase
    {
        private readonly IPerfilService _perfilService;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;
        private ValidationMessageDto _oValMsgDto;

        public PerfilController(IPerfilService perfilService, IValidationMessageService validationMessageService, IAppLogger logger)
        {
            _perfilService = perfilService;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _Logger = logger;
            _Logger.LogInfo("| ** API.BS.GestionManagement ** |");
        }

        /// <summary>
        /// Listado de Perfiles.
        /// </summary>
        /// <remarks>
        /// Listado de Perfiles.
        /// </remarks>
        /// <response code="200">Listado de Perfiles.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado de Perfiles")]
        [HttpGet("GetPerfiles")]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetPerfilListResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetPerfilListResponseDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetPerfilListResponseDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPerfilesAsync()
        {
            _Logger.LogInfo($"GetPerfiles|Begin|GetPerfilesAsync|request:");
            var result = await _perfilService.GetPerfilesAsync();
            _Logger.LogInfo($"GetPerfiles|End|GetPerfilesAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }
    }
}