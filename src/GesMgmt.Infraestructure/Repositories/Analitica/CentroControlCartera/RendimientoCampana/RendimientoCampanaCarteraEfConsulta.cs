using GesMgmt.Domain.Entities.Analitica.CentroControlCartera;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories.Analitica.CentroControlCartera;

internal static class RendimientoCampanaCarteraEfConsulta
{
    private const int IdClienteCrmMaf = 59;

    private static readonly string[] EstadosCumplidos =
    [
        "FULFILLED",
        "PARTIAL",
        "FULFILLED_OUT_OF_RANGE"
    ];

    private static readonly string[] EstadosCumplimiento =
    [
        "FULFILLED",
        "PARTIAL",
        "FULFILLED_OUT_OF_RANGE",
        "BROKEN"
    ];

    public static async Task<IReadOnlyList<RendimientoCampanaCarteraDbFila>?> ObtenerAsync(
        AnaliticaDbContext context,
        int idClienteCrm,
        RendimientoCampanaCarteraRequest request,
        CancellationToken cancellationToken)
    {
        var claveCliente = await ResolverClaveClienteAsync(
            context,
            idClienteCrm,
            request.UnidadNegocio,
            cancellationToken);

        if (!claveCliente.HasValue)
        {
            return null;
        }

        var usarDeduplicacionCampana =
            idClienteCrm == IdClienteCrmMaf
            && !request.IdSubCartera.HasValue;

        var eligibleCampaigns = ConstruirCampanasElegibles(
            context,
            claveCliente.Value,
            request);

        var ranges = await eligibleCampaigns
            .OrderByDescending(row => row.FechaHasta)
            .ThenByDescending(row => row.CodigoCampana)
            .ToListAsync(cancellationToken);

        if (ranges.Count == 0)
        {
            return [];
        }

        var snapshotDates = await ConstruirFilasCorte(
                context,
                claveCliente.Value,
                request,
                eligibleCampaigns)
            .GroupBy(row => row.ClaveCampana)
            .Select(group => new FechaCorteCampana(
                group.Key,
                group.Max(row => row.FechaCalendario)))
            .ToDictionaryAsync(
                row => row.ClaveCampana,
                row => row.FechaCorte,
                cancellationToken);

        if (snapshotDates.Count == 0)
        {
            return [];
        }

        var snapshotMetrics = await CargarMetricasCorteAsync(
            context,
            claveCliente.Value,
            request,
            usarDeduplicacionCampana,
            snapshotDates,
            cancellationToken);

        var flowMetrics = await CargarMetricasFlujoAsync(
            context,
            claveCliente.Value,
            request,
            eligibleCampaigns,
            cancellationToken);

        var contactMetrics = await CargarMetricasContactoAsync(
            context,
            claveCliente.Value,
            request,
            usarDeduplicacionCampana,
            eligibleCampaigns,
            cancellationToken);

        var promiseMetrics = await CargarMetricasPromesaAsync(
            context,
            claveCliente.Value,
            request,
            usarDeduplicacionCampana,
            eligibleCampaigns,
            cancellationToken);

        var paymentMetrics = await CargarMetricasPagoAsync(
            context,
            claveCliente.Value,
            request,
            usarDeduplicacionCampana,
            eligibleCampaigns,
            cancellationToken);

        var includeClientLevelTarget =
            UnidadNegocioCarteraPolicy.PuedeUsarMetaNivelCliente(
                idClienteCrm,
                request.UnidadNegocio);

        var targetMetrics = includeClientLevelTarget
            ? await CargarMetricasMetaAsync(
                context,
                claveCliente.Value,
                eligibleCampaigns,
                cancellationToken)
            : new Dictionary<int, MetricasMetaCampana>();

        var result = new List<RendimientoCampanaCarteraDbFila>(ranges.Count);

        foreach (var range in ranges)
        {
            if (!snapshotMetrics.TryGetValue(range.ClaveCampana, out var snapshot))
            {
                continue;
            }

            flowMetrics.TryGetValue(range.ClaveCampana, out var flow);
            contactMetrics.TryGetValue(range.ClaveCampana, out var contact);
            promiseMetrics.TryGetValue(range.ClaveCampana, out var promise);
            paymentMetrics.TryGetValue(range.ClaveCampana, out var payment);
            targetMetrics.TryGetValue(range.ClaveCampana, out var meta);

            result.Add(new RendimientoCampanaCarteraDbFila
            {
                CodigoCampana = range.CodigoCampana,
                NombreCampana = range.NombreCampana,
                FechaDesde = range.FechaDesde,
                FechaHasta = range.FechaHasta,
                FechaCorte = snapshot.FechaCorte,
                CarteraAsignada = snapshot.CarteraAsignada,
                CarteraGestionada = snapshot.CarteraGestionada,
                CarteraPendiente = snapshot.CarteraPendiente,
                TasaAvance = Dividir(
                    snapshot.CarteraGestionada,
                    snapshot.CarteraAsignada),
                CantidadGestiones = flow?.CantidadGestiones ?? 0,
                TasaContactabilidad = Dividir(
                    snapshot.ContactedPortfolio,
                    snapshot.CarteraAsignada),
                TasaContactoDirecto = Dividir(
                    contact?.DirectContactClients ?? 0,
                    contact?.ClassifiableClients ?? 0),
                TasaCierre = Dividir(
                    promise?.ClientesPromesaValida ?? 0,
                    contact?.DirectContactClients ?? 0),
                CantidadPromesas = promise?.CantidadPromesas ?? 0,
                TasaCumplimientoPromesa = Dividir(
                    promise?.FulfillmentPaidAmount ?? 0m,
                    promise?.FulfillmentPromiseAmount ?? 0m),
                CantidadPagos = payment?.CantidadPagos ?? 0,
                MontoRecuperado = Redondear(flow?.MontoRecuperado ?? 0m, 4),
                MontoMeta = request.IdSubCartera is null && includeClientLevelTarget
                    ? meta?.MontoMeta
                    : null,
                FechaActualizacionUtc = MaximoNullable(
                [
                    snapshot.FechaCarga,
                    flow?.FechaCarga,
                    contact?.FechaCarga,
                    promise?.FechaCarga,
                    payment?.FechaCarga,
                    meta?.FechaCarga
                ])
            });
        }

        return result;
    }

