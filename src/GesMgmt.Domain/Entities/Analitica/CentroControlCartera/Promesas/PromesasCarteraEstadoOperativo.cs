namespace GesMgmt.Domain.Entities.Analitica.CentroControlCartera;

/// <summary>
/// Reglas operativas derivadas de fecha e importes normalizados.
/// No dependen de codigo_estado porque los orígenes pueden clasificarlo
/// con semánticas distintas o inconsistentes.
/// </summary>
public static class PromesasCarteraEstadoOperativo
{
    public const string SinPagoRegistrado = "sin-pago-registrado";
    public const string PagoParcial = "pago-parcial";

    public static bool EsVencidaConSaldo(
        DateTime? fechaVencimiento,
        decimal? montoPromesa,
        decimal? montoPagado,
        DateTime fechaHoy) =>
        fechaVencimiento.HasValue
        && fechaVencimiento.Value < fechaHoy
        && (montoPromesa ?? 0m) > (montoPagado ?? 0m);

    public static decimal CalcularMontoPendiente(
        decimal? montoPromesa,
        decimal? montoPagado)
    {
        var prometido = montoPromesa ?? 0m;
        var pagado = montoPagado ?? 0m;

        return prometido > pagado
            ? prometido - pagado
            : 0m;
    }

    public static string ObtenerClaveSituacion(decimal? montoPagado) =>
        (montoPagado ?? 0m) > 0m
            ? PagoParcial
            : SinPagoRegistrado;
}
