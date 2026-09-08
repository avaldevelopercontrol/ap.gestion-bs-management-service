using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Application.Interfaces.Analytics;
using Microsoft.AspNetCore.Mvc;

namespace GesMgmt.WebAPI.Controllers.Analytics;

[Route("api/v1/analytics-access/user/options")]
public sealed class AnalyticsUserOptionsController(
    IAnalyticsUserContext userContext,
    IAnalyticsUserOptionQueryService userOptionService,
    IAnalyticsAccessService accessService,
    IAnalyticsOptionService optionService,
    IAnalyticsOptionAccessService optionAccessService,
    IAnalyticsReportClientConfigurationService reportClientConfigurationService)
    : AnalyticsControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AnalyticsUserOptionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var identityError = RequireUser(userContext, out var userId);
        if (identityError is not null)
        {
            return identityError;
        }

        return Ok(await userOptionService.GetAsync(userId, cancellationToken));
    }

    [HttpGet("{optionId:int}/clients")]
    [ProducesResponseType(typeof(AnalyticsUserAllowedClientsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetAllowedClientsAsync(
        int optionId,
        CancellationToken cancellationToken)
    {
        var validation = ValidateOptionAndUser(optionId, out var userId);
        if (validation is not null)
        {
            return validation;
        }

        var clients = await accessService.GetAllowedClientsAsync(
            userId,
            optionId,
            cancellationToken);

        return Ok(AnalyticsUserAllowedClientsResponse.FromClients(
            optionId,
            clients));
    }

    [HttpGet("{optionId:int}/client-scopes")]
    [ProducesResponseType(typeof(AnalyticsUserAllowedClientsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetClientScopesAsync(
        int optionId,
        CancellationToken cancellationToken)
    {
        var validation = ValidateOptionAndUser(optionId, out var userId);
        if (validation is not null)
        {
            return validation;
        }

        if (!await optionService.IsActiveAsync(optionId, cancellationToken))
        {
            return Ok(AnalyticsUserAllowedClientsResponse.FromIds(
                optionId,
                Array.Empty<int>()));
        }

        var clientIds = await accessService.GetClientScopedClientIdsAsync(
            userId,
            optionId,
            cancellationToken);

        return Ok(AnalyticsUserAllowedClientsResponse.FromIds(
            optionId,
            clientIds));
    }

    [HttpGet("{optionId:int}/group-scopes")]
    [ProducesResponseType(typeof(AnalyticsUserAllowedGroupsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetGroupScopesAsync(
        int optionId,
        CancellationToken cancellationToken)
    {
        var validation = ValidateOptionAndUser(optionId, out var userId);
        if (validation is not null)
        {
            return validation;
        }

        if (!await optionService.IsActiveAsync(optionId, cancellationToken))
        {
            return Ok(new AnalyticsUserAllowedGroupsResponse(
                optionId,
                Allowed: false,
                ScopeMode: "NONE",
                GroupIds: Array.Empty<int>(),
                RequiresClientSelection: false));
        }

        var requiresClientSelection =
            await reportClientConfigurationService.RequiresClientSelectionAsync(
                optionId,
                cancellationToken);
        var access = await optionAccessService.ResolveAsync(
            userId,
            optionId,
            cancellationToken);

        return Ok(new AnalyticsUserAllowedGroupsResponse(
            optionId,
            access.Allowed,
            access.ScopeMode,
            access.MatchedGroupIds,
            requiresClientSelection));
    }

    private IActionResult? ValidateOptionAndUser(
        int optionId,
        out int userId)
    {
        userId = 0;

        if (optionId <= 0)
        {
            return AnalyticsProblem(
                StatusCodes.Status400BadRequest,
                "Opción Analytics inválida",
                "optionId debe ser un entero positivo.");
        }

        return RequireUser(userContext, out userId);
    }
}