    private static IQueryable<RangoCampana> ConstruirCampanasElegibles(
        AnaliticaDbContext context,
        int claveCliente,
        RendimientoCampanaCarteraRequest request)
    {
        var requestedDateFrom = request.FechaDesde?.ToDateTime(TimeOnly.MinValue);
        var requestedDateTo = request.FechaHasta?.ToDateTime(TimeOnly.MinValue);

        var selectedCampaigns =
            from campana in context.CampanasAnalitica.AsNoTracking()
            join fact in context.HechosDiariosCarteraAnalitica.AsNoTracking()
                on new { campana.ClaveCliente, campana.ClaveCampana }
                equals new { fact.ClaveCliente, fact.ClaveCampana }
            join date in context.FechasAnalitica.AsNoTracking()
                on fact.ClaveFecha equals date.ClaveFecha
            where campana.ClaveCliente == claveCliente
                  && (request.Campana == null
                      || campana.CodigoCampana == request.Campana)
                  && (!request.IdSubCartera.HasValue
                      || fact.ClaveCartera == request.IdSubCartera.Value)
                  && (request.UnidadNegocio == null
                      || context.CarterasAnalitica.Any(portfolio =>
                          portfolio.ClaveCartera == fact.ClaveCartera
                          && portfolio.UnidadNegocioOrigen == request.UnidadNegocio))
            group date by new
            {
                campana.ClaveCampana,
                campana.CodigoCampana,
                campana.NombreCampana
            }
            into grouped
            select new
            {
                grouped.Key.ClaveCampana,
                grouped.Key.CodigoCampana,
                grouped.Key.NombreCampana,
                FechaDisponibleDesde = grouped.Min(row => row.FechaCalendario),
                FechaDisponibleHasta = grouped.Max(row => row.FechaCalendario)
            };

        return selectedCampaigns
            .Select(row => new RangoCampana
            {
                ClaveCampana = row.ClaveCampana,
                CodigoCampana = row.CodigoCampana,
                NombreCampana = row.NombreCampana,
                FechaDesde = !requestedDateFrom.HasValue
                    || requestedDateFrom.Value < row.FechaDisponibleDesde
                        ? row.FechaDisponibleDesde
                        : requestedDateFrom.Value,
                FechaHasta = !requestedDateTo.HasValue
                    || requestedDateTo.Value > row.FechaDisponibleHasta
                        ? row.FechaDisponibleHasta
                        : requestedDateTo.Value
            })
            .Where(row => row.FechaDesde <= row.FechaHasta);
    }

