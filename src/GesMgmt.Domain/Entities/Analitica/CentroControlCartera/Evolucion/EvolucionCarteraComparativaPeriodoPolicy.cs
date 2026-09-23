namespace GesMgmt.Domain.Entities.Analitica.CentroControlCartera;

public readonly record struct EvolucionCarteraRangoComparable(
    RangoEvolucionCartera Rango,
    bool CubreHorizonteCompleto);

public static class EvolucionCarteraComparativaPeriodoPolicy
{
    public static EvolucionCarteraRangoComparable? Resolver(
        EvolucionCarteraContexto referencia,
        RangoEvolucionCartera rangoReferencia,
        DateOnly inicioCampanaHistorica,
        DateOnly finCampanaHistorica)
    {
        var desplazamientoInicio =
            rangoReferencia.FechaDesde.DayNumber - referencia.FechaInicio.DayNumber;
        var desplazamientoFin =
            rangoReferencia.FechaHasta.DayNumber - referencia.FechaInicio.DayNumber;

        var fechaDesde = inicioCampanaHistorica.AddDays(desplazamientoInicio);
        if (fechaDesde > finCampanaHistorica)
        {
            return null;
        }

        var fechaHastaSolicitada = inicioCampanaHistorica.AddDays(desplazamientoFin);
        var cubreHorizonteCompleto = fechaHastaSolicitada <= finCampanaHistorica;
        var fechaHasta = cubreHorizonteCompleto
            ? fechaHastaSolicitada
            : finCampanaHistorica;

        return new EvolucionCarteraRangoComparable(
            new RangoEvolucionCartera(fechaDesde, fechaHasta),
            cubreHorizonteCompleto);
    }
}
