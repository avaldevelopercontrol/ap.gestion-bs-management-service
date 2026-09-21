using GesMgmt.Infraestructure.Repositories.Analitica.CentroControlCartera;

namespace GesMgmt.UnitTests.Analitica.Portfolio;

public sealed class PromesasCarteraFechaActualTests
{
    [Theory]
    [InlineData("2026-09-21T04:59:59Z", 2026, 9, 20)]
    [InlineData("2026-09-21T05:00:00Z", 2026, 9, 21)]
    [InlineData("2026-09-22T04:59:59Z", 2026, 9, 21)]
    public void ObtenerHoyPeru_RespetaElDiaCalendarioDePeru(
        string utcNow,
        int year,
        int month,
        int day)
    {
        var expectedDate = new DateTime(year, month, day);
        var timeProvider = new FixedTimeProvider(DateTimeOffset.Parse(utcNow));

        var result = PromesasCarteraFechaActual.ObtenerHoyPeru(timeProvider);

        Assert.Equal(expectedDate, result.FechaDesde);
        Assert.Equal(expectedDate.AddDays(1), result.FechaHastaExclusiva);
    }

    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => utcNow;
    }
}