    private static IQueryable<FilaMetricaCampana> ConstruirFilasCorte(
        AnaliticaDbContext context,
        int claveCliente,
        RendimientoCampanaCarteraRequest request,
        IQueryable<RangoCampana> eligibleCampaigns) =>
        from range in eligibleCampaigns
        join metric in context.MetricasDiariasCarteraAnalitica.AsNoTracking()
            on range.ClaveCampana equals metric.ClaveCampana
        where metric.ClaveCliente == claveCliente
              && metric.TieneCorteOrigen
              && metric.FechaCalendario >= range.FechaDesde
              && metric.FechaCalendario <= range.FechaHasta
              && (!request.IdSubCartera.HasValue
                  || metric.ClaveCartera == request.IdSubCartera.Value)
              && (request.UnidadNegocio == null
                  || context.CarterasAnalitica.Any(portfolio =>
                      portfolio.ClaveCartera == metric.ClaveCartera
                      && portfolio.UnidadNegocioOrigen == request.UnidadNegocio))
        select new FilaMetricaCampana
        {
            ClaveCampana = metric.ClaveCampana,
            ClaveCartera = metric.ClaveCartera,
            FechaCalendario = metric.FechaCalendario,
            ClientesAsignadosCorte = metric.ClientesAsignadosCorte,
            ClientesGestionadosCorte = metric.ClientesGestionadosCorte,
            ClientesPendientesCorte = metric.ClientesPendientesCorte,
            ClientesContactadosCorte = metric.ClientesContactadosCorte,
            EventosGestionDia = metric.EventosGestionDia,
            MontoRecuperadoDia = metric.MontoRecuperadoDia,
            FechaCarga = metric.FechaCarga
        };

    private static async Task<Dictionary<int, MetricasCorteCampana>> CargarMetricasCorteAsync(
        AnaliticaDbContext context,
        int claveCliente,
        RendimientoCampanaCarteraRequest request,
        bool usarDeduplicacionCampana,
        IReadOnlyDictionary<int, DateTime> snapshotDates,
        CancellationToken cancellationToken)
    {
        var campaignKeys = snapshotDates.Keys.ToArray();

        if (usarDeduplicacionCampana)
        {
            var campaignRows = await context.EvolucionDiariaCampanaAnalitica
                .AsNoTracking()
                .Where(row =>
                    row.ClaveCliente == claveCliente
                    && campaignKeys.Contains(row.ClaveCampana))
                .ToListAsync(cancellationToken);

            return campaignRows
                .Where(row =>
                    snapshotDates.TryGetValue(row.ClaveCampana, out var fechaCorte)
                    && row.FechaCalendario == fechaCorte)
                .ToDictionary(
                    row => row.ClaveCampana,
                    row => new MetricasCorteCampana(
                        snapshotDates[row.ClaveCampana],
                        row.ClientesAsignados ?? 0,
                        row.ClientesGestionados ?? 0,
                        row.ClientesPendientes ?? 0,
                        row.ClientesContactados ?? 0,
                        row.FechaCarga));
        }

        var rows = await context.MetricasDiariasCarteraAnalitica
            .AsNoTracking()
            .Where(metric =>
                metric.ClaveCliente == claveCliente
                && campaignKeys.Contains(metric.ClaveCampana)
                && metric.TieneCorteOrigen
                && (!request.IdSubCartera.HasValue
                    || metric.ClaveCartera == request.IdSubCartera.Value)
                && (request.UnidadNegocio == null
                    || context.CarterasAnalitica.Any(portfolio =>
                        portfolio.ClaveCartera == metric.ClaveCartera
                        && portfolio.UnidadNegocioOrigen == request.UnidadNegocio)))
            .Select(metric => new FilaMetricaCampana
            {
                ClaveCampana = metric.ClaveCampana,
                ClaveCartera = metric.ClaveCartera,
                FechaCalendario = metric.FechaCalendario,
                ClientesAsignadosCorte = metric.ClientesAsignadosCorte,
                ClientesGestionadosCorte = metric.ClientesGestionadosCorte,
                ClientesPendientesCorte = metric.ClientesPendientesCorte,
                ClientesContactadosCorte = metric.ClientesContactadosCorte,
                FechaCarga = metric.FechaCarga
            })
            .ToListAsync(cancellationToken);

        return rows
            .Where(row => snapshotDates.TryGetValue(row.ClaveCampana, out var fechaCorte)
                && row.FechaCalendario == fechaCorte)
            .GroupBy(row => row.ClaveCampana)
            .ToDictionary(
                group => group.Key,
                group => new MetricasCorteCampana(
                    snapshotDates[group.Key],
                    group.Sum(row => (long)(row.ClientesAsignadosCorte ?? 0)),
                    group.Sum(row => (long)(row.ClientesGestionadosCorte ?? 0)),
                    group.Sum(row => (long)(row.ClientesPendientesCorte ?? 0)),
                    group.Sum(row => (long)(row.ClientesContactadosCorte ?? 0)),
                    group.Select(row => row.FechaCarga).Max()));
    }

