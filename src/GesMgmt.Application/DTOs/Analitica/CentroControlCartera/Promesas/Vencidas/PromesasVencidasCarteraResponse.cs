using GesMgmt.Domain.Entities.Analitica.CentroControlCartera;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GesMgmt.Application.DTOs.Analitica.CentroControlCartera;

public sealed record PromesasVencidasCarteraResumen(
    long CantidadVencidas,
    decimal MontoVencido,
    decimal MontoPendiente);
public sealed record PromesasVencidasCarteraAntiguedadRango(
    string Clave,
    string Etiqueta,
    long Cantidad,
    decimal MontoPromesa,
    decimal MontoPendiente);
public sealed record PromesaVencidaCarteraItem(
    long IdPromesa,
    long IdDeudor,
    string? NombreDeudor,
    DateOnly? FechaVencimiento,
    int? DiasVencimiento,
    decimal MontoPromesa,
    decimal MontoPagado,
    decimal MontoPendiente,
    string ClaveSituacion,
    string EtiquetaSituacion,
    string ClaveAntiguedad,
    int? IdAsesor,
    string? NombreAsesor,
    int? IdSupervisor,
    string? NombreSupervisor);
public sealed record PromesaVencidaCarteraFiltroOpcion(
    int Id,
    string Nombre);
public sealed record PromesaVencidaCarteraOpcionesFiltro(
    IReadOnlyList<PromesaVencidaCarteraFiltroOpcion> Asesores,
    IReadOnlyList<PromesaVencidaCarteraFiltroOpcion> Supervisores);
public sealed record PromesasVencidasCarteraResponse(
    PromesasCarteraCampana Campana,
    DateOnly? FechaCorte,
    DateTimeOffset? FechaActualizacion,
    PromesasVencidasCarteraResumen Resumen,
    IReadOnlyList<PromesasVencidasCarteraAntiguedadRango> Antiguedad,
    PromesaVencidaCarteraOpcionesFiltro Filtros,
    PromesasCarteraPaginacion Paginacion,
    IReadOnlyList<PromesaVencidaCarteraItem> Elementos);
