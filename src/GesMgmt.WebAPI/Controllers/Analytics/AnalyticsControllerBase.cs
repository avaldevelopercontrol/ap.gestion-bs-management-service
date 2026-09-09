using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GesMgmt.WebAPI.Controllers.Analytics;

[ApiController]
public abstract class AnalyticsControllerBase : ControllerBase
{
    public const string PortfolioConcurrencyPolicyName =
        "portfolio-control-center-concurrency";

    public const string PortfolioRequestTimeoutPolicyName =
        "portfolio-control-center-timeout";

    protected IActionResult? RequireUser(
        IAnalyticsUserContext userContext,
        out int userId)
    {
        if (userContext.TryGetUserId(out userId))
        {
            return null;
        }

        return AnalyticsProblem(
            StatusCodes.Status401Unauthorized,
            "Identidad no disponible",
            "No se pudo identificar al usuario SISGES.");
    }

    protected async Task<AnalyticsAdministratorAccess> RequireAdministratorAsync(
        IAnalyticsUserContext userContext,
        IAnalyticsAuthorizationService authorizationService,
        SisgesOptionPermission permission,
        CancellationToken cancellationToken)
    {
        var identityError = RequireUser(userContext, out var userId);
        if (identityError is not null)
        {
            return new AnalyticsAdministratorAccess(0, identityError);
        }

        int? groupId = userContext.TryGetGroupId(out var currentGroupId)
            ? currentGroupId
            : null;

        var authorization = await authorizationService.CanAccessAdministrationAsync(
            userId,
            groupId,
            permission,
            cancellationToken);

        if (!authorization.Allowed)
        {
            return new AnalyticsAdministratorAccess(
                userId,
                AnalyticsProblem(
                    StatusCodes.Status403Forbidden,
                    "Acceso administrativo denegado",
                    authorization.Reason ??
                    "El usuario no tiene permisos sobre Mantener módulo."));
        }

        return new AnalyticsAdministratorAccess(userId, null);
    }

    protected IActionResult ToCommandResult(
        AnalyticsAdministrationCommandResult result) =>
        result.Status switch
        {
            AnalyticsAdministrationCommandStatus.Success => NoContent(),
            AnalyticsAdministrationCommandStatus.NotFound => NotFound(),
            AnalyticsAdministrationCommandStatus.InvalidRequest => AnalyticsProblem(
                StatusCodes.Status400BadRequest,
                result.Title ?? "Solicitud inválida",
                result.Detail ?? "La solicitud no pudo ser procesada."),
            _ => throw new ArgumentOutOfRangeException(
                nameof(result),
                result.Status,
                "Estado de administración no soportado.")
        };


    protected IActionResult ToPortfolioResult<T>(
        PortfolioOperationResult<T> result)
    {
        if (result.ValidationErrors is not null)
        {
            return AnalyticsValidationProblem(result.ValidationErrors);
        }

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return AnalyticsProblem(
            result.ErrorStatusCode ?? StatusCodes.Status500InternalServerError,
            result.ErrorTitle ?? "Error de Portfolio Control Center",
            result.ErrorDetail ?? "No se pudo completar la operación solicitada.");
    }

    protected IActionResult AnalyticsValidationProblem(
        IReadOnlyDictionary<string, string[]> errors)
    {
        var problem = new ValidationProblemDetails(
            errors.ToDictionary(
                pair => pair.Key,
                pair => pair.Value,
                StringComparer.Ordinal))
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Uno o más errores de validación ocurrieron.",
            Detail = "Revise los parámetros enviados.",
            Instance = HttpContext.Request.Path.Value
        };
        var result = new BadRequestObjectResult(problem);
        result.ContentTypes.Add("application/problem+json");
        return result;
    }

    protected IActionResult AnalyticsValidationProblem(ModelStateDictionary modelState)
    {
        var errors = modelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .ToDictionary(
                entry => entry.Key,
                entry => entry.Value!.Errors
                    .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                        ? "El valor proporcionado no es válido."
                        : error.ErrorMessage)
                    .ToArray(),
                StringComparer.Ordinal);

        return AnalyticsValidationProblem(errors);
    }

    protected ObjectResult AnalyticsProblem(
        int statusCode,
        string title,
        string detail)
    {
        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = HttpContext.Request.Path.Value
        };

        var result = new ObjectResult(problem)
        {
            StatusCode = statusCode
        };
        result.ContentTypes.Add("application/problem+json");
        return result;
    }

    protected sealed record AnalyticsAdministratorAccess(
        int UserId,
        IActionResult? Error);
}