    private static async Task<Dictionary<int, MetricasFlujoCampana>> CargarMetricasFlujoAsync(
        AnaliticaDbContext context,
        int claveCliente,
        RendimientoCampanaCarteraRequest request,
        IQueryable<RangoCampana> eligibleCampaigns,
        CancellationToken cancellationToken)
    {
        var query =
            from range in eligibleCampaigns
            join metric in context.MetricasDiariasCarteraAnalitica.AsNoTracking()
                on range.ClaveCampana equals metric.ClaveCampana
            where metric.ClaveCliente == claveCliente
                  && metric.FechaCalendario >= range.FechaDesde
                  && metric.FechaCalendario <= range.FechaHasta
                  && (!request.IdSubCartera.HasValue
                      || metric.ClaveCartera == request.IdSubCartera.Value)
                  && (request.UnidadNegocio == null
                      || context.CarterasAnalitica.Any(portfolio =>
                          portfolio.ClaveCartera == metric.ClaveCartera
                          && portfolio.UnidadNegocioOrigen == request.UnidadNegocio))
            group metric by range.ClaveCampana
            into grouped
            select new MetricasFlujoCampana(
                grouped.Key,
                grouped.Sum(row => (long)(row.EventosGestionDia ?? 0)),
                grouped.Sum(row => row.MontoRecuperadoDia ?? 0m),
                grouped.Max(row => row.FechaCarga));

        return await query.ToDictionaryAsync(
            row => row.ClaveCampana,
            cancellationToken);
    }

    private static async Task<Dictionary<int, MetricasContactoCampana>> CargarMetricasContactoAsync(
        AnaliticaDbContext context,
        int claveCliente,
        RendimientoCampanaCarteraRequest request,
        bool usarDeduplicacionCampana,
        IQueryable<RangoCampana> eligibleCampaigns,
        CancellationToken cancellationToken)
    {
        var contacts =
            from range in eligibleCampaigns
            join fact in context.HechosContactoDiarioDeudorAnalitica.AsNoTracking()
                on range.ClaveCampana equals fact.ClaveCampana
            join date in context.FechasAnalitica.AsNoTracking()
                on fact.ClaveFecha equals date.ClaveFecha
            where fact.ClaveCliente == claveCliente
                  && date.FechaCalendario >= range.FechaDesde
                  && date.FechaCalendario <= range.FechaHasta
                  && (!request.IdSubCartera.HasValue
                      || fact.ClaveCartera == request.IdSubCartera.Value)
                  && (request.UnidadNegocio == null
                      || context.CarterasAnalitica.Any(portfolio =>
                          portfolio.ClaveCartera == fact.ClaveCartera
                          && portfolio.UnidadNegocioOrigen == request.UnidadNegocio))
            select new
            {
                range.ClaveCampana,
                fact.ClaveCartera,
                fact.IdDeudorOrigen,
                fact.TuvoContactoDirecto,
                fact.TuvoContactoIndirecto,
                fact.TuvoSinContacto,
                fact.FechaCarga
            };

        // Los hechos detallados aún no persisten clave_cliente_maf.
        // Mantener el grano cartera/deudor evita fusionar clientes distintos.
        var directCounts = await contacts
            .Where(row => row.TuvoContactoDirecto)
            .Select(row => new
            {
                row.ClaveCampana,
                row.ClaveCartera,
                row.IdDeudorOrigen
            })
            .Distinct()
            .GroupBy(row => row.ClaveCampana)
            .Select(group => new MetricaCantidadCampana(
                group.Key,
                group.LongCount()))
            .ToDictionaryAsync(row => row.ClaveCampana, cancellationToken);

        var classifiableCounts = await contacts
            .Where(row => row.TuvoContactoDirecto || row.TuvoContactoIndirecto || row.TuvoSinContacto)
            .Select(row => new
            {
                row.ClaveCampana,
                row.ClaveCartera,
                row.IdDeudorOrigen
            })
            .Distinct()
            .GroupBy(row => row.ClaveCampana)
            .Select(group => new MetricaCantidadCampana(
                group.Key,
                group.LongCount()))
            .ToDictionaryAsync(row => row.ClaveCampana, cancellationToken);

        var fechaCarga = await contacts
            .GroupBy(row => row.ClaveCampana)
            .Select(group => new FechaCargaCampana(
                group.Key,
                group.Max(row => row.FechaCarga)))
            .ToDictionaryAsync(row => row.ClaveCampana, cancellationToken);

        return fechaCarga.Keys
            .Union(directCounts.Keys)
            .Union(classifiableCounts.Keys)
            .ToDictionary(
                claveCampana => claveCampana,
                claveCampana => new MetricasContactoCampana(
                    directCounts.GetValueOrDefault(claveCampana)?.Count ?? 0,
                    classifiableCounts.GetValueOrDefault(claveCampana)?.Count ?? 0,
                    fechaCarga.GetValueOrDefault(claveCampana)?.FechaCarga));
    }

