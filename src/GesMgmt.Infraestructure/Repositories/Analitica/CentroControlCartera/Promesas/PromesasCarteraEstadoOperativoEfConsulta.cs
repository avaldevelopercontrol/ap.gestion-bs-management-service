using GesMgmt.Domain.Entities.Analitica.CentroControlCartera;

namespace GesMgmt.Infraestructure.Repositories.Analitica.CentroControlCartera;

/// <summary>
/// Traducción EF de la regla de promesa vencida con saldo para mantener
/// el filtrado en SQL y evitar materializar todo el universo de promesas.
/// </summary>
internal static class PromesasCarteraEstadoOperativoEfConsulta
{
    public static IQueryable<PromesaOperativaSupervisorAnalitica> AplicarVencidasConSaldo(
        IQueryable<PromesaOperativaSupervisorAnalitica> query,
        DateTime fechaHoy) =>
        query.Where(row =>
            row.FechaVencimientoPromesa.HasValue
            && row.FechaVencimientoPromesa.Value < fechaHoy
            && (row.MontoPromesa ?? 0m) > (row.MontoPagado ?? 0m));
}
