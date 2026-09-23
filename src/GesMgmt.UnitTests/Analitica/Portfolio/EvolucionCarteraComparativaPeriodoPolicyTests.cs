using GesMgmt.Domain.Entities.Analitica.CentroControlCartera;

namespace GesMgmt.UnitTests.Analitica.Portfolio;

public sealed class EvolucionCarteraComparativaPeriodoPolicyTests
{
    [Fact]
    public void Resolver_MantieneElMismoTramoRelativoEnElMesHistorico()
    {
        var referencia = CrearContexto(
            "2026-09",
            new DateOnly(2026, 9, 1),
            new DateOnly(2026, 9, 30));
        var rango = new RangoEvolucionCartera(
            new DateOnly(2026, 9, 5),
            new DateOnly(2026, 9, 18));

        var resultado = EvolucionCarteraComparativaPeriodoPolicy.Resolver(
            referencia,
            rango,
            new DateOnly(2026, 8, 1),
            new DateOnly(2026, 8, 31));

        Assert.NotNull(resultado);
        Assert.Equal(new DateOnly(2026, 8, 5), resultado.Value.Rango.FechaDesde);
        Assert.Equal(new DateOnly(2026, 8, 18), resultado.Value.Rango.FechaHasta);
        Assert.True(resultado.Value.CubreHorizonteCompleto);
    }

    [Fact]
    public void Resolver_NoMarcaComoComparableCompletoUnMesQueNoAlcanzaElHorizonte()
    {
        var referencia = CrearContexto(
            "2026-03",
            new DateOnly(2026, 3, 1),
            new DateOnly(2026, 3, 31));
        var rango = new RangoEvolucionCartera(
            new DateOnly(2026, 3, 1),
            new DateOnly(2026, 3, 31));

        var resultado = EvolucionCarteraComparativaPeriodoPolicy.Resolver(
            referencia,
            rango,
            new DateOnly(2026, 2, 1),
            new DateOnly(2026, 2, 28));

        Assert.NotNull(resultado);
        Assert.Equal(new DateOnly(2026, 2, 1), resultado.Value.Rango.FechaDesde);
        Assert.Equal(new DateOnly(2026, 2, 28), resultado.Value.Rango.FechaHasta);
        Assert.False(resultado.Value.CubreHorizonteCompleto);
    }

    [Fact]
    public void Resolver_DescartaMesCuandoNiElInicioDelTramoExiste()
    {
        var referencia = CrearContexto(
            "2026-03",
            new DateOnly(2026, 3, 1),
            new DateOnly(2026, 3, 31));
        var rango = new RangoEvolucionCartera(
            new DateOnly(2026, 3, 30),
            new DateOnly(2026, 3, 31));

        var resultado = EvolucionCarteraComparativaPeriodoPolicy.Resolver(
            referencia,
            rango,
            new DateOnly(2026, 2, 1),
            new DateOnly(2026, 2, 28));

        Assert.Null(resultado);
    }

    private static EvolucionCarteraContexto CrearContexto(
        string codigo,
        DateOnly inicio,
        DateOnly fin) =>
        new(
            ClaveCliente: 1,
            ClaveCampana: 10,
            CodigoCampana: codigo,
            NombreCampana: codigo,
            FechaInicio: inicio,
            FechaFin: fin,
            FechaUltimaEvolucion: fin);
}
