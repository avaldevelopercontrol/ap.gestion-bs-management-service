using GesMgmt.Domain.Entities.Analitica.CentroControlCartera;
using GesMgmt.Domain.Interfaces.Analitica.CentroControlCartera;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories.Analitica.CentroControlCartera;

internal sealed class PromesasVencidasCarteraRepository(
    AnaliticaDbContext context,
    AvalDbContext avalContext,
    TimeProvider timeProvider)
    : IPromesasVencidasCarteraRepository
{
    public Task<PromesasVencidasCarteraConsultaResult> ObtenerAsync(
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
            "diasVencimiento",
            "desc",
            cancellationToken);

    public async Task<PromesasVencidasCarteraConsultaResult> ObtenerAsync(
        int claveCliente,
        int claveCampana,
        long? idSubCartera,
        string? unidadNegocio,
        int pagina,
        int tamanoPagina,
        string? antiguedad,
        string ordenarPor,
        string direccionOrden,
        CancellationToken cancellationToken)
    {
        var rangoHoyPeru = PromesasCarteraFechaActual.ObtenerHoyPeru(timeProvider);
        var fechaCorte = rangoHoyPeru.FechaDesde;

        var baseQuery = PromesaCarteraDetallesEfConsulta.AplicarAlcance(
            context,
            PromesasCarteraEstadoOperativoEfConsulta.AplicarVencidasConSaldo(
                context.PromesaOperativaSupervisorAnalitica
                    .AsNoTracking()
                    .Where(row =>
                        row.ClaveCliente == claveCliente
                        && row.ClaveCampana == claveCampana
                        && row.EsPromesaValida),
                fechaCorte),
            idSubCartera,
            unidadNegocio);

        var totalCount = await baseQuery.LongCountAsync(cancellationToken);
        if (totalCount == 0)
        {
            return new PromesasVencidasCarteraConsultaResult(
                new PromesasVencidasCarteraResumenDbFila
                {
                    FechaCorte = fechaCorte
                },
                [],
                [],
                [],
                [],
                PromesasCarteraPaginacion.Crear(pagina, tamanoPagina, 0));
        }

        var itemQuery = baseQuery.Select(row => new OverdueItemProjection
        {
            IdPromesa = row.ClaveHechoPromesa,
            IdDeudor = row.IdDeudorOrigen,
            FechaVencimiento = row.FechaVencimientoPromesa,
            DiasVencimiento = row.FechaVencimientoPromesa == null
                ? null
                : EF.Functions.DateDiffDay(row.FechaVencimientoPromesa.Value, fechaCorte) < 1
                    ? 1
                    : EF.Functions.DateDiffDay(row.FechaVencimientoPromesa.Value, fechaCorte),
            MontoPromesa = row.MontoPromesa ?? 0m,
            MontoPagado = row.MontoPagado ?? 0m,
            MontoPendiente = (row.MontoPromesa ?? 0m) > (row.MontoPagado ?? 0m)
                ? (row.MontoPromesa ?? 0m) - (row.MontoPagado ?? 0m)
                : 0m,
            ClaveSituacion = (row.MontoPagado ?? 0m) > 0m
                ? PromesasCarteraEstadoOperativo.PagoParcial
                : PromesasCarteraEstadoOperativo.SinPagoRegistrado,
            IdAsesor = row.ClaveAsesor,
            NombreAsesor = row.NombreAsesor == null || row.NombreAsesor.Trim() == string.Empty
                ? null
                : row.NombreAsesor.Trim(),
            IdSupervisor = row.ClaveSupervisor,
            NombreSupervisor = row.NombreSupervisor == null || row.NombreSupervisor.Trim() == string.Empty
                ? null
                : row.NombreSupervisor.Trim(),
            ClaveAntiguedad = row.FechaVencimientoPromesa == null
                ? "sin-clasificar"
                : EF.Functions.DateDiffDay(row.FechaVencimientoPromesa.Value, fechaCorte) >= 1
                    && EF.Functions.DateDiffDay(row.FechaVencimientoPromesa.Value, fechaCorte) <= 3
                        ? "1-3"
                        : EF.Functions.DateDiffDay(row.FechaVencimientoPromesa.Value, fechaCorte) >= 4
                            && EF.Functions.DateDiffDay(row.FechaVencimientoPromesa.Value, fechaCorte) <= 7
                                ? "4-7"
                                : "8-mas",
            FechaActualizacionUtc = row.FechaCarga
        });

        var summaryData = await itemQuery
            .GroupBy(_ => 1)
            .Select(group => new
            {
                CantidadPromesas = group.LongCount(),
                MontoPromesa = group.Sum(row => row.MontoPromesa),
                MontoPendiente = group.Sum(row => row.MontoPendiente),
                FechaActualizacionUtc = group.Max(row => row.FechaActualizacionUtc)
            })
            .SingleAsync(cancellationToken);

        var resumen = new PromesasVencidasCarteraResumenDbFila
        {
            CantidadVencidas = summaryData.CantidadPromesas,
            MontoVencido = PromesaCarteraDetallesEfConsulta.RedondearMonto(summaryData.MontoPromesa),
            MontoPendiente = PromesaCarteraDetallesEfConsulta.RedondearMonto(summaryData.MontoPendiente),
            FechaCorte = fechaCorte,
            FechaActualizacionUtc = summaryData.FechaActualizacionUtc
        };

        var filasAntiguedad = await itemQuery
            .GroupBy(row => row.ClaveAntiguedad)
            .Select(group => new
            {
                ClaveAntiguedad = group.Key,
                CantidadPromesas = group.LongCount(),
                MontoPromesa = group.Sum(row => row.MontoPromesa),
                MontoPendiente = group.Sum(row => row.MontoPendiente)
            })
            .ToListAsync(cancellationToken);

        var rangosAntiguedad = filasAntiguedad
            .Select(row => new PromesasVencidasCarteraAntiguedadDbFila
            {
                ClaveAntiguedad = row.ClaveAntiguedad,
                CantidadPromesas = row.CantidadPromesas,
                MontoPromesa = PromesaCarteraDetallesEfConsulta.RedondearMonto(row.MontoPromesa),
                MontoPendiente = PromesaCarteraDetallesEfConsulta.RedondearMonto(row.MontoPendiente)
            })
            .ToArray();

        var advisors = await itemQuery
            .Where(row => row.IdAsesor.HasValue && row.NombreAsesor != null)
            .GroupBy(row => row.IdAsesor!.Value)
            .Select(group => new CarteraVencidoAsesorFiltroDbFila
            {
                IdAsesor = group.Key,
                NombreAsesor = group.Max(row => row.NombreAsesor)!
            })
            .OrderBy(row => row.NombreAsesor)
            .ToArrayAsync(cancellationToken);

        var supervisores = await itemQuery
            .Where(row => row.IdSupervisor.HasValue && row.NombreSupervisor != null)
            .GroupBy(row => row.IdSupervisor!.Value)
            .Select(group => new CarteraVencidoSupervisorFiltroDbFila
            {
                IdSupervisor = group.Key,
                NombreSupervisor = group.Max(row => row.NombreSupervisor)!
            })
            .OrderBy(row => row.NombreSupervisor)
            .ToArrayAsync(cancellationToken);

        if (antiguedad is not null)
        {
            itemQuery = itemQuery.Where(row => row.ClaveAntiguedad == antiguedad);
        }

        var filteredCount = antiguedad is null
            ? totalCount
            : await itemQuery.LongCountAsync(cancellationToken);

        var orderedItems = AplicarOrden(itemQuery, ordenarPor, direccionOrden)
            .ThenBy(row => row.IdPromesa);

        var itemRows = await orderedItems
            .Skip(PromesaCarteraDetallesEfConsulta.ObtenerOffset(pagina, tamanoPagina))
            .Take(tamanoPagina)
            .ToListAsync(cancellationToken);

        var nombresPorDeudor = await PromesasCarteraDeudorEfConsulta.ObtenerNombresAsync(
            avalContext,
            itemRows.Select(row => row.IdDeudor),
            cancellationToken);

        var elementos = itemRows
            .Select(row => new PromesaVencidaCarteraDbFila
            {
                IdPromesa = row.IdPromesa,
                IdDeudor = row.IdDeudor,
                NombreDeudor = nombresPorDeudor.GetValueOrDefault(row.IdDeudor),
                FechaVencimiento = row.FechaVencimiento,
                DiasVencimiento = row.DiasVencimiento,
                MontoPromesa = PromesaCarteraDetallesEfConsulta.RedondearMonto(row.MontoPromesa),
                MontoPagado = PromesaCarteraDetallesEfConsulta.RedondearMonto(row.MontoPagado),
                MontoPendiente = PromesaCarteraDetallesEfConsulta.RedondearMonto(row.MontoPendiente),
                ClaveSituacion = row.ClaveSituacion,
                IdAsesor = row.IdAsesor,
                NombreAsesor = PromesaCarteraDetallesEfConsulta.NormalizarNombre(row.NombreAsesor),
                IdSupervisor = row.IdSupervisor,
                NombreSupervisor = PromesaCarteraDetallesEfConsulta.NormalizarNombre(row.NombreSupervisor),
                ClaveAntiguedad = row.ClaveAntiguedad,
                FechaCorte = fechaCorte,
                FechaActualizacionUtc = row.FechaActualizacionUtc
            })
            .ToArray();

        return new PromesasVencidasCarteraConsultaResult(
            resumen,
            rangosAntiguedad,
            elementos,
            advisors,
            supervisores,
            PromesasCarteraPaginacion.Crear(pagina, tamanoPagina, filteredCount));
    }

    private static IOrderedQueryable<OverdueItemProjection> AplicarOrden(
        IQueryable<OverdueItemProjection> query,
        string ordenarPor,
        string direccionOrden)
    {
        var ascending = string.Equals(direccionOrden, "asc", StringComparison.OrdinalIgnoreCase);

        return ordenarPor.ToLowerInvariant() switch
        {
            "debtorid" => ascending
                ? query.OrderBy(row => row.IdDeudor)
                : query.OrderByDescending(row => row.IdDeudor),
            "duedate" => ascending
                ? query.OrderBy(row => row.FechaVencimiento)
                : query.OrderByDescending(row => row.FechaVencimiento),
            "overduedays" => ascending
                ? query.OrderBy(row => row.DiasVencimiento)
                : query.OrderByDescending(row => row.DiasVencimiento),
            "promiseamount" => ascending
                ? query.OrderBy(row => row.MontoPromesa)
                : query.OrderByDescending(row => row.MontoPromesa),
            "paidamount" => ascending
                ? query.OrderBy(row => row.MontoPagado)
                : query.OrderByDescending(row => row.MontoPagado),
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

    private sealed class OverdueItemProjection
    {
        public long IdPromesa { get; init; }
        public long IdDeudor { get; init; }
        public DateTime? FechaVencimiento { get; init; }
        public int? DiasVencimiento { get; init; }
        public decimal MontoPromesa { get; init; }
        public decimal MontoPagado { get; init; }
        public decimal MontoPendiente { get; init; }
        public string ClaveSituacion { get; init; } = string.Empty;
        public int? IdAsesor { get; init; }
        public string? NombreAsesor { get; init; }
        public int? IdSupervisor { get; init; }
        public string? NombreSupervisor { get; init; }
        public string ClaveAntiguedad { get; init; } = string.Empty;
        public DateTime? FechaActualizacionUtc { get; init; }
    }
}
