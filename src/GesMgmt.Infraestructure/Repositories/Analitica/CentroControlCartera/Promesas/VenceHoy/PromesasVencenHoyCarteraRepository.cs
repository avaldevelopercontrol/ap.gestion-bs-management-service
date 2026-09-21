using GesMgmt.Domain.Entities.Analitica.CentroControlCartera;
using GesMgmt.Domain.Interfaces.Analitica.CentroControlCartera;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories.Analitica.CentroControlCartera;

internal sealed class PromesasVencenHoyCarteraRepository(
    AnaliticaDbContext context,
    TimeProvider timeProvider)
    : IPromesasVencenHoyCarteraRepository
{
    public Task<PromesasVencenHoyCarteraConsultaResult> ObtenerAsync(
        int claveCliente,
        int claveCampana,
        long? idSubCartera,
        string? unidadNegocio,
        int pagina,
        int tamanoPagina,
        CancellationToken cancellationToken) =>
        ObtenerAsync(
            claveCliente,
            claveCampana,
            idSubCartera,
            unidadNegocio,
            pagina,
            tamanoPagina,
            null,
            "montoPendiente",
            "desc",
            cancellationToken);

    public async Task<PromesasVencenHoyCarteraConsultaResult> ObtenerAsync(
        int claveCliente,
        int claveCampana,
        long? idSubCartera,
        string? unidadNegocio,
        int pagina,
        int tamanoPagina,
        string? estado,
        string ordenarPor,
        string direccionOrden,
        CancellationToken cancellationToken)
    {
        var rangoHoyPeru = PromesasCarteraFechaActual.ObtenerHoyPeru(timeProvider);

        var baseQuery = PromesaCarteraDetallesEfConsulta.AplicarAlcance(
            context,
            context.PromesaOperativaAnalitica
                .AsNoTracking()
                .Where(row =>
                    row.ClaveCliente == claveCliente
                    && row.ClaveCampana == claveCampana
                    && row.EsPromesaValida
                    && row.FechaVencimientoPromesa >= rangoHoyPeru.FechaDesde
                    && row.FechaVencimientoPromesa < rangoHoyPeru.FechaHastaExclusiva),
            idSubCartera,
            unidadNegocio);

        var metricQuery = baseQuery.Select(row => new DueTodayMetricProjection
        {
            FechaVencimiento = row.FechaVencimientoPromesa,
            MontoPromesa = row.MontoPromesa ?? 0m,
            MontoPagado = row.MontoPagado ?? 0m,
            MontoPendiente = (row.MontoPromesa ?? 0m) > (row.MontoPagado ?? 0m)
                ? (row.MontoPromesa ?? 0m) - (row.MontoPagado ?? 0m)
                : 0m,
            ClaveEstado = (row.MontoPagado ?? 0m) <= 0m
                ? "pendiente"
                : (row.MontoPagado ?? 0m) < (row.MontoPromesa ?? 0m)
                    ? "parcial"
                    : "cubierta",
            FechaActualizacionUtc = row.FechaCarga
        });

        var summaryData = await metricQuery
            .GroupBy(_ => 1)
            .Select(group => new
            {
                CantidadPromesas = group.LongCount(),
                MontoPromesa = group.Sum(row => row.MontoPromesa),
                MontoPagado = group.Sum(row => row.MontoPagado),
                MontoPendiente = group.Sum(row => row.MontoPendiente),
                FechaCorte = group.Max(row => row.FechaVencimiento),
                FechaActualizacionUtc = group.Max(row => row.FechaActualizacionUtc)
            })
            .SingleOrDefaultAsync(cancellationToken);

        var resumen = summaryData is null
            ? new PromesasVencenHoyCarteraResumenDbFila()
            : new PromesasVencenHoyCarteraResumenDbFila
            {
                CantidadVenceHoy = summaryData.CantidadPromesas,
                MontoVenceHoy = PromesaCarteraDetallesEfConsulta.RedondearMonto(summaryData.MontoPromesa),
                MontoPagado = PromesaCarteraDetallesEfConsulta.RedondearMonto(summaryData.MontoPagado),
                MontoPendiente = PromesaCarteraDetallesEfConsulta.RedondearMonto(summaryData.MontoPendiente),
                FechaCorte = summaryData.FechaCorte,
                FechaActualizacionUtc = summaryData.FechaActualizacionUtc
            };

        var statusRows = await metricQuery
            .GroupBy(row => row.ClaveEstado)
            .Select(group => new
            {
                ClaveEstado = group.Key,
                CantidadPromesas = group.LongCount(),
                MontoPromesa = group.Sum(row => row.MontoPromesa),
                MontoPagado = group.Sum(row => row.MontoPagado),
                MontoPendiente = group.Sum(row => row.MontoPendiente)
            })
            .ToListAsync(cancellationToken);

        var statusBuckets = statusRows
            .Select(row => new PromesasVencenHoyCarteraEstadoDbFila
            {
                ClaveEstado = row.ClaveEstado,
                CantidadPromesas = row.CantidadPromesas,
                MontoPromesa = PromesaCarteraDetallesEfConsulta.RedondearMonto(row.MontoPromesa),
                MontoPagado = PromesaCarteraDetallesEfConsulta.RedondearMonto(row.MontoPagado),
                MontoPendiente = PromesaCarteraDetallesEfConsulta.RedondearMonto(row.MontoPendiente)
            })
            .ToArray();

        var filteredCount = estado is null
            ? resumen.CantidadVenceHoy
            : await metricQuery.LongCountAsync(
                row => row.ClaveEstado == estado,
                cancellationToken);

        var itemQuery = baseQuery.Select(row => new DueTodayItemProjection
        {
            IdPromesa = row.ClaveHechoPromesa,
            IdDeudor = row.IdDeudorOrigen,
            FechaVencimiento = row.FechaVencimientoPromesa,
            MontoPromesa = row.MontoPromesa ?? 0m,
            MontoPagado = row.MontoPagado ?? 0m,
            MontoPendiente = (row.MontoPromesa ?? 0m) > (row.MontoPagado ?? 0m)
                ? (row.MontoPromesa ?? 0m) - (row.MontoPagado ?? 0m)
                : 0m,
            FechaUltimoPago = row.FechaUltimoPago,
            ClaveEstado = (row.MontoPagado ?? 0m) <= 0m
                ? "pendiente"
                : (row.MontoPagado ?? 0m) < (row.MontoPromesa ?? 0m)
                    ? "parcial"
                    : "cubierta",
            EtiquetaEstado = (row.MontoPagado ?? 0m) <= 0m
                ? "Pendiente"
                : (row.MontoPagado ?? 0m) < (row.MontoPromesa ?? 0m)
                    ? "Pago parcial"
                    : "Cubierta",
            IdAsesor = row.ClaveAsesor,
            NombreAsesor = context.PromesaOperativaSupervisorAnalitica
                .AsNoTracking()
                .Where(attribution =>
                    attribution.ClaveCliente == row.ClaveCliente
                    && attribution.ClaveCampana == row.ClaveCampana
                    && attribution.ClaveHechoPromesa == row.ClaveHechoPromesa)
                .OrderBy(attribution => attribution.ClaveSupervisor == null ? 1 : 0)
                .ThenByDescending(attribution => attribution.ClaveSupervisorAsesor)
                .ThenByDescending(attribution => attribution.FechaCarga)
                .Select(attribution => attribution.NombreAsesor == null
                    || attribution.NombreAsesor.Trim() == string.Empty
                        ? null
                        : attribution.NombreAsesor.Trim())
                .FirstOrDefault(),
            IdSupervisor = context.PromesaOperativaSupervisorAnalitica
                .AsNoTracking()
                .Where(attribution =>
                    attribution.ClaveCliente == row.ClaveCliente
                    && attribution.ClaveCampana == row.ClaveCampana
                    && attribution.ClaveHechoPromesa == row.ClaveHechoPromesa)
                .OrderBy(attribution => attribution.ClaveSupervisor == null ? 1 : 0)
                .ThenByDescending(attribution => attribution.ClaveSupervisorAsesor)
                .ThenByDescending(attribution => attribution.FechaCarga)
                .Select(attribution => attribution.ClaveSupervisor)
                .FirstOrDefault(),
            NombreSupervisor = context.PromesaOperativaSupervisorAnalitica
                .AsNoTracking()
                .Where(attribution =>
                    attribution.ClaveCliente == row.ClaveCliente
                    && attribution.ClaveCampana == row.ClaveCampana
                    && attribution.ClaveHechoPromesa == row.ClaveHechoPromesa)
                .OrderBy(attribution => attribution.ClaveSupervisor == null ? 1 : 0)
                .ThenByDescending(attribution => attribution.ClaveSupervisorAsesor)
                .ThenByDescending(attribution => attribution.FechaCarga)
                .Select(attribution => attribution.NombreSupervisor == null
                    || attribution.NombreSupervisor.Trim() == string.Empty
                        ? null
                        : attribution.NombreSupervisor.Trim())
                .FirstOrDefault(),
            FechaActualizacionUtc = row.FechaCarga
        });

        if (estado is not null)
        {
            itemQuery = itemQuery.Where(row => row.ClaveEstado == estado);
        }

        var orderedItems = AplicarOrden(itemQuery, ordenarPor, direccionOrden)
            .ThenBy(row => row.IdPromesa);

        var itemRows = await orderedItems
            .Skip(PromesaCarteraDetallesEfConsulta.ObtenerOffset(pagina, tamanoPagina))
            .Take(tamanoPagina)
            .ToListAsync(cancellationToken);

        var elementos = itemRows
            .Select(row => new PromesaVenceHoyCarteraDbFila
            {
                IdPromesa = row.IdPromesa,
                IdDeudor = row.IdDeudor,
                FechaVencimiento = row.FechaVencimiento,
                MontoPromesa = PromesaCarteraDetallesEfConsulta.RedondearMonto(row.MontoPromesa),
                MontoPagado = PromesaCarteraDetallesEfConsulta.RedondearMonto(row.MontoPagado),
                MontoPendiente = PromesaCarteraDetallesEfConsulta.RedondearMonto(row.MontoPendiente),
                FechaUltimoPago = row.FechaUltimoPago,
                ClaveEstado = row.ClaveEstado,
                IdAsesor = row.IdAsesor,
                NombreAsesor = PromesaCarteraDetallesEfConsulta.NormalizarNombre(row.NombreAsesor),
                IdSupervisor = row.IdSupervisor,
                NombreSupervisor = PromesaCarteraDetallesEfConsulta.NormalizarNombre(row.NombreSupervisor),
                FechaActualizacionUtc = row.FechaActualizacionUtc
            })
            .ToArray();

        return new PromesasVencenHoyCarteraConsultaResult(
            resumen,
            statusBuckets,
            elementos,
            PromesasCarteraPaginacion.Crear(pagina, tamanoPagina, filteredCount));
    }

    private static IOrderedQueryable<DueTodayItemProjection> AplicarOrden(
        IQueryable<DueTodayItemProjection> query,
        string ordenarPor,
        string direccionOrden)
    {
        var ascending = string.Equals(direccionOrden, "asc", StringComparison.OrdinalIgnoreCase);

        return ordenarPor.ToLowerInvariant() switch
        {
            "debtorid" => ascending
                ? query.OrderBy(row => row.IdDeudor)
                : query.OrderByDescending(row => row.IdDeudor),
            "promiseamount" => ascending
                ? query.OrderBy(row => row.MontoPromesa)
                : query.OrderByDescending(row => row.MontoPromesa),
            "paidamount" => ascending
                ? query.OrderBy(row => row.MontoPagado)
                : query.OrderByDescending(row => row.MontoPagado),
            "statuslabel" => ascending
                ? query.OrderBy(row => row.EtiquetaEstado)
                : query.OrderByDescending(row => row.EtiquetaEstado),
            "lastpaymentdate" => ascending
                ? query.OrderBy(row => row.FechaUltimoPago)
                : query.OrderByDescending(row => row.FechaUltimoPago),
            "advisorname" => ascending
                ? query.OrderBy(row => row.NombreAsesor)
                : query.OrderByDescending(row => row.NombreAsesor),
            "supervisorname" => ascending
                ? query.OrderBy(row => row.NombreSupervisor)
                : query.OrderByDescending(row => row.NombreSupervisor),
            _ => ascending
                ? query.OrderBy(row => row.MontoPendiente)
                : query.OrderByDescending(row => row.MontoPendiente)
        };
    }

    private sealed class DueTodayMetricProjection
    {
        public DateTime? FechaVencimiento { get; init; }
        public decimal MontoPromesa { get; init; }
        public decimal MontoPagado { get; init; }
        public decimal MontoPendiente { get; init; }
        public string ClaveEstado { get; init; } = string.Empty;
        public DateTime? FechaActualizacionUtc { get; init; }
    }

    private sealed class DueTodayItemProjection
    {
        public long IdPromesa { get; init; }
        public long IdDeudor { get; init; }
        public DateTime? FechaVencimiento { get; init; }
        public decimal MontoPromesa { get; init; }
        public decimal MontoPagado { get; init; }
        public decimal MontoPendiente { get; init; }
        public DateTime? FechaUltimoPago { get; init; }
        public string ClaveEstado { get; init; } = string.Empty;
        public string EtiquetaEstado { get; init; } = string.Empty;
        public int? IdAsesor { get; init; }
        public string? NombreAsesor { get; init; }
        public int? IdSupervisor { get; init; }
        public string? NombreSupervisor { get; init; }
        public DateTime? FechaActualizacionUtc { get; init; }
    }
}
