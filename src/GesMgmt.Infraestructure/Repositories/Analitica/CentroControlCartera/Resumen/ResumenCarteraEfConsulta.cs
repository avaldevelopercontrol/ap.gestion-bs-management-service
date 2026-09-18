using GesMgmt.Domain.Entities.Analitica.CentroControlCartera;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories.Analitica.CentroControlCartera;

internal static class ResumenCarteraEfConsulta
{
    private const int IdClienteCrmMaf = 59;
    private const string CodigoOrigenMaf = "MAF_DAILY";

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

    private static readonly string[] CodigosOrigenVigencia =
    [
        "CLARO_INTRADAY_UPSTREAM",
        "GESTION_COB2_LIVE",
        "CLARO_ADVISOR_DAILY",
        "CLARO_PORTFOLIO_SNAPSHOT"
    ];

    public static async Task<ResumenCarteraDbFila> EjecutarAsync(
        AnaliticaDbContext context,
        int claveCliente,
        int claveCampana,
        long? idSubCartera,
        string? unidadNegocio,
        RangoResumenCartera range,
        CancellationToken cancellationToken)
    {
        var fechaDesde = range.FechaDesde.ToDateTime(TimeOnly.MinValue);
        var dateToExclusive = range.FechaHasta
            .AddDays(1)
            .ToDateTime(TimeOnly.MinValue);

        var esMaf = await EsClienteMafAsync(
            context,
            claveCliente,
            cancellationToken);

        var usarDeduplicacionCampana = esMaf && !idSubCartera.HasValue;

        var snapshot = await ObtenerCorteAsync(
            context,
            claveCliente,
            claveCampana,
            idSubCartera,
            unidadNegocio,
            usarDeduplicacionCampana,
            fechaDesde,
            dateToExclusive,
            cancellationToken);

        var flow = await ObtenerFlujoAsync(
            context,
            claveCliente,
            claveCampana,
            idSubCartera,
            unidadNegocio,
            fechaDesde,
            dateToExclusive,
            cancellationToken);

        var contact = await ObtenerMetricasContactoAsync(
            context,
            claveCliente,
            claveCampana,
            idSubCartera,
            unidadNegocio,
            usarDeduplicacionCampana,
            fechaDesde,
            dateToExclusive,
            cancellationToken);

        var promise = await ObtenerMetricasPromesaAsync(
            context,
            claveCliente,
            claveCampana,
            idSubCartera,
            unidadNegocio,
            usarDeduplicacionCampana,
            fechaDesde,
            dateToExclusive,
            cancellationToken);

        var payment = await ObtenerMetricasPagoAsync(
            context,
            claveCliente,
            claveCampana,
            idSubCartera,
            unidadNegocio,
            usarDeduplicacionCampana,
            fechaDesde,
            dateToExclusive,
            cancellationToken);

        var vigencia = await ObtenerVigenciaAsync(
            context,
            esMaf,
            cancellationToken);

        var fechaActualizacionUtc = new DateTime?[]
        {
            snapshot.FechaCargaUtc,
            flow.FechaCargaUtc,
            contact.FechaCargaUtc,
            promise.FechaCargaUtc,
            payment.FechaCargaUtc
        }.Max();

        return new ResumenCarteraDbFila
        {
            FechaCorte = snapshot.FechaCorte,
            CarteraAsignada = snapshot.CarteraAsignada,
            CarteraGestionada = snapshot.CarteraGestionada,
            CarteraPendiente = snapshot.CarteraPendiente,
            CantidadGestiones = flow.CantidadGestiones,
            IntensidadGestion = Dividir(flow.CantidadGestiones, snapshot.CarteraGestionada),
            MontoRecuperado = Redondear(flow.MontoRecuperado, 4),
            TasaContactabilidad = snapshot.ContactedPortfolio.HasValue
                ? Dividir(snapshot.ContactedPortfolio.Value, snapshot.CarteraAsignada)
                : null,
            TasaContactoDirecto = Dividir(contact.DirectContactClients, contact.ClassifiableClients),
            TasaCierre = Dividir(promise.ClientesPromesaValida, contact.DirectContactClients),
            CantidadPromesas = promise.CantidadPromesas,
            TasaCumplimientoPromesa = Dividir(
                promise.FulfillmentPaidAmount,
                promise.FulfillmentPromiseAmount),
            CantidadPagos = payment.CantidadPagos,
            FechaActualizacionUtc = fechaActualizacionUtc,
            FechaCorteOperacionLocal = vigencia.FechaCorteOperacionLocal,
            FechaActualizacionBaseCarteraUtc = vigencia.FechaActualizacionBaseCarteraUtc,
            FechaActualizacionDatosUtc = vigencia.FechaActualizacionDatosUtc
        };
    }

