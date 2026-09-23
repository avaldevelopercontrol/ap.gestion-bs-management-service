namespace GesMgmt.Domain.Entities.Analitica.CentroControlCartera;

public sealed record EvolucionCarteraSerieHistoricaDb(
    EvolucionCarteraContexto Contexto,
    RangoEvolucionCartera Rango,
    bool CubrePeriodoComparable,
    IReadOnlyList<EvolucionCarteraDbFila> Filas);
