using GesMgmt.Domain.Entities.Analitica.CentroControlCartera;

namespace GesMgmt.UnitTests.Analitica.Portfolio;

public sealed class PromesasCarteraEstadoOperativoTests
{
    private static readonly DateTime Hoy = new(2026, 9, 23);

    [Theory]
    [InlineData("2026-09-22", 100, 0, true)]
    [InlineData("2026-09-22", 100, 40, true)]
    [InlineData("2026-09-22", 100, 100, false)]
    [InlineData("2026-09-23", 100, 0, false)]
    [InlineData("2026-09-24", 100, 0, false)]
    public void EsVencidaConSaldo_UsaFechaYSaldoNoElCodigoEstado(
        string fechaVencimiento,
        decimal montoPromesa,
        decimal montoPagado,
        bool esperado)
    {
        var result = PromesasCarteraEstadoOperativo.EsVencidaConSaldo(
            DateTime.Parse(fechaVencimiento),
            montoPromesa,
            montoPagado,
            Hoy);

        Assert.Equal(esperado, result);
    }

    [Theory]
    [InlineData(null, "sin-pago-registrado")]
    [InlineData(0, "sin-pago-registrado")]
    [InlineData(1, "pago-parcial")]
    [InlineData(40, "pago-parcial")]
    public void ObtenerClaveSituacion_NoConfiaEnPartialDeLaFuente(
        decimal? montoPagado,
        string esperado)
    {
        var result = PromesasCarteraEstadoOperativo.ObtenerClaveSituacion(montoPagado);

        Assert.Equal(esperado, result);
    }

    [Theory]
    [InlineData(100, 0, 100)]
    [InlineData(100, 40, 60)]
    [InlineData(100, 100, 0)]
    [InlineData(100, 120, 0)]
    public void CalcularMontoPendiente_NuncaDevuelveSaldoNegativo(
        decimal montoPromesa,
        decimal montoPagado,
        decimal esperado)
    {
        var result = PromesasCarteraEstadoOperativo.CalcularMontoPendiente(
            montoPromesa,
            montoPagado);

        Assert.Equal(esperado, result);
    }
}