    private static async Task<MetricasCorte> ObtenerCorteAsync(
        AnaliticaDbContext context,
        int claveCliente,
        int claveCampana,
        long? idSubCartera,
        string? unidadNegocio,
        bool usarDeduplicacionCampana,
        DateTime fechaDesde,
        DateTime dateToExclusive,
        CancellationToken cancellationToken)
    {
        if (usarDeduplicacionCampana)
        {
            var corteCampana = await context.EvolucionDiariaCampanaAnalitica
                .AsNoTracking()
                .Where(row =>
                    row.ClaveCliente == claveCliente
                    && row.ClaveCampana == claveCampana
                    && row.FechaCalendario >= fechaDesde
                    && row.FechaCalendario < dateToExclusive)
                .OrderByDescending(row => row.FechaCalendario)
                .Select(row => new MetricasCorte(
                    row.FechaCalendario,
                    row.ClientesAsignados ?? 0,
                    row.ClientesGestionados ?? 0,
                    row.ClientesPendientes ?? 0,
                    row.ClientesContactados,
                    row.FechaCarga))
                .FirstOrDefaultAsync(cancellationToken);

            if (corteCampana is not null)
            {
                return corteCampana;
            }
        }

        var realSnapshot = await (
            from fact in context.HechosDiariosCarteraAnalitica.AsNoTracking()
            join date in context.FechasAnalitica.AsNoTracking()
                on fact.ClaveFecha equals date.ClaveFecha
            where fact.ClaveCliente == claveCliente
                && fact.ClaveCampana == claveCampana
                && fact.TieneCorteOrigen
                && date.FechaCalendario >= fechaDesde
                && date.FechaCalendario < dateToExclusive
                && (!idSubCartera.HasValue || fact.ClaveCartera == idSubCartera.Value)
                && (unidadNegocio == null || context.CarterasAnalitica.Any(
                    portfolio =>
                        portfolio.ClaveCartera == fact.ClaveCartera
                        && portfolio.UnidadNegocioOrigen == unidadNegocio))
            group fact by date.FechaCalendario
            into grouped
            orderby grouped.Key descending
            select new MetricasCorte(
                grouped.Key,
                (long)(grouped.Sum(row => row.ClientesAsignadosCorte) ?? 0),
                (long)(grouped.Sum(row => row.ClientesGestionadosCorte) ?? 0),
                (long)(grouped.Sum(row => row.ClientesPendientesCorte) ?? 0),
                grouped.Any(row => row.ClientesContactadosCorte.HasValue)
                    ? (long?)grouped.Sum(row => row.ClientesContactadosCorte)
                    : null,
                grouped.Max(row => row.FechaCarga)))
            .FirstOrDefaultAsync(cancellationToken);

        if (realSnapshot is not null)
        {
            return realSnapshot;
        }

        var fallbackSnapshot = await (
            from fact in context.HechosEvolucionDiariaCarteraAnalitica.AsNoTracking()
            join date in context.FechasAnalitica.AsNoTracking()
                on fact.ClaveFecha equals date.ClaveFecha
            where fact.ClaveCliente == claveCliente
                && fact.ClaveCampana == claveCampana
                && date.FechaCalendario >= fechaDesde
                && date.FechaCalendario < dateToExclusive
                && (!idSubCartera.HasValue || fact.ClaveCartera == idSubCartera.Value)
                && (unidadNegocio == null || context.CarterasAnalitica.Any(
                    portfolio =>
                        portfolio.ClaveCartera == fact.ClaveCartera
                        && portfolio.UnidadNegocioOrigen == unidadNegocio))
            group fact by date.FechaCalendario
            into grouped
            orderby grouped.Key descending
            select new MetricasCorte(
                grouped.Key,
                (long)(grouped.Sum(row => row.ClientesAsignados) ?? 0),
                (long)(grouped.Sum(row => row.ClientesGestionados) ?? 0),
                (long)(grouped.Sum(row => row.ClientesPendientes) ?? 0),
                null,
                grouped.Max(row => row.FechaCarga)))
            .FirstOrDefaultAsync(cancellationToken);

        return fallbackSnapshot ?? MetricasCorte.Empty;
    }

