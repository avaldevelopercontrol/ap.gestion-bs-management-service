using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Application.Interfaces.Analytics;
using Microsoft.AspNetCore.Mvc;

namespace GesMgmt.WebAPI.Controllers.Analytics;

[Route("api/v1/analytics-access/options/{optionId:int}/report-client-embeds")]
public sealed class AnalyticsOptionReportClientEmbedsController(
    IAnalyticsUserContext userContext,
    IAnalyticsAuthorizationService authorizationService,
    IAnalyticsOptionService optionService,
    IAnalyticsOptionReportClientEmbedsAdministrationService service)
    : AnalyticsControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(AnalyticsOptionReportClientEmbedsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        var validation = await ValidateOptionAsync(optionId, cancellationToken);
        if (validation is not null)
        {
            return validation;
        }

        return Ok(await service.GetAsync(optionId, cancellationToken));
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> PutAsync(
        int optionId,
        [FromBody] UpdateAnalyticsOptionReportClientEmbedsRequest? request,
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

        var validation = await ValidateOptionAsync(optionId, cancellationToken);
        if (validation is not null)
        {
            return validation;
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
            request.Publications,
            administrator.UserId,
            cancellationToken);

        return ToCommandResult(result);
    }

    private async Task<IActionResult?> ValidateOptionAsync(
        int optionId,
        CancellationToken cancellationToken)
    {
        if (optionId <= 0)
        {
            return AnalyticsProblem(
                StatusCodes.Status400BadRequest,
                "Opción Analytics inválida",
                "optionId debe ser un entero positivo.");
        }

        return await optionService.ExistsAsync(optionId, cancellationToken)
            ? null
            : NotFound();
    }
}
