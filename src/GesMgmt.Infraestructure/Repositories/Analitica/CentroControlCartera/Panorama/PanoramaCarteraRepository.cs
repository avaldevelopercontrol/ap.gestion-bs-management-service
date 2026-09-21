using GesMgmt.Application.DTOs.Analitica.CentroControlCartera;
using GesMgmt.Domain.Entities.Analitica.CentroControlCartera;
using GesMgmt.Domain.Interfaces.Analitica.CentroControlCartera;
using GesMgmt.Infraestructure.Persistence;

namespace GesMgmt.Infraestructure.Repositories.Analitica.CentroControlCartera;

internal sealed class PanoramaCarteraRepository(
    AnaliticaDbContext context,
    TimeProvider timeProvider)
    : IPanoramaCarteraRepository
{
    public Task<PanoramaCarteraContexto?> ResolverContextoAsync(
        int idClienteCrm,
        string? codigoCampana,
        long? idSubCartera,
        string? unidadNegocio,
        CancellationToken cancellationToken) =>
        PanoramaCarteraContextoEfConsulta.EjecutarAsync(
            context,
            idClienteCrm,
            codigoCampana,
            idSubCartera,
            unidadNegocio,
            cancellationToken);

    public async Task<PanoramaCarteraDbFilas> ObtenerPanoramaAsync(
        int claveCliente,
        int claveCampana,
        long? idSubCartera,
        string? unidadNegocio,
        bool includeClientLevelTarget,
        RangoResumenCartera range,
        CancellationToken cancellationToken)
    {
        var resumen = await ResumenCarteraEfConsulta.EjecutarAsync(
            context,
            claveCliente,
            claveCampana,
            idSubCartera,
            unidadNegocio,
            range,
            cancellationToken);

        AvanceMetaCarteraDbFila? targetProgress = null;
        if (idSubCartera is null && includeClientLevelTarget)
        {
            targetProgress = await AvanceMetaCarteraEfConsulta.ObtenerAsync(
                context,
                claveCliente,
                claveCampana,
                unidadNegocio,
                range.FechaHasta,
                cancellationToken);
        }

        var rangoHoyPeru = PromesasCarteraFechaActual.ObtenerHoyPeru(timeProvider);

        var promises = await PromesasCarteraEfConsulta.ObtenerOperacionalAsync(
            context,
            claveCliente,
            claveCampana,
            idSubCartera,
            unidadNegocio,
            rangoHoyPeru,
            cancellationToken);

        var evolution = await EvolucionCarteraEfConsulta.ObtenerAsync(
            context,
            claveCliente,
            claveCampana,
            idSubCartera,
            unidadNegocio,
            new RangoEvolucionCartera(range.FechaDesde, range.FechaHasta),
            cancellationToken);

        return new PanoramaCarteraDbFilas(
            resumen,
            targetProgress,
            promises,
            evolution);
    }
}
