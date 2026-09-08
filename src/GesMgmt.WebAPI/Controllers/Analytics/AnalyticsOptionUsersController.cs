using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Application.Interfaces.Analytics;
using Microsoft.AspNetCore.Mvc;

namespace GesMgmt.WebAPI.Controllers.Analytics;

[Route("api/v1/analytics-access/options/{optionId:int}/users")]
public sealed class AnalyticsOptionUsersController(
    IAnalyticsUserContext userContext,
    IAnalyticsAuthorizationService authorizationService,
    IAnalyticsUserOptionAdministrationService service) : AnalyticsControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(AnalyticsOptionUsersResponse), StatusCodes.Status200OK)]
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

        if (optionId <= 0)
        {
            return AnalyticsProblem(
                StatusCodes.Status400BadRequest,
                "Opción Analytics inválida",
                "optionId debe ser un entero positivo.");
        }

        var response = await service.GetAsync(optionId, cancellationToken);
        return response is null ? NotFound() : Ok(response);
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
        [FromBody] UpdateAnalyticsUserOptionsRequest? request,
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
            request.UserIds,
            administrator.UserId,
            cancellationToken);

        return ToCommandResult(result);
    }
}