    private static async Task<Dictionary<int, MetricasPromesaCampana>> CargarMetricasPromesaAsync(
        AnaliticaDbContext context,
        int claveCliente,
        RendimientoCampanaCarteraRequest request,
        bool usarDeduplicacionCampana,
        IQueryable<RangoCampana> eligibleCampaigns,
        CancellationToken cancellationToken)
    {
        var promises =
            from range in eligibleCampaigns
            join promise in context.PromesasAnalitica.AsNoTracking()
                on range.ClaveCampana equals promise.ClaveCampana
            where promise.ClaveCliente == claveCliente
                  && promise.EsPromesaValida
                  && promise.FechaGestion >= range.FechaDesde
                  && promise.FechaGestion < range.FechaHasta.AddDays(1)
                  && (!request.IdSubCartera.HasValue
                      || promise.ClaveCartera == request.IdSubCartera.Value)
                  && (request.UnidadNegocio == null
                      || context.CarterasAnalitica.Any(portfolio =>
                          portfolio.ClaveCartera == promise.ClaveCartera
                          && portfolio.UnidadNegocioOrigen == request.UnidadNegocio))
            select new
            {
                range.ClaveCampana,
                promise.ClaveCartera,
                promise.IdDeudorOrigen,
                promise.CodigoEstado,
                promise.MontoPagado,
                promise.MontoPromesa,
                promise.FechaCarga
            };

        var totals = await promises
            .GroupBy(row => row.ClaveCampana)
            .Select(group => new TotalesPromesaCampana(
                group.Key,
                group.LongCount(),
                group.Sum(row => EstadosCumplidos.Contains(row.CodigoEstado)
                    ? row.MontoPagado ?? 0m
                    : 0m),
                group.Sum(row => EstadosCumplimiento.Contains(row.CodigoEstado)
                    ? row.MontoPromesa ?? 0m
                    : 0m),
                group.Max(row => row.FechaCarga)))
            .ToDictionaryAsync(row => row.ClaveCampana, cancellationToken);

        var debtorCounts = await promises
            .Select(row => new
            {
                row.ClaveCampana,
                row.ClaveCartera,
                row.IdDeudorOrigen
            })
            .Distinct()
            .GroupBy(row => row.ClaveCampana)
            .Select(group => new MetricaCantidadCampana(
                group.Key,
                group.LongCount()))
            .ToDictionaryAsync(row => row.ClaveCampana, cancellationToken);

        return totals.Keys
            .Union(debtorCounts.Keys)
            .ToDictionary(
                claveCampana => claveCampana,
                claveCampana =>
                {
                    var totalsRow = totals.GetValueOrDefault(claveCampana);
                    return new MetricasPromesaCampana(
                        totalsRow?.CantidadPromesas ?? 0,
                        debtorCounts.GetValueOrDefault(claveCampana)?.Count ?? 0,
                        totalsRow?.FulfillmentPaidAmount ?? 0m,
                        totalsRow?.FulfillmentPromiseAmount ?? 0m,
                        totalsRow?.FechaCarga);
                });
    }