    private static async Task<MetricasFlujo> ObtenerFlujoAsync(
        AnaliticaDbContext context,
        int claveCliente,
        int claveCampana,
        long? idSubCartera,
        string? unidadNegocio,
        DateTime fechaDesde,
        DateTime dateToExclusive,
        CancellationToken cancellationToken)
    {
        var metrics = context.MetricasDiariasCarteraAnalitica
            .AsNoTracking()
            .Where(row =>
                row.ClaveCliente == claveCliente
                && row.ClaveCampana == claveCampana
                && row.FechaCalendario >= fechaDesde
                && row.FechaCalendario < dateToExclusive
                && (!idSubCartera.HasValue || row.ClaveCartera == idSubCartera.Value)
                && (unidadNegocio == null || context.CarterasAnalitica.Any(
                    portfolio =>
                        portfolio.ClaveCartera == row.ClaveCartera
                        && portfolio.UnidadNegocioOrigen == unidadNegocio)));

        return await metrics
            .GroupBy(_ => 1)
            .Select(grouped => new MetricasFlujo(
                (long)(grouped.Sum(row => row.EventosGestionDia) ?? 0),
                grouped.Sum(row => row.MontoRecuperadoDia) ?? 0m,
                grouped.Max(row => row.FechaCarga)))
            .SingleOrDefaultAsync(cancellationToken)
            ?? MetricasFlujo.Empty;
    }

    private static async Task<MetricasContacto> ObtenerMetricasContactoAsync(
        AnaliticaDbContext context,
        int claveCliente,
        int claveCampana,
        long? idSubCartera,
        string? unidadNegocio,
        bool usarDeduplicacionCampana,
        DateTime fechaDesde,
        DateTime dateToExclusive,
        CancellationToken cancellationToken)
    {
        var contacts =
            from fact in context.HechosContactoDiarioDeudorAnalitica.AsNoTracking()
            join date in context.FechasAnalitica.AsNoTracking()
                on fact.ClaveFecha equals date.ClaveFecha
            where fact.ClaveCliente == claveCliente
                && fact.ClaveCampana == claveCampana
                && date.FechaCalendario >= fechaDesde
                && date.FechaCalendario < dateToExclusive
                && (!idSubCartera.HasValue || fact.ClaveCartera == idSubCartera.Value)
                && (unidadNegocio == null || context.CarterasAnalitica.Any(
                    portfolio =>
                        portfolio.ClaveCartera == fact.ClaveCartera
                        && portfolio.UnidadNegocioOrigen == unidadNegocio))
            select fact;

        // Los hechos detallados aún no persisten clave_cliente_maf.
        // Deduplicar solo por IdDeudorOrigen fusiona clientes MAF distintos.
        // Conservamos el grano físico hasta persistir la clave de negocio.
        var directContactClients = await contacts
            .Where(row => row.TuvoContactoDirecto)
            .Select(row => new { row.ClaveCartera, row.IdDeudorOrigen })
            .Distinct()
            .LongCountAsync(cancellationToken);

        var classifiableClients = await contacts
            .Where(row =>
                row.TuvoContactoDirecto
                || row.TuvoContactoIndirecto
                || row.TuvoSinContacto)
            .Select(row => new { row.ClaveCartera, row.IdDeudorOrigen })
            .Distinct()
            .LongCountAsync(cancellationToken);

        var fechaCargaUtc = await contacts
            .Select(row => row.FechaCarga)
            .MaxAsync(cancellationToken);

        return new MetricasContacto(
            directContactClients,
            classifiableClients,
            fechaCargaUtc);
    }

