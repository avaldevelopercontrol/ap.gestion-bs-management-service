using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.WebAPI.Services.Analytics;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace GesMgmt.WebAPI.Controllers.Analytics;

[Route("api/v1/portfolio-control-center/bootstrap")]
[EnableRateLimiting(PortfolioControlCenterResourceProtection.ConcurrencyPolicyName)]
[RequestTimeout(PortfolioControlCenterResourceProtection.RequestTimeoutPolicyName)]
public sealed class PortfolioBootstrapController(IPortfolioBootstrapService service) : AnalyticsControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PortfolioBootstrapResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetAsync(
        [FromQuery] string? campaign,
        [FromQuery] string? subPortfolioId,
        [FromQuery] string? dateFrom,
        [FromQuery] string? dateTo,
        [FromQuery] string? businessUnit,
        [FromQuery] int? crmClientId,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return AnalyticsValidationProblem(ModelState);
        }

        var result = await service.GetAsync(
            campaign,
            subPortfolioId,
            dateFrom,
            dateTo,
            businessUnit,
            crmClientId,
            cancellationToken);

        return ToPortfolioResult(result);
    }
}
