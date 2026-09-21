using GesMgmt.Domain.Entities.Analitica.CentroControlCartera;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories.Analitica.CentroControlCartera;

internal static class PromesasCarteraEfConsulta
{
    public static async Task<PromesasCarteraContexto?> ResolverContextoAsync(
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

        var promiseRows = await AplicarAlcanceCartera(
                context,
                context.PromesaOperativaAnalitica
                    .AsNoTracking()
                    .Where(row =>
                        row.ClaveCliente == claveCliente.Value
                        && campaignKeys.Contains(row.ClaveCampana)),
                idSubCartera,
                unidadNegocio)
            .Select(row => new
            {
                row.ClaveCampana,
                row.FechaCarga
            })
            .ToListAsync(cancellationToken);

        var promiseStats = promiseRows
            .GroupBy(row => row.ClaveCampana)
            .ToDictionary(
                group => group.Key,
                group => new EstadisticasPromesa(
                    group.LongCount(),
                    group.Max(row => row.FechaCarga)));

        var latestPortfolioDates = await CargarUltimasFechasCarteraAsync(
            context,
            claveCliente.Value,
            campaignKeys,
            idSubCartera,
            unidadNegocio,
            cancellationToken);

        var selected = campanas
            .Select(campana => new CampanaSeleccionada(
                campana,
                promiseStats.GetValueOrDefault(campana.ClaveCampana),
                latestPortfolioDates.GetValueOrDefault(campana.ClaveCampana)))
            .Where(item =>
                (idSubCartera is null && unidadNegocio is null)
                || item.LatestPortfolioDataDate.HasValue)
            .OrderBy(item => item.EstadisticasPromesa?.CantidadPromesas > 0 ? 0 : 1)
            .ThenByDescending(item => item.EstadisticasPromesa?.LatestLoadedAt)
            .ThenByDescending(item => item.Campana.FechaInicio)
            .ThenByDescending(item => item.Campana.ClaveCampana)
            .FirstOrDefault();

        if (selected is null)
        {
            return null;
        }

        return new PromesasCarteraContexto(
            claveCliente.Value,
            selected.Campana.ClaveCampana,
            selected.Campana.CodigoCampana,
            selected.Campana.NombreCampana);
    }

    public static async Task<PromesasCarteraDbFila> ObtenerOperacionalAsync(
        AnaliticaDbContext context,
        int claveCliente,
        int claveCampana,
        long? idSubCartera,
        string? unidadNegocio,
        PromesasCarteraRangoDia rangoHoyPeru,
        CancellationToken cancellationToken)
    {
        var rows = await AplicarAlcanceCartera(
                context,
                context.PromesaOperativaAnalitica
                    .AsNoTracking()
                    .Where(row =>
                        row.ClaveCliente == claveCliente
                        && row.ClaveCampana == claveCampana
                        && row.EsPromesaValida),
                idSubCartera,
                unidadNegocio)
            .ToListAsync(cancellationToken);

        var dueToday = rows
            .Where(row =>
                row.FechaVencimientoPromesa >= rangoHoyPeru.FechaDesde
                && row.FechaVencimientoPromesa < rangoHoyPeru.FechaHastaExclusiva)
            .ToArray();
        var fulfillmentRows = rows
            .Where(row => row.EstaCumplidaOParcial || row.EstaRota)
            .ToArray();

        var fulfillmentPaidAmount = rows
            .Where(row => row.EstaCumplidaOParcial)
            .Sum(row => row.MontoPagado ?? 0m);

        var fulfillmentPromiseAmount = fulfillmentRows
            .Sum(row => row.MontoPromesa ?? 0m);

        return new PromesasCarteraDbFila
        {
            CantidadVenceHoy = dueToday.LongLength,
            MontoVenceHoy = RedondearMonto(
                dueToday.Sum(row => row.MontoPromesa ?? 0m)),
            CantidadVencidas = rows.LongCount(row => row.EstaRota),
            TasaCumplimiento = Dividir(
                fulfillmentPaidAmount,
                fulfillmentPromiseAmount),
            FechaActualizacionUtc = rows.Count == 0
                ? null
                : rows.Max(row => row.FechaCarga)
        };
    }

    private static IQueryable<PromesaOperativaAnalitica> AplicarAlcanceCartera(
        AnaliticaDbContext context,
        IQueryable<PromesaOperativaAnalitica> query,
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

    private static async Task<Dictionary<int, DateTime?>> CargarUltimasFechasCarteraAsync(
        AnaliticaDbContext context,
        int claveCliente,
        IReadOnlyCollection<int> campaignKeys,
        long? idSubCartera,
        string? unidadNegocio,
        CancellationToken cancellationToken)
    {
        var query = context.MetricasDiariasCarteraAnalitica
            .AsNoTracking()
            .Where(row =>
                row.ClaveCliente == claveCliente
                && campaignKeys.Contains(row.ClaveCampana));

        if (idSubCartera.HasValue)
        {
            query = query.Where(row => row.ClaveCartera == idSubCartera.Value);
        }

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

    private static decimal? Dividir(decimal numerator, decimal denominator)
    {
        if (denominator == 0m)
        {
            return null;
        }

        return decimal.Round(
            numerator / denominator,
            6,
            MidpointRounding.AwayFromZero);
    }

    private static decimal RedondearMonto(decimal value) =>
        decimal.Round(value, 4, MidpointRounding.AwayFromZero);

    private static string? NormalizarUnidadNegocio(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private sealed record EstadisticasPromesa(
        long CantidadPromesas,
        DateTime? LatestLoadedAt);

    private sealed record CampanaSeleccionada(
        DimensionCampanaAnalitica Campana,
        EstadisticasPromesa? EstadisticasPromesa,
        DateTime? LatestPortfolioDataDate);
}