    private static async Task<MetricasPromesa> ObtenerMetricasPromesaAsync(
        AnaliticaDbContext context,
        int claveCliente,
        int claveCampana,
        long? idSubCartera,
        string? unidadNegocio,
        bool usarDeduplicacionCampana,
        DateTime fechaDesde,
        DateTime dateToExclusive,
        CancellationToken cancellationToken)
    {
        var promises = context.PromesasAnalitica
            .AsNoTracking()
            .Where(row =>
                row.ClaveCliente == claveCliente
                && row.ClaveCampana == claveCampana
                && row.EsPromesaValida
                && row.FechaGestion >= fechaDesde
                && row.FechaGestion < dateToExclusive
                && (!idSubCartera.HasValue || row.ClaveCartera == idSubCartera.Value)
                && (unidadNegocio == null || context.CarterasAnalitica.Any(
                    portfolio =>
                        portfolio.ClaveCartera == row.ClaveCartera
                        && portfolio.UnidadNegocioOrigen == unidadNegocio)));

        var aggregate = await promises
            .GroupBy(_ => 1)
            .Select(grouped => new
            {
                CantidadPromesas = grouped.LongCount(),
                FulfillmentPaidAmount = grouped.Sum(row =>
                    EstadosCumplidos.Contains(row.CodigoEstado)
                        ? row.MontoPagado ?? 0m
                        : 0m),
                FulfillmentPromiseAmount = grouped.Sum(row =>
                    EstadosCumplimiento.Contains(row.CodigoEstado)
                        ? row.MontoPromesa ?? 0m
                        : 0m),
                FechaCargaUtc = grouped.Max(row => row.FechaCarga)
            })
            .SingleOrDefaultAsync(cancellationToken);

        var validPromiseClients = await promises
            .Select(row => new { row.ClaveCartera, row.IdDeudorOrigen })
            .Distinct()
            .LongCountAsync(cancellationToken);

        return aggregate is null
            ? new MetricasPromesa(0L, validPromiseClients, 0m, 0m, null)
            : new MetricasPromesa(
                aggregate.CantidadPromesas,
                validPromiseClients,
                aggregate.FulfillmentPaidAmount,
                aggregate.FulfillmentPromiseAmount,
                aggregate.FechaCargaUtc);
    }

    private static async Task<MetricasPago> ObtenerMetricasPagoAsync(
        AnaliticaDbContext context,
        int claveCliente,
        int claveCampana,
        long? idSubCartera,
        string? unidadNegocio,
        bool usarDeduplicacionCampana,
        DateTime fechaDesde,
        DateTime dateToExclusive,
        CancellationToken cancellationToken)
    {
        var payments =
            from fact in context.HechosPagoDiarioDeudorAnalitica.AsNoTracking()
            join date in context.FechasAnalitica.AsNoTracking()
                on fact.ClaveFecha equals date.ClaveFecha
            where fact.ClaveCliente == claveCliente
                && fact.ClaveCampana == claveCampana
                && date.FechaCalendario >= fechaDesde
                && date.FechaCalendario < dateToExclusive
                && (!idSubCartera.HasValue || fact.ClaveCartera == idSubCartera.Value)
                && (unidadNegocio == null || context.CarterasAnalitica.Any(
                    portfolio =>
                        portfolio.ClaveCartera == fact.ClaveCartera
                        && portfolio.UnidadNegocioOrigen == unidadNegocio))
            select fact;

        var cantidadPagos = await payments
            .Select(row => new { row.ClaveCartera, row.IdDeudorOrigen })
            .Distinct()
            .LongCountAsync(cancellationToken);

        var fechaCargaUtc = await payments
            .Select(row => row.FechaCarga)
            .MaxAsync(cancellationToken);

        return new MetricasPago(cantidadPagos, fechaCargaUtc);
    }

