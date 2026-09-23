using GesMgmt.Application.Services.Analitica.CentroControlCartera;
using GesMgmt.Domain.Entities.Analitica.CentroControlCartera;

namespace GesMgmt.UnitTests.Analitica.Portfolio;

public sealed class PromesasVencidasCarteraResponseMapperTests
{
    [Theory]
    [InlineData("sin-pago-registrado", "Sin pago registrado")]
    [InlineData("pago-parcial", "Pago parcial")]
    public void Map_ExponeNombreDeudorYSituacionOperativa(
        string claveSituacion,
        string etiquetaEsperada)
    {
        var contexto = new PromesasCarteraContexto(
            1,
            15,
            "2026-09",
            "Septiembre 2026");

        var resultado = new PromesasVencidasCarteraConsultaResult(
            new PromesasVencidasCarteraResumenDbFila
            {
                CantidadVencidas = 1,
                MontoVencido = 500m,
                MontoPendiente = 350m,
                FechaCorte = new DateTime(2026, 9, 23)
            },
            [],
            [
                new PromesaVencidaCarteraDbFila
                {
                    IdPromesa = 901,
                    IdDeudor = 16068,
                    NombreDeudor = "INVERSIONES METCON SAC",
                    FechaVencimiento = new DateTime(2026, 9, 22),
                    DiasVencimiento = 1,
                    MontoPromesa = 500m,
                    MontoPagado = claveSituacion == "pago-parcial" ? 150m : 0m,
                    MontoPendiente = claveSituacion == "pago-parcial" ? 350m : 500m,
                    ClaveSituacion = claveSituacion,
                    ClaveAntiguedad = "1-3"
                }
            ],
            [],
            [],
            PromesasCarteraPaginacion.Crear(1, 20, 1));

        var response = PromesasVencidasCarteraResponseMapper.Map(contexto, resultado);

        var item = Assert.Single(response.Elementos);
        Assert.Equal("INVERSIONES METCON SAC", item.NombreDeudor);
        Assert.Equal(claveSituacion, item.ClaveSituacion);
        Assert.Equal(etiquetaEsperada, item.EtiquetaSituacion);
    }
}
