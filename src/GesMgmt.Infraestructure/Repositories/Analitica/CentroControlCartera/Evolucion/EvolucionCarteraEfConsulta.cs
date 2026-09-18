using GesMgmt.Domain.Entities.Analitica.CentroControlCartera;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories.Analitica.CentroControlCartera;

internal static class EvolucionCarteraEfConsulta
{
    private const int IdClienteCrmMaf = 59;
    public static async Task<EvolucionCarteraContexto?> ResolverContextoAsync(
        AnaliticaDbContext context,
        int idClienteCrm,
        string? codigoCampana,
        long? idSubCartera,
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

        if (unidadNegocio is null
            && await TieneAlcanceUnidadNegocioAmbiguoAsync(
                context,
                claveCliente.Value,
                cancellationToken))
        {
            return null;
        }

        var candidateCampaigns = context.CampanasAnalitica
            .AsNoTracking()
            .Where(campana => campana.ClaveCliente == claveCliente.Value);

        if (codigoCampana is not null)
        {
            candidateCampaigns = candidateCampaigns.Where(
                campana => campana.CodigoCampana == codigoCampana);
        }
        else if (unidadNegocio is not null)
        {
            var latestCampaignKey = await context.CampanasAnalitica
                .AsNoTracking()
                .Where(campana =>
                    campana.ClaveCliente == claveCliente.Value
                    && context.HechosDiariosCarteraAnalitica.Any(fact =>
                        fact.ClaveCliente == campana.ClaveCliente
                        && fact.ClaveCampana == campana.ClaveCampana))
                .OrderByDescending(campana => campana.FechaInicio)
                .ThenByDescending(campana => campana.ClaveCampana)
                .Select(campana => (int?)campana.ClaveCampana)
                .FirstOrDefaultAsync(cancellationToken);

            if (!latestCampaignKey.HasValue)
            {
                return null;
            }

            candidateCampaigns = candidateCampaigns.Where(
                campana => campana.ClaveCampana == latestCampaignKey.Value);
        }

        var campanas = await candidateCampaigns.ToListAsync(cancellationToken);
        if (campanas.Count == 0)
        {
            return null;
        }

        var campaignKeys = campanas
            .Select(campana => campana.ClaveCampana)
            .ToArray();

        var latestEvolucionDates = await CargarUltimasFechasEvolucionAsync(
            context,
            claveCliente.Value,
            campaignKeys,
            idSubCartera,
            unidadNegocio,
            cancellationToken);

        var latestPortfolioDates = idSubCartera.HasValue
            ? await CargarUltimasFechasCarteraAsync(
                context,
                claveCliente.Value,
                campaignKeys,
                idSubCartera.Value,
                unidadNegocio,
                cancellationToken)
            : new Dictionary<int, DateTime?>();

        var selected = campanas
            .Select(campana => new CampanaSeleccionada(
                campana,
                latestEvolucionDates.GetValueOrDefault(campana.ClaveCampana),
                latestPortfolioDates.GetValueOrDefault(campana.ClaveCampana)))
            .Where(item =>
                idSubCartera.HasValue
                    ? item.LatestPortfolioDataDate.HasValue
                    : unidadNegocio is null || item.FechaUltimaEvolucion.HasValue)
            .OrderBy(item => item.FechaUltimaEvolucion.HasValue ? 0 : 1)
            .ThenByDescending(item => item.Campana.FechaInicio)
            .ThenByDescending(item => item.Campana.ClaveCampana)
            .FirstOrDefault();

        if (selected is null)
        {
            return null;
        }

        return new EvolucionCarteraContexto(
            claveCliente.Value,
            selected.Campana.ClaveCampana,
            selected.Campana.CodigoCampana,
            selected.Campana.NombreCampana,
            DateOnly.FromDateTime(selected.Campana.FechaInicio),
            DateOnly.FromDateTime(selected.Campana.FechaFin),
            selected.FechaUltimaEvolucion.HasValue
                ? DateOnly.FromDateTime(selected.FechaUltimaEvolucion.Value)
                : null);
    }

