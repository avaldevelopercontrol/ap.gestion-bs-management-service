using GesMgmt.Application.DTOs.Analitica.CentroControlCartera;
using GesMgmt.Domain.Entities.Analitica.CentroControlCartera;

namespace GesMgmt.Application.Services.Analitica.CentroControlCartera;

internal static class EvolucionCarteraComparativaResponseMapper
{
    private const int CantidadMesesHistoricos = 6;

    public static EvolucionCarteraComparativaResponse Map(
        EvolucionCarteraContexto referencia,
        RangoEvolucionCartera rangoReferencia,
        IReadOnlyList<EvolucionCarteraSerieHistoricaDb> historico)
    {
        var mesAnteriorCodigo = referencia.FechaInicio
            .AddMonths(-1)
            .ToString("yyyy-MM");

        var mesAnterior = historico
            .FirstOrDefault(item =>
                string.Equals(
                    item.Contexto.CodigoCampana,
                    mesAnteriorCodigo,
                    StringComparison.OrdinalIgnoreCase));

        var comparablesCompletos = historico
            .Where(item => item.CubrePeriodoComparable && item.Filas.Count > 0)
            .ToArray();

        var comparablesAvance = comparablesCompletos
            .Where(item => item.Filas[^1].CarteraAsignada > 0)
            .ToArray();

        var mejorAvance = comparablesAvance
            .OrderByDescending(CalcularAvanceFinal)
            .ThenByDescending(item => item.Contexto.FechaInicio)
            .FirstOrDefault();

        var mejorRecuperacion = comparablesCompletos
            .OrderByDescending(item => item.Filas[^1].MontoRecuperado)
            .ThenByDescending(item => item.Contexto.FechaInicio)
            .FirstOrDefault();

        return new EvolucionCarteraComparativaResponse(
            new EvolucionCarteraPeriodo(
                rangoReferencia.FechaDesde,
                rangoReferencia.FechaHasta),
            Math.Min(comparablesAvance.Length, CantidadMesesHistoricos),
            Math.Min(comparablesCompletos.Length, CantidadMesesHistoricos),
            MapSerie(mesAnterior),
            MapSerie(mejorAvance),
            MapSerie(mejorRecuperacion));
    }

    private static decimal CalcularAvanceFinal(
        EvolucionCarteraSerieHistoricaDb serie)
    {
        var final = serie.Filas[^1];
        return final.CarteraAsignada <= 0
            ? 0m
            : decimal.Divide(final.CarteraGestionada * 100m, final.CarteraAsignada);
    }

    private static EvolucionCarteraSerieComparativa? MapSerie(
        EvolucionCarteraSerieHistoricaDb? serie)
    {
        if (serie is null || serie.Filas.Count == 0)
        {
            return null;
        }

        return new EvolucionCarteraSerieComparativa(
            new EvolucionCarteraCampana(
                serie.Contexto.CodigoCampana,
                serie.Contexto.NombreCampana),
            new EvolucionCarteraPeriodo(
                serie.Rango.FechaDesde,
                serie.Rango.FechaHasta),
            serie.CubrePeriodoComparable,
            serie.Filas
                .Select(row => new EvolucionCarteraPunto(
                    DateOnly.FromDateTime(row.Periodo),
                    row.CarteraAsignada,
                    row.CarteraGestionada,
                    row.CarteraPendiente,
                    row.MontoRecuperado))
                .ToArray());
    }
}
