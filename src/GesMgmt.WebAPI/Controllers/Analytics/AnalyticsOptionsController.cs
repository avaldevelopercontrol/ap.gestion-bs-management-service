using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Domain.Constants;
using Microsoft.AspNetCore.Mvc;

namespace GesMgmt.WebAPI.Controllers.Analytics;

[Route("api/v1/analytics-access/options")]
public sealed class AnalyticsOptionsController(
    IAnalyticsUserContext userContext,
    IAnalyticsAuthorizationService authorizationService,
    IAnalyticsOptionService optionService) : AnalyticsControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AnalyticsOptionConfigResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var administrator = await RequireAdministratorAsync(
            userContext,
            authorizationService,
            SisgesOptionPermission.Consult,
            cancellationToken);

        if (administrator.Error is not null)
        {
            return administrator.Error;
        }

        var options = await optionService.GetAllAsync(cancellationToken);
        return Ok(options);
    }

    [HttpPut("{optionId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> PutAsync(
        int optionId,
        [FromBody] UpsertAnalyticsOptionRequest? request,
        CancellationToken cancellationToken)
    {
        var administrator = await RequireAdministratorAsync(
            userContext,
            authorizationService,
            SisgesOptionPermission.Edit,
            cancellationToken);

        if (administrator.Error is not null)
        {
            return administrator.Error;
        }

        if (optionId <= 0)
        {
            return BadRequest();
        }

        if (request is null)
        {
            return AnalyticsProblem(
                StatusCodes.Status400BadRequest,
                "Datos de opción inválidos",
                "El cuerpo de la solicitud es obligatorio.");
        }

        var optionCode = request.OptionCode?.Trim();
        var optionName = request.OptionName?.Trim();

        if (string.IsNullOrWhiteSpace(optionCode) ||
            string.IsNullOrWhiteSpace(optionName))
        {
            return AnalyticsProblem(
                StatusCodes.Status400BadRequest,
                "Datos de opción inválidos",
                "optionCode y optionName son obligatorios.");
        }

        await optionService.UpsertAsync(
            optionId,
            optionCode,
            optionName,
            request.IsActive,
            administrator.UserId,
            cancellationToken);

        return NoContent();
    }
}
