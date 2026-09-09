using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Mvc;

namespace GesMgmt.WebAPI.Controllers.Analytics;

[Route("api/v1/portfolio-control-center/promises")]
[EnableRateLimiting(AnalyticsControllerBase.PortfolioConcurrencyPolicyName)]
[RequestTimeout(AnalyticsControllerBase.PortfolioRequestTimeoutPolicyName)]
public sealed class PortfolioPromisesController(IPortfolioPromisesService service) : AnalyticsControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PortfolioPromisesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
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