    private static async Task<MetricasVigencia> ObtenerVigenciaAsync(
        AnaliticaDbContext context,
        bool esMaf,
        CancellationToken cancellationToken)
    {
        if (esMaf)
        {
            var watermarkMaf = await context.ControlesCargaAnalitica
                .AsNoTracking()
                .Where(row => row.CodigoOrigen == CodigoOrigenMaf)
                .OrderByDescending(row => row.FechaUltimoExito)
                .FirstOrDefaultAsync(cancellationToken);

            return watermarkMaf is null
                ? new MetricasVigencia(null, null, null)
                : new MetricasVigencia(
                    watermarkMaf.FechaHoraUltimoOrigen,
                    watermarkMaf.FechaUltimoExito,
                    watermarkMaf.FechaUltimoExito);
        }

        var watermarks = await context.ControlesCargaAnalitica
            .AsNoTracking()
            .Where(row => CodigosOrigenVigencia.Contains(row.CodigoOrigen))
            .ToListAsync(cancellationToken);

        var intraday = watermarks
            .Where(row => row.CodigoOrigen == "CLARO_INTRADAY_UPSTREAM")
            .ToArray();

        var liveAndAdvisor = watermarks
            .Where(row =>
                row.CodigoOrigen == "GESTION_COB2_LIVE"
                || row.CodigoOrigen == "CLARO_ADVISOR_DAILY")
            .ToArray();

        var fechaCorteOperacionLocal = intraday
            .Select(row => row.FechaHoraUltimoOrigen)
            .Max()
            ?? liveAndAdvisor
                .Select(row => row.FechaHoraUltimoOrigen)
                .Min();

        var fechaActualizacionBaseCarteraUtc = watermarks
            .Where(row => row.CodigoOrigen == "CLARO_PORTFOLIO_SNAPSHOT")
            .Select(row => row.FechaUltimoExito)
            .Max();

        var fechaActualizacionDatosUtc = intraday
            .Select(row => row.FechaUltimoExito)
            .Max()
            ?? liveAndAdvisor
                .Select(row => row.FechaUltimoExito)
                .Max();

        return new MetricasVigencia(
            fechaCorteOperacionLocal,
            fechaActualizacionBaseCarteraUtc,
            fechaActualizacionDatosUtc);
    }

    private static Task<bool> EsClienteMafAsync(
        AnaliticaDbContext context,
        int claveCliente,
        CancellationToken cancellationToken) =>
        context.ClientesAnalitica
            .AsNoTracking()
            .AnyAsync(
                client =>
                    client.ClaveCliente == claveCliente
                    && client.IdClienteCrm == IdClienteCrmMaf,
                cancellationToken);

    private static decimal? Dividir(long numerator, long denominator)
    {
        if (denominator == 0)
        {
            return null;
        }

        return Redondear((decimal)numerator / denominator, 6);
    }

    private static decimal? Dividir(decimal numerator, decimal denominator)
    {
        if (denominator == 0m)
        {
            return null;
        }

        return Redondear(numerator / denominator, 6);
    }

    private static decimal Redondear(decimal value, int decimals) =>
        decimal.Round(value, decimals, MidpointRounding.AwayFromZero);

    private sealed record MetricasCorte(
        DateTime? FechaCorte,
        long CarteraAsignada,
        long CarteraGestionada,
        long CarteraPendiente,
        long? ContactedPortfolio,
        DateTime? FechaCargaUtc)
    {
        public static MetricasCorte Empty { get; } =
            new(null, 0L, 0L, 0L, null, null);
    }

    private sealed record MetricasFlujo(
        long CantidadGestiones,
        decimal MontoRecuperado,
        DateTime? FechaCargaUtc)
    {
        public static MetricasFlujo Empty { get; } = new(0L, 0m, null);
    }

    private sealed record MetricasContacto(
        long DirectContactClients,
        long ClassifiableClients,
        DateTime? FechaCargaUtc);

    private sealed record MetricasPromesa(
        long CantidadPromesas,
        long ClientesPromesaValida,
        decimal FulfillmentPaidAmount,
        decimal FulfillmentPromiseAmount,
        DateTime? FechaCargaUtc);

    private sealed record MetricasPago(
        long CantidadPagos,
        DateTime? FechaCargaUtc);

    private sealed record MetricasVigencia(
        DateTime? FechaCorteOperacionLocal,
        DateTime? FechaActualizacionBaseCarteraUtc,
        DateTime? FechaActualizacionDatosUtc);
}
