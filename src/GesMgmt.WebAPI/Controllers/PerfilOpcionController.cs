using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Perfil;
using GesMgmt.Application.Interfaces.PerfilOpcion;
using GesMgmt.Application.Services.Perfil;
using GesMgmt.Infraestructure.Logger;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using static GesMgmt.Application.DTOs.Perfil.PerfilResponseDto;
using static GesMgmt.Application.DTOs.PerfilOpcion.PerfilOpcionResponseDto;

namespace GesMgmt.WebAPI.Controllers
{
    [ApiController]
    [Route("v1/PerfilOpcion")]
    [Produces("application/json")]
    public class PerfilOpcionController : ControllerBase
    {
        private readonly IPerfilOpcionService _perfilOpcionService;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;
        private ValidationMessageDto _oValMsgDto;

        public PerfilOpcionController(IPerfilOpcionService perfilOpcionService, IValidationMessageService validationMessageService, IAppLogger logger)
        {
            _perfilOpcionService = perfilOpcionService;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _Logger = logger;
            _Logger.LogInfo("| ** API.BS.GestionManagement ** |");
        }

        /// <summary>
        /// Listado de Perfil - Opción.
        /// </summary>
        /// <remarks>
        /// Listado de Perfil - Opción.
        /// </remarks>
        /// <response code="200">Listado de Perfil - Opción.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Listado de Perfil - Opción")]
        [HttpGet("GetPerfilOpciones")]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetPerfilOpcionResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetPerfilOpcionResponseDto>>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultListDto<IEnumerable<GetPerfilOpcionResponseDto>>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPerfilOpcionesAsync()
        {
            _Logger.LogInfo($"GetPerfilOpciones|Begin|GetPerfilOpcionesAsync|request:");
            var result = await _perfilOpcionService.GetPerfilOpcionesAsync();
            _Logger.LogInfo($"GetPerfilOpciones|End|GetPerfilOpcionesAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }
    }
}
