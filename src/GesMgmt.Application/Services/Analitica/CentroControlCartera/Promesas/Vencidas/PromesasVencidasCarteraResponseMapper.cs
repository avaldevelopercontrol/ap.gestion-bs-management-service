using GesMgmt.Application.DTOs.Analitica.CentroControlCartera;
using GesMgmt.Domain.Entities.Analitica.CentroControlCartera;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GesMgmt.Application.Services.Analitica.CentroControlCartera;

internal static class PromesasVencidasCarteraResponseMapper
{
    private static readonly IReadOnlyDictionary<string, string> EtiquetasAntiguedad =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["1-3"] = "1 - 3 días",
            ["4-7"] = "4 - 7 días",
            ["8-mas"] = "8+ días",
            ["sin-clasificar"] = "Sin fecha"
        };

    public static PromesasVencidasCarteraResponse Map(
        PromesasCarteraContexto context,
        PromesasVencidasCarteraConsultaResult result)
    {
        return new PromesasVencidasCarteraResponse(
            new PromesasCarteraCampana(context.CodigoCampana, context.NombreCampana),
            result.Resumen.FechaCorte.HasValue
                ? DateOnly.FromDateTime(result.Resumen.FechaCorte.Value)
                : null,
            ConvertirOffsetUtc(result.Resumen.FechaActualizacionUtc),
            new PromesasVencidasCarteraResumen(
                result.Resumen.CantidadVencidas,
                result.Resumen.MontoVencido,
                result.Resumen.MontoPendiente),
            result.Antiguedad
                .OrderBy(item => ObtenerOrdenAntiguedad(item.ClaveAntiguedad))
                .Select(item => new PromesasVencidasCarteraAntiguedadRango(
                    item.ClaveAntiguedad,
                    EtiquetasAntiguedad.GetValueOrDefault(item.ClaveAntiguedad, item.ClaveAntiguedad),
                    item.CantidadPromesas,
                    item.MontoPromesa,
                    item.MontoPendiente))
                .ToArray(),
            new PromesaVencidaCarteraOpcionesFiltro(
                result.Asesores.Select(item =>
                    new PromesaVencidaCarteraFiltroOpcion(item.IdAsesor, item.NombreAsesor)).ToArray(),
                result.Supervisores.Select(item =>
                    new PromesaVencidaCarteraFiltroOpcion(item.IdSupervisor, item.NombreSupervisor)).ToArray()),
            result.Paginacion,
            result.Elementos.Select(item => new PromesaVencidaCarteraItem(
                item.IdPromesa,
                item.IdDeudor,
                item.NombreDeudor,
                item.FechaVencimiento.HasValue ? DateOnly.FromDateTime(item.FechaVencimiento.Value) : null,
                item.DiasVencimiento,
                item.MontoPromesa,
                item.MontoPagado,
                item.MontoPendiente,
                item.ClaveSituacion,
                ObtenerEtiquetaSituacion(item.ClaveSituacion),
                item.ClaveAntiguedad,
                item.IdAsesor,
                item.NombreAsesor,
                item.IdSupervisor,
                item.NombreSupervisor)).ToArray());
    }

    private static string ObtenerEtiquetaSituacion(string claveSituacion) =>
        claveSituacion switch
        {
            PromesasCarteraEstadoOperativo.PagoParcial => "Pago parcial",
            _ => "Sin pago registrado"
        };

    private static int ObtenerOrdenAntiguedad(string claveAntiguedad) =>
        claveAntiguedad switch
        {
            "1-3" => 1,
            "4-7" => 2,
            "8-mas" => 3,
            "sin-clasificar" => 4,
            _ => 5
        };

    private static DateTimeOffset? ConvertirOffsetUtc(DateTime? value)
    {
        if (!value.HasValue)
        {
            return null;
        }

        var utc = DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
        return new DateTimeOffset(utc);
    }
}
