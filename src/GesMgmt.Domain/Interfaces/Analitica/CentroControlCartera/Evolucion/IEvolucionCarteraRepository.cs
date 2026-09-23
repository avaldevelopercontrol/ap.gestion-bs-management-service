using GesMgmt.Domain.Entities.Analitica.CentroControlCartera;
namespace GesMgmt.Domain.Interfaces.Analitica.CentroControlCartera;

public interface IEvolucionCarteraRepository
{
    Task<EvolucionCarteraContexto?> ResolverContextoAsync(
        int idClienteCrm,
        string? codigoCampana,
        long? idSubCartera,
        string? unidadNegocio,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<EvolucionCarteraDbFila>> ObtenerEvolucionAsync(
        int claveCliente,
        int claveCampana,
        long? idSubCartera,
        string? unidadNegocio,
        RangoEvolucionCartera range,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<EvolucionCarteraSerieHistoricaDb>> ObtenerHistoricoComparableAsync(
        EvolucionCarteraContexto referencia,
        RangoEvolucionCartera rangoReferencia,
        long? idSubCartera,
        string? unidadNegocio,
        int cantidadMeses,
        CancellationToken cancellationToken);
}