    public static async Task<IReadOnlyList<EvolucionCarteraDbFila>> ObtenerAsync(
        AnaliticaDbContext context,
        int claveCliente,
        int claveCampana,
        long? idSubCartera,
        string? unidadNegocio,
        RangoEvolucionCartera range,
        CancellationToken cancellationToken)
    {
        var fechaDesde = range.FechaDesde.ToDateTime(TimeOnly.MinValue);
        var dateToExclusive = range.FechaHasta
            .AddDays(1)
            .ToDateTime(TimeOnly.MinValue);

        var usarEvolucionCampana = idSubCartera is null
            && (unidadNegocio is null
                || await EsClienteMafAsync(
                    context,
                    claveCliente,
                    cancellationToken));

        if (usarEvolucionCampana)
        {
            var rows = await context.EvolucionDiariaCampanaAnalitica
                .AsNoTracking()
                .Where(row =>
                    row.ClaveCliente == claveCliente
                    && row.ClaveCampana == claveCampana
                    && row.FechaCalendario >= fechaDesde
                    && row.FechaCalendario < dateToExclusive)
                .OrderBy(row => row.FechaCalendario)
                .ToListAsync(cancellationToken);

            return rows
                .Select(row => new EvolucionCarteraDbFila
                {
                    Periodo = row.FechaCalendario,
                    CarteraAsignada = row.ClientesAsignados ?? 0,
                    CarteraGestionada = row.ClientesGestionados ?? 0,
                    CarteraPendiente = row.ClientesPendientes ?? 0,
                    MontoRecuperado = RedondearMonto(row.MontoRecuperadoAcumulado ?? 0m),
                    FechaCargaUtc = row.FechaCarga
                })
                .ToArray();
        }

        var scopedRows = await AplicarAlcanceCartera(
                context,
                context.EvolucionDiariaCarteraAnalitica
                    .AsNoTracking()
                    .Where(row =>
                        row.ClaveCliente == claveCliente
                        && row.ClaveCampana == claveCampana
                        && row.FechaCalendario >= fechaDesde
                        && row.FechaCalendario < dateToExclusive),
                idSubCartera,
                unidadNegocio)
            .ToListAsync(cancellationToken);

        if (idSubCartera.HasValue)
        {
            return scopedRows
                .OrderBy(row => row.FechaCalendario)
                .Select(row => new EvolucionCarteraDbFila
                {
                    Periodo = row.FechaCalendario,
                    CarteraAsignada = row.ClientesAsignados ?? 0,
                    CarteraGestionada = row.ClientesGestionados ?? 0,
                    CarteraPendiente = row.ClientesPendientes ?? 0,
                    MontoRecuperado = RedondearMonto(row.MontoRecuperadoAcumulado ?? 0m),
                    FechaCargaUtc = row.FechaCarga
                })
                .ToArray();
        }

        return scopedRows
            .GroupBy(row => row.FechaCalendario)
            .OrderBy(group => group.Key)
            .Select(group => new EvolucionCarteraDbFila
            {
                Periodo = group.Key,
                CarteraAsignada = group.Sum(row => (long)(row.ClientesAsignados ?? 0)),
                CarteraGestionada = group.Sum(row => (long)(row.ClientesGestionados ?? 0)),
                CarteraPendiente = group.Sum(row => (long)(row.ClientesPendientes ?? 0)),
                MontoRecuperado = RedondearMonto(
                    group.Sum(row => row.MontoRecuperadoAcumulado ?? 0m)),
                FechaCargaUtc = group.Max(row => row.FechaCarga)
            })
            .ToArray();
    }

    private static IQueryable<EvolucionDiariaCarteraAnalitica> AplicarAlcanceCartera(
        AnaliticaDbContext context,
        IQueryable<EvolucionDiariaCarteraAnalitica> query,
        long? idSubCartera,
        string? unidadNegocio)
    {
        if (idSubCartera.HasValue)
        {
            query = query.Where(row => row.ClaveCartera == idSubCartera.Value);
        }

        if (unidadNegocio is null)
        {
            return query;
        }

        return from row in query
               join portfolio in context.CarterasAnalitica.AsNoTracking()
                   on row.ClaveCartera equals portfolio.ClaveCartera
               where portfolio.UnidadNegocioOrigen == unidadNegocio
               select row;
    }

