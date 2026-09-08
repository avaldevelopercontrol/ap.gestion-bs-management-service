using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Application.Interfaces.Analytics;
using Microsoft.AspNetCore.Mvc;

namespace GesMgmt.WebAPI.Controllers.Analytics;

[Route("api/v1/analytics-access/options/{optionId:int}/power-bi-configuration")]
public sealed class AnalyticsPowerBiConfigurationController(
    IAnalyticsUserContext userContext,
    IAnalyticsAuthorizationService authorizationService,
    IAnalyticsPowerBiConfigurationService service) : AnalyticsControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(AnalyticsPowerBiConfigurationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetAsync(
        int optionId,
        CancellationToken cancellationToken)
    {
        var administrator = await RequireAdministratorAsync(
            userContext,
            authorizationService,
            cancellationToken);

        if (administrator.Error is not null)
        {
            return administrator.Error;
        }

        if (optionId <= 0)
        {
            return AnalyticsProblem(
                StatusCodes.Status400BadRequest,
                "Opción Analytics inválida",
                "optionId debe ser un entero positivo.");
        }

        return Ok(await service.GetAsync(optionId, cancellationToken));
    }

    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> PatchAsync(
        int optionId,
        [FromBody] UpdateAnalyticsPowerBiConfigurationRequest? request,
        CancellationToken cancellationToken)
    {
        var administrator = await RequireAdministratorAsync(
            userContext,
            authorizationService,
            cancellationToken);

        if (administrator.Error is not null)
        {
            return administrator.Error;
        }

        if (optionId <= 0)
        {
            return AnalyticsProblem(
                StatusCodes.Status400BadRequest,
                "Opción Analytics inválida",
                "optionId debe ser un entero positivo.");
        }

        if (request is null)
        {
            return AnalyticsProblem(
                StatusCodes.Status400BadRequest,
                "Solicitud inválida",
                "El cuerpo de la solicitud es obligatorio.");
        }

        var result = await service.UpdateAsync(
            optionId,
            request,
            administrator.UserId,
            cancellationToken);

        return ToCommandResult(result);
    }
}
