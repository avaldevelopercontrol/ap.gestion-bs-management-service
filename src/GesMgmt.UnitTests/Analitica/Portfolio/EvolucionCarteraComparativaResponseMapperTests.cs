using GesMgmt.Application.Services.Analitica.CentroControlCartera;
using GesMgmt.Domain.Entities.Analitica.CentroControlCartera;

namespace GesMgmt.UnitTests.Analitica.Portfolio;

public sealed class EvolucionCarteraComparativaResponseMapperTests
{
    [Fact]
    public void Map_SeleccionaMejorMesIndependientePorMetrica()
    {
        var referencia = Contexto("2026-09", 9);
        var rango = new RangoEvolucionCartera(
            new DateOnly(2026, 9, 1),
            new DateOnly(2026, 9, 10));

        EvolucionCarteraSerieHistoricaDb[] historico =
        [
            Serie("2026-08", 8, asignada: 100, gestionada: 80, recuperado: 900m),
            Serie("2026-07", 7, asignada: 100, gestionada: 92, recuperado: 700m),
            Serie("2026-06", 6, asignada: 100, gestionada: 75, recuperado: 1_200m)
        ];

        var response = EvolucionCarteraComparativaResponseMapper.Map(
            referencia,
            rango,
            historico);

        Assert.Equal("2026-08", response.MesAnterior?.Campana.Code);
        Assert.Equal("2026-07", response.MejorAvance?.Campana.Code);
        Assert.Equal("2026-06", response.MejorRecuperacion?.Campana.Code);
        Assert.Equal(3, response.MesesComparablesAvance);
        Assert.Equal(3, response.MesesComparablesRecuperacion);
    }

    [Fact]
    public void Map_ExcluyeSerieIncompletaDeLosMejores_PeroConservaMesAnterior()
    {
        var referencia = Contexto("2026-09", 9);
        var rango = new RangoEvolucionCartera(
            new DateOnly(2026, 9, 1),
            new DateOnly(2026, 9, 10));

        var agosto = Serie(
            "2026-08",
            8,
            asignada: 100,
            gestionada: 99,
            recuperado: 9_999m,
            cubrePeriodo: false);
        var julio = Serie(
            "2026-07",
            7,
            asignada: 100,
            gestionada: 80,
            recuperado: 800m);

        var response = EvolucionCarteraComparativaResponseMapper.Map(
            referencia,
            rango,
            [agosto, julio]);

        Assert.Equal("2026-08", response.MesAnterior?.Campana.Code);
        Assert.False(response.MesAnterior?.CubrePeriodoComparable);
        Assert.Equal("2026-07", response.MejorAvance?.Campana.Code);
        Assert.Equal("2026-07", response.MejorRecuperacion?.Campana.Code);
        Assert.Equal(1, response.MesesComparablesAvance);
        Assert.Equal(1, response.MesesComparablesRecuperacion);
    }

    private static EvolucionCarteraContexto Contexto(
        string codigo,
        int mes)
    {
        var inicio = new DateOnly(2026, mes, 1);
        return new EvolucionCarteraContexto(
            1,
            mes,
            codigo,
            codigo,
            inicio,
            inicio.AddMonths(1).AddDays(-1),
            null);
    }

    private static EvolucionCarteraSerieHistoricaDb Serie(
        string codigo,
        int mes,
        long asignada,
        long gestionada,
        decimal recuperado,
        bool cubrePeriodo = true)
    {
        var contexto = Contexto(codigo, mes);
        var rango = new RangoEvolucionCartera(
            contexto.FechaInicio,
            contexto.FechaInicio.AddDays(9));

        return new EvolucionCarteraSerieHistoricaDb(
            contexto,
            rango,
            cubrePeriodo,
            [
                new EvolucionCarteraDbFila
                {
                    Periodo = rango.FechaDesde.ToDateTime(TimeOnly.MinValue),
                    CarteraAsignada = asignada,
                    CarteraGestionada = 0,
                    CarteraPendiente = asignada,
                    MontoRecuperado = 0m
                },
                new EvolucionCarteraDbFila
                {
                    Periodo = rango.FechaHasta.ToDateTime(TimeOnly.MinValue),
                    CarteraAsignada = asignada,
                    CarteraGestionada = gestionada,
                    CarteraPendiente = asignada - gestionada,
                    MontoRecuperado = recuperado
                }
            ]);
    }
}
