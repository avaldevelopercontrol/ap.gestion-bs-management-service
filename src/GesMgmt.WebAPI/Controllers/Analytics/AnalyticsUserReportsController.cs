using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Application.Interfaces.Analytics;
using Microsoft.AspNetCore.Mvc;

namespace GesMgmt.WebAPI.Controllers.Analytics;

[Route("api/v1/analytics-access/user/options/{optionId:int}")]
public sealed class AnalyticsUserReportsController(
    IAnalyticsUserContext userContext,
    IAnalyticsOptionService optionService,
    IAnalyticsReportClientAccessService accessService,
    IAnalyticsReportClientEmbedLookupService embedService) : AnalyticsControllerBase
{
    [HttpGet("report-clients")]
    [ProducesResponseType(typeof(AnalyticsReportClientsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetReportClientsAsync(
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
            return NotFound();
        }

        var access = await accessService.ResolveAsync(
            userId,
            optionId,
            cancellationToken);

        if (!access.HasOptionAccess)
        {
            return AnalyticsProblem(
                StatusCodes.Status403Forbidden,
                "Acceso al reporte denegado",
                "El usuario no pertenece al grupo habilitado para este reporte.");
        }

        return Ok(new AnalyticsReportClientsResponse(
            optionId,
            access.Clients));
    }

    [HttpGet("report-client-embed")]
    [ProducesResponseType(typeof(AnalyticsReportClientEmbedResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetReportClientEmbedAsync(
        int optionId,
        [FromQuery] int clientId,
        [FromQuery] string? reportClient,
        CancellationToken cancellationToken)
    {
        var requestedName = reportClient?.Trim() ?? string.Empty;

        if (optionId <= 0 || clientId <= 0 || requestedName.Length == 0)
        {
            return AnalyticsProblem(
                StatusCodes.Status400BadRequest,
                "Selección de cartera inválida",
                "optionId, clientId y reportClient son obligatorios y deben ser válidos.");
        }

        var identityError = RequireUser(userContext, out var userId);
        if (identityError is not null)
        {
            return identityError;
        }

        if (!await optionService.IsActiveAsync(optionId, cancellationToken))
        {
            return NotFound();
        }

        var access = await accessService.ResolveAsync(
            userId,
            optionId,
            cancellationToken);

        if (!access.HasOptionAccess)
        {
            return AnalyticsProblem(
                StatusCodes.Status403Forbidden,
                "Acceso al reporte denegado",
                "El usuario no pertenece al grupo habilitado para este reporte.");
        }

        var authorizedClient = access.Clients.FirstOrDefault(client =>
            client.ClientId == clientId &&
            string.Equals(
                client.Name.Trim(),
                requestedName,
                StringComparison.OrdinalIgnoreCase));

        if (authorizedClient is null)
        {
            return AnalyticsProblem(
                StatusCodes.Status403Forbidden,
                "Cartera no autorizada",
                "La cartera solicitada no está habilitada para el usuario.");
        }

        var embed = await embedService.ResolveAsync(
            optionId,
            authorizedClient.ClientId,
            authorizedClient.Name,
            cancellationToken);

        if (embed.Status == AnalyticsReportClientEmbedLookupStatus.NotFound)
        {
            return AnalyticsProblem(
                StatusCodes.Status404NotFound,
                "Publicación no configurada",
                "La cartera todavía no tiene una URL Publish to web asignada para este reporte.");
        }

        if (embed.Status == AnalyticsReportClientEmbedLookupStatus.InvalidConfiguration)
        {
            return AnalyticsProblem(
                StatusCodes.Status500InternalServerError,
                "Publicación Power BI inválida",
                "La URL configurada para la cartera no es una publicación válida de Power BI.");
        }

        return Ok(new AnalyticsReportClientEmbedResponse(
            optionId,
            authorizedClient.ClientId,
            authorizedClient.Name,
            embed.EmbedUrl!));
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
