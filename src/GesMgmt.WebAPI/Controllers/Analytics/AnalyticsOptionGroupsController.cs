using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Application.Interfaces.Analytics;
using Microsoft.AspNetCore.Mvc;

namespace GesMgmt.WebAPI.Controllers.Analytics;

[Route("api/v1/analytics-access/options/{optionId:int}/groups")]
public sealed class AnalyticsOptionGroupsController(
    IAnalyticsUserContext userContext,
    IAnalyticsAuthorizationService authorizationService,
    IAnalyticsOptionGroupAdministrationService service) : AnalyticsControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(AnalyticsOptionGroupsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
            return BadRequest();
        }

        var response = await service.GetAsync(optionId, cancellationToken);
        return response is null ? NotFound() : Ok(response);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> PutAsync(
        int optionId,
        [FromBody] UpdateAnalyticsOptionGroupsRequest? request,
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
            return BadRequest();
        }

        if (request is null)
        {
            return BadRequest();
        }

        var result = await service.UpdateAsync(
            optionId,
            request.GroupIds,
            administrator.UserId,
            cancellationToken);

        return ToCommandResult(result);
    }
}
