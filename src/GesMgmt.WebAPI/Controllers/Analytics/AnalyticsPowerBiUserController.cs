using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Application.Interfaces.Analytics;
using Microsoft.AspNetCore.Mvc;

namespace GesMgmt.WebAPI.Controllers.Analytics;

[Route("api/v1/analytics-access/user")]
public sealed class AnalyticsPowerBiUserController(
    IAnalyticsUserContext userContext,
    IAnalyticsPowerBiUserAccessService accessService,
    IAnalyticsOptionService optionService,
    IAnalyticsPowerBiViewerContextService contextService) : AnalyticsControllerBase
{
    private const int MaxOptionIds = 100;

    [HttpGet("power-bi-access")]
    [ProducesResponseType(typeof(AnalyticsPowerBiAccessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetAccessAsync(
        [FromQuery] string? optionIds,
        CancellationToken cancellationToken)
    {
        if (!TryParseOptionIds(optionIds, out var normalizedOptionIds))
        {
            return AnalyticsProblem(
                StatusCodes.Status400BadRequest,
                "Opciones Analytics inválidas",
                $"optionIds debe contener entre 1 y {MaxOptionIds} enteros positivos separados por coma.");
        }

        var identityError = RequireUser(userContext, out var userId);
        if (identityError is not null)
        {
            return identityError;
        }

        var access = await accessService.ResolveAsync(
            userId,
            normalizedOptionIds,
            cancellationToken);

        return Ok(new AnalyticsPowerBiAccessResponse(
            access.Select(option => new AnalyticsPowerBiOptionAccessResponse(
                    option.OptionId,
                    option.Allowed,
                    option.RequiresClientSelection))
                .ToArray()));
    }

    [HttpGet("options/{optionId:int}/power-bi-viewer-context")]
    [ProducesResponseType(typeof(AnalyticsPowerBiViewerContextResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetViewerContextAsync(
        int optionId,
        [FromQuery] int? clientId,
        [FromQuery] string? reportClient,
        CancellationToken cancellationToken)
    {
        if (optionId <= 0)
        {
            return AnalyticsProblem(
                StatusCodes.Status400BadRequest,
                "Opción Analytics inválida",
                "optionId debe ser un entero positivo.");
        }

        var identityError = RequireUser(userContext, out var userId);
        if (identityError is not null)
        {
            return identityError;
        }

        if (!await optionService.IsActiveAsync(optionId, cancellationToken))
        {
            return Ok(new AnalyticsPowerBiViewerContextResponse(
                optionId,
                Allowed: false,
                RequiresClientSelection: false,
                ClientSelectionStatus: AnalyticsPowerBiClientSelectionStatus.NotRequired,
                SelectedClient: null,
                EmbedUrl: null));
        }

        var selection = BuildSelection(clientId, reportClient);
        var context = await contextService.ResolveAsync(
            userId,
            optionId,
            selection,
            cancellationToken);

        return Ok(new AnalyticsPowerBiViewerContextResponse(
            optionId,
            context.Allowed,
            context.RequiresClientSelection,
            context.ClientSelectionStatus,
            context.SelectedClient,
            context.EmbedUrl));
    }

    private static bool TryParseOptionIds(
        string? value,
        out int[] optionIds)
    {
        optionIds = Array.Empty<int>();

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var parts = value.Split(
            ',',
            StringSplitOptions.RemoveEmptyEntries |
            StringSplitOptions.TrimEntries);

        if (parts.Length == 0 || parts.Length > MaxOptionIds)
        {
            return false;
        }

        var parsed = new HashSet<int>();

        foreach (var part in parts)
        {
            if (!int.TryParse(part, out var optionId) || optionId <= 0)
            {
                return false;
            }

            parsed.Add(optionId);
        }

        if (parsed.Count == 0 || parsed.Count > MaxOptionIds)
        {
            return false;
        }

        optionIds = parsed.OrderBy(optionId => optionId).ToArray();
        return true;
    }

    private static AnalyticsPowerBiViewerSelection? BuildSelection(
        int? clientId,
        string? reportClient)
    {
        var name = reportClient?.Trim() ?? string.Empty;

        return clientId is > 0 && name.Length > 0
            ? new AnalyticsPowerBiViewerSelection(clientId.Value, name)
            : null;
    }
}