    private static async Task<Dictionary<int, MetricasPagoCampana>> CargarMetricasPagoAsync(
        AnaliticaDbContext context,
        int claveCliente,
        RendimientoCampanaCarteraRequest request,
        bool usarDeduplicacionCampana,
        IQueryable<RangoCampana> eligibleCampaigns,
        CancellationToken cancellationToken)
    {
        var payments =
            from range in eligibleCampaigns
            join fact in context.HechosPagoDiarioDeudorAnalitica.AsNoTracking()
                on range.ClaveCampana equals fact.ClaveCampana
            join date in context.FechasAnalitica.AsNoTracking()
                on fact.ClaveFecha equals date.ClaveFecha
            where fact.ClaveCliente == claveCliente
                  && date.FechaCalendario >= range.FechaDesde
                  && date.FechaCalendario <= range.FechaHasta
                  && (!request.IdSubCartera.HasValue
                      || fact.ClaveCartera == request.IdSubCartera.Value)
                  && (request.UnidadNegocio == null
                      || context.CarterasAnalitica.Any(portfolio =>
                          portfolio.ClaveCartera == fact.ClaveCartera
                          && portfolio.UnidadNegocioOrigen == request.UnidadNegocio))
            select new
            {
                range.ClaveCampana,
                fact.ClaveCartera,
                fact.IdDeudorOrigen,
                fact.FechaCarga
            };

        var counts = await payments
            .Select(row => new
            {
                row.ClaveCampana,
                row.ClaveCartera,
                row.IdDeudorOrigen
            })
            .Distinct()
            .GroupBy(row => row.ClaveCampana)
            .Select(group => new MetricaCantidadCampana(
                group.Key,
                group.LongCount()))
            .ToDictionaryAsync(row => row.ClaveCampana, cancellationToken);

        var fechaCarga = await payments
            .GroupBy(row => row.ClaveCampana)
            .Select(group => new FechaCargaCampana(
                group.Key,
                group.Max(row => row.FechaCarga)))
            .ToDictionaryAsync(row => row.ClaveCampana, cancellationToken);

        return counts.Keys
            .Union(fechaCarga.Keys)
            .ToDictionary(
                claveCampana => claveCampana,
                claveCampana => new MetricasPagoCampana(
                    counts.GetValueOrDefault(claveCampana)?.Count ?? 0,
                    fechaCarga.GetValueOrDefault(claveCampana)?.FechaCarga));
    }

    private static async Task<Dictionary<int, MetricasMetaCampana>> CargarMetricasMetaAsync(
        AnaliticaDbContext context,
        int claveCliente,
        IQueryable<RangoCampana> eligibleCampaigns,
        CancellationToken cancellationToken)
    {
        var targets =
            from range in eligibleCampaigns
            join meta in context.HechosMetaMensualAnalitica.AsNoTracking()
                on range.ClaveCampana equals meta.ClaveCampana
            where meta.ClaveCliente == claveCliente
                  && meta.ClaveCartera == null
            group meta by range.ClaveCampana
            into grouped
            select new MetricasMetaCampana(
                grouped.Key,
                grouped.Max(row => row.MontoMetaRecuperacion),
                grouped.Max(row => row.FechaCorteOrigen));

        return await targets.ToDictionaryAsync(
            row => row.ClaveCampana,
            cancellationToken);
    }

