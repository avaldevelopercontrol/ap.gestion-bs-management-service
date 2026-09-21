namespace GesMgmt.Infraestructure.Repositories.Analitica.CentroControlCartera;

internal readonly record struct PromesasCarteraRangoDia(
    DateTime FechaDesde,
    DateTime FechaHastaExclusiva);

internal static class PromesasCarteraFechaActual
{
    private static readonly TimeSpan PeruUtcOffset = TimeSpan.FromHours(-5);

    public static PromesasCarteraRangoDia ObtenerHoyPeru(
        TimeProvider timeProvider)
    {
        ArgumentNullException.ThrowIfNull(timeProvider);

        var fechaPeru = timeProvider
            .GetUtcNow()
            .ToOffset(PeruUtcOffset)
            .Date;

        return new PromesasCarteraRangoDia(
            fechaPeru,
            fechaPeru.AddDays(1));
    }
}
