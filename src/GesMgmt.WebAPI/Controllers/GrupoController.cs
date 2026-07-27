using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Grupo;
using GesMgmt.Infraestructure.Logger;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using static GesMgmt.Application.DTOs.Gestion.GestionResponseDto;
using static GesMgmt.Application.DTOs.Grupo.GrupoResponseDto;

namespace GesMgmt.WebAPI.Controllers
{
    [ApiController]
    [Route("v1/Grupo")]
    [Produces("application/json")]
    public class GrupoController : ControllerBase
    {
        private readonly IGrupoService _grupoService;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;
        private ValidationMessageDto _oValMsgDto;

        public GrupoController(IGrupoService grupoService, IValidationMessageService validationMessageService, IAppLogger logger)
        {
            _grupoService = grupoService;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _Logger = logger;
            _Logger.LogInfo("| ** API.BS.GestionManagement ** |");
        }

        /// <summary>
        /// Listado de Grupos.
        /// </summary>
        /// <remarks>
        /// Listado de Grupos.
        /// </remarks>
        /// <response code="200">Listado de Grupos.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado de Grupos")]
        [HttpGet("GetGrupos")]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetGrupoListResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetGrupoListResponseDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetGrupoListResponseDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGruposAsync()
        {
            _Logger.LogInfo($"GetGrupos|Begin|GetGruposAsync|request:");
            var result = await _grupoService.GetGruposAsync();
            _Logger.LogInfo($"GetGrupos|End|GetGruposAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }

    }
}
