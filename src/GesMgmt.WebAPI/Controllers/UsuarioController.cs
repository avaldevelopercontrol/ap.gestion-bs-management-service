using GesMgmt.Application.DTOs;
using GesMgmt.Application.Interfaces;
using GesMgmt.Application.Interfaces.Usuario;
using GesMgmt.Infraestructure.Logger;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using static GesMgmt.Application.DTOs.Usuario.UsuarioRequestDto;
using static GesMgmt.Application.DTOs.Usuario.UsuarioResponseDto;

namespace GesMgmt.WebAPI.Controllers
{
    [ApiController]
    [Route("v1/Usuario")]
    [Produces("application/json")]
    public class UsuarioController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IValidationMessageService _validationMessageService;
        private readonly IAppLogger _Logger;
        private ValidationMessageDto _oValMsgDto;

        public UsuarioController(IUsuarioService usuarioService, IValidationMessageService validationMessageService, IAppLogger logger)
        {
            _usuarioService = usuarioService;
            _validationMessageService = validationMessageService;
            _oValMsgDto = new ValidationMessageDto();
            _Logger = logger;
            _Logger.LogInfo("| ** API.BS.GestionManagement ** |");
        }

        /// <summary>
        /// Valida el Login Usuario y Clave.
        /// </summary>
        /// <remarks>
        /// Valida el Login Usuario y Clave.
        /// </remarks>
        /// <response code="200">Valida el Login Usuario y Clave.</response>
        [SwaggerOperation(Summary = "[API]: Endpoint Login de Usuario y Clave")]
        [HttpGet("GetLoginUsuario")]
        [ProducesResponseType(typeof(ResultDto<GetUsuarioLoginResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResultDto<>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetLoginUsuarioAsync([FromQuery] GetUsuarioLoginRequestDto usuarioDeudorDto)
        {
            _Logger.LogInfo($"GetLoginUsuario|Begin|GetLoginUsuarioAsync|request: {JsonSerializer.Serialize(usuarioDeudorDto)}");
            var result = await _usuarioService.GetLoginUsuarioAsync(usuarioDeudorDto);
            _Logger.LogInfo($"GetLoginUsuario|End|GetLoginUsuarioAsync|response: {JsonSerializer.Serialize(result)}");
            return Ok(result);
        }
    }
}