    private static async Task<int?> ResolverClaveClienteAsync(
        AnaliticaDbContext context,
        int idClienteCrm,
        string? unidadNegocio,
        CancellationToken cancellationToken)
    {
        var claveCliente = await context.ClientesAnalitica
            .AsNoTracking()
            .Where(client => client.IdClienteCrm == idClienteCrm)
            .Select(client => (int?)client.ClaveCliente)
            .SingleOrDefaultAsync(cancellationToken);

        if (!claveCliente.HasValue)
        {
            return null;
        }

        if (unidadNegocio is null)
        {
            var unidadesNegocio = await (
                from fact in context.HechosDiariosCarteraAnalitica.AsNoTracking()
                join portfolio in context.CarterasAnalitica.AsNoTracking()
                    on fact.ClaveCartera equals portfolio.ClaveCartera
                where fact.ClaveCliente == claveCliente.Value
                select portfolio.UnidadNegocioOrigen)
                .Distinct()
                .ToListAsync(cancellationToken);

            var normalizedScopes = unidadesNegocio
                .Select(NormalizarUnidadNegocio)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(2)
                .Count();

            return normalizedScopes > 1 ? null : claveCliente;
        }

        var hasBusinessUnit = await (
            from fact in context.HechosDiariosCarteraAnalitica.AsNoTracking()
            join portfolio in context.CarterasAnalitica.AsNoTracking()
                on fact.ClaveCartera equals portfolio.ClaveCartera
            where fact.ClaveCliente == claveCliente.Value
                  && portfolio.UnidadNegocioOrigen == unidadNegocio
            select fact)
            .AnyAsync(cancellationToken);

        return hasBusinessUnit ? claveCliente : null;
    }

    private static string? NormalizarUnidadNegocio(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();

    private static decimal? Dividir(long numerator, long denominator) =>
        denominator == 0
            ? null
            : Redondear((decimal)numerator / denominator, 6);

    private static decimal? Dividir(decimal numerator, decimal denominator) =>
        denominator == 0m
            ? null
            : Redondear(numerator / denominator, 6);

    private static decimal Redondear(decimal value, int decimals) =>
        decimal.Round(value, decimals, MidpointRounding.AwayFromZero);

    private static DateTime? MaximoNullable(IEnumerable<DateTime?> values)
    {
        var populated = values
            .Where(value => value.HasValue)
            .Select(value => value!.Value)
            .ToArray();

        return populated.Length == 0
            ? null
            : populated.Max();
    }

    private sealed class RangoCampana
    {
        public int ClaveCampana { get; init; }
        public string CodigoCampana { get; init; } = string.Empty;
        public string NombreCampana { get; init; } = string.Empty;
        public DateTime FechaDesde { get; init; }
        public DateTime FechaHasta { get; init; }
    }

    private sealed class FilaMetricaCampana
    {
        public int ClaveCampana { get; init; }
        public long ClaveCartera { get; init; }
        public DateTime FechaCalendario { get; init; }
        public int? ClientesAsignadosCorte { get; init; }
        public int? ClientesGestionadosCorte { get; init; }
        public int? ClientesPendientesCorte { get; init; }
        public int? ClientesContactadosCorte { get; init; }
        public int? EventosGestionDia { get; init; }
        public decimal? MontoRecuperadoDia { get; init; }
        public DateTime? FechaCarga { get; init; }
    }

    private sealed record FechaCorteCampana(int ClaveCampana, DateTime FechaCorte);

    private sealed record MetricasCorteCampana(
        DateTime FechaCorte,
        long CarteraAsignada,
        long CarteraGestionada,
        long CarteraPendiente,
        long ContactedPortfolio,
        DateTime? FechaCarga);

    private sealed record MetricasFlujoCampana(
        int ClaveCampana,
        long CantidadGestiones,
        decimal MontoRecuperado,
        DateTime? FechaCarga);

    private sealed record MetricasContactoCampana(
        long DirectContactClients,
        long ClassifiableClients,
        DateTime? FechaCarga);

    private sealed record MetricasPromesaCampana(
        long CantidadPromesas,
        long ClientesPromesaValida,
        decimal FulfillmentPaidAmount,
        decimal FulfillmentPromiseAmount,
        DateTime? FechaCarga);

    private sealed record MetricasPagoCampana(
        long CantidadPagos,
        DateTime? FechaCarga);

    private sealed record MetricasMetaCampana(
        int ClaveCampana,
        decimal? MontoMeta,
        DateTime? FechaCarga);

    private sealed record TotalesPromesaCampana(
        int ClaveCampana,
        long CantidadPromesas,
        decimal FulfillmentPaidAmount,
        decimal FulfillmentPromiseAmount,
        DateTime? FechaCarga);

    private sealed record MetricaCantidadCampana(int ClaveCampana, long Count);

    private sealed record FechaCargaCampana(int ClaveCampana, DateTime? FechaCarga);
}
