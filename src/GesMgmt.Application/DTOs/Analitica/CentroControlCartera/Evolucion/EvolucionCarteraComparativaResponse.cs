namespace GesMgmt.Application.DTOs.Analitica.CentroControlCartera;

public sealed record EvolucionCarteraSerieComparativa(
    EvolucionCarteraCampana Campana,
    EvolucionCarteraPeriodo Periodo,
    bool CubrePeriodoComparable,
    IReadOnlyList<EvolucionCarteraPunto> Evolucion);

public sealed record EvolucionCarteraComparativaResponse(
    EvolucionCarteraPeriodo PeriodoReferencia,
    int MesesComparablesAvance,
    int MesesComparablesRecuperacion,
    EvolucionCarteraSerieComparativa? MesAnterior,
    EvolucionCarteraSerieComparativa? MejorAvance,
    EvolucionCarteraSerieComparativa? MejorRecuperacion);
