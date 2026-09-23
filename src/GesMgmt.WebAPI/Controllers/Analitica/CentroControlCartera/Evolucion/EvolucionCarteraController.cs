using GesMgmt.Application.DTOs.Analitica.CentroControlCartera;
using GesMgmt.Application.Interfaces.Analitica.CentroControlCartera;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Mvc;

namespace GesMgmt.WebAPI.Controllers.Analitica;

[Route("v1/Analitica/CentroControlCartera/Evolucion")]
[EnableRateLimiting(AnaliticaControllerBase.NombrePoliticaConcurrenciaCartera)]
[RequestTimeout(AnaliticaControllerBase.NombrePoliticaTimeoutCartera)]
public sealed class EvolucionCarteraController(IEvolucionCarteraService service) : AnaliticaControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(EvolucionCarteraResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> ObtenerAsync(
        [FromQuery] string? campana,
        [FromQuery] string? idSubCartera,
        [FromQuery] string? fechaDesde,
        [FromQuery] string? fechaHasta,
        [FromQuery] string? unidadNegocio,
        [FromQuery] int? idClienteCrm,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ProblemaValidacionAnalitica(ModelState);
        }

        var result = await service.ObtenerAsync(
            campana,
            idSubCartera,
            fechaDesde,
            fechaHasta,
            unidadNegocio,
            idClienteCrm,
            cancellationToken);

        return ToPortfolioResult(result);
    }

    [HttpGet("Comparativa")]
    [ProducesResponseType(typeof(EvolucionCarteraComparativaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> ObtenerComparativaAsync(
        [FromQuery] string? campana,
        [FromQuery] string? idSubCartera,
        [FromQuery] string? fechaDesde,
        [FromQuery] string? fechaHasta,
        [FromQuery] string? unidadNegocio,
        [FromQuery] int? idClienteCrm,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ProblemaValidacionAnalitica(ModelState);
        }

        var result = await service.ObtenerComparativaAsync(
            campana,
            idSubCartera,
            fechaDesde,
            fechaHasta,
            unidadNegocio,
            idClienteCrm,
            cancellationToken);

        return ToPortfolioResult(result);
    }
}
