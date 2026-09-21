using GesMgmt.Domain.Entities.Analitica.CentroControlCartera;
using GesMgmt.Domain.Interfaces.Analitica.CentroControlCartera;
using GesMgmt.Infraestructure.Persistence;

namespace GesMgmt.Infraestructure.Repositories.Analitica.CentroControlCartera;

internal sealed class PromesasCarteraRepository(
    AnaliticaDbContext context,
    TimeProvider timeProvider)
    : IPromesasCarteraRepository
{
    public Task<PromesasCarteraContexto?> ResolverContextoAsync(
        int idClienteCrm,
        string? codigoCampana,
        long? idSubCartera,
        string? unidadNegocio,
        CancellationToken cancellationToken) =>
        PromesasCarteraEfConsulta.ResolverContextoAsync(
            context,
            idClienteCrm,
            codigoCampana,
            idSubCartera,
            unidadNegocio,
            cancellationToken);

    public Task<PromesasCarteraDbFila> ObtenerPromesasOperacionalesAsync(
        int claveCliente,
        int claveCampana,
        long? idSubCartera,
        string? unidadNegocio,
        CancellationToken cancellationToken)
    {
        var rangoHoyPeru = PromesasCarteraFechaActual.ObtenerHoyPeru(timeProvider);

        return PromesasCarteraEfConsulta.ObtenerOperacionalAsync(
            context,
            claveCliente,
            claveCampana,
            idSubCartera,
            unidadNegocio,
            rangoHoyPeru,
            cancellationToken);
    }
}