    private static async Task<bool> TieneAlcanceUnidadNegocioAmbiguoAsync(
        AnaliticaDbContext context,
        int claveCliente,
        CancellationToken cancellationToken)
    {
        var unidadesNegocio = await (
            from fact in context.HechosDiariosCarteraAnalitica.AsNoTracking()
            join portfolio in context.CarterasAnalitica.AsNoTracking()
                on fact.ClaveCartera equals portfolio.ClaveCartera
            where fact.ClaveCliente == claveCliente
            select portfolio.UnidadNegocioOrigen)
            .Distinct()
            .ToListAsync(cancellationToken);

        return unidadesNegocio
            .Select(NormalizarUnidadNegocio)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(2)
            .Count() > 1;
    }

    private static async Task<Dictionary<int, DateTime?>> CargarUltimasFechasEvolucionAsync(
        AnaliticaDbContext context,
        int claveCliente,
        IReadOnlyCollection<int> campaignKeys,
        long? idSubCartera,
        string? unidadNegocio,
        CancellationToken cancellationToken)
    {
        var query = context.EvolucionDiariaCarteraAnalitica
            .AsNoTracking()
            .Where(row =>
                row.ClaveCliente == claveCliente
                && campaignKeys.Contains(row.ClaveCampana));

        query = AplicarAlcanceCartera(context, query, idSubCartera, unidadNegocio);

        return await query
            .GroupBy(row => row.ClaveCampana)
            .Select(group => new
            {
                ClaveCampana = group.Key,
                LatestDate = (DateTime?)group.Max(row => row.FechaCalendario)
            })
            .ToDictionaryAsync(
                item => item.ClaveCampana,
                item => item.LatestDate,
                cancellationToken);
    }

    private static async Task<Dictionary<int, DateTime?>> CargarUltimasFechasCarteraAsync(
        AnaliticaDbContext context,
        int claveCliente,
        IReadOnlyCollection<int> campaignKeys,
        long idSubCartera,
        string? unidadNegocio,
        CancellationToken cancellationToken)
    {
        var query = context.MetricasDiariasCarteraAnalitica
            .AsNoTracking()
            .Where(row =>
                row.ClaveCliente == claveCliente
                && campaignKeys.Contains(row.ClaveCampana)
                && row.ClaveCartera == idSubCartera);

        if (unidadNegocio is null)
        {
            return await query
                .GroupBy(row => row.ClaveCampana)
                .Select(group => new
                {
                    ClaveCampana = group.Key,
                    LatestDate = (DateTime?)group.Max(row => row.FechaCalendario)
                })
                .ToDictionaryAsync(
                    item => item.ClaveCampana,
                    item => item.LatestDate,
                    cancellationToken);
        }

        return await (
            from row in query
            join portfolio in context.CarterasAnalitica.AsNoTracking()
                on row.ClaveCartera equals portfolio.ClaveCartera
            where portfolio.UnidadNegocioOrigen == unidadNegocio
            group row by row.ClaveCampana
            into grouped
            select new
            {
                ClaveCampana = grouped.Key,
                LatestDate = (DateTime?)grouped.Max(row => row.FechaCalendario)
            })
            .ToDictionaryAsync(
                item => item.ClaveCampana,
                item => item.LatestDate,
                cancellationToken);
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

    private static decimal RedondearMonto(decimal value) =>
        decimal.Round(value, 4, MidpointRounding.AwayFromZero);

    private static string? NormalizarUnidadNegocio(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private sealed record CampanaSeleccionada(
        DimensionCampanaAnalitica Campana,
        DateTime? FechaUltimaEvolucion,
        DateTime? LatestPortfolioDataDate);
}
