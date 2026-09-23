using System.Globalization;
using System.Text.RegularExpressions;

namespace GesMgmt.Domain.Entities.Analitica.CentroControlCartera;

public sealed record PromesasVencidasCarteraMetadatosDbFila
{
    public required string TipoFila { get; init; }
    public string? ClaveAntiguedad { get; init; }
    public int? IdAsesor { get; init; }
    public string? NombreAsesor { get; init; }
    public int? IdSupervisor { get; init; }
    public string? NombreSupervisor { get; init; }
    public long CantidadPromesas { get; init; }
    public decimal MontoPromesa { get; init; }
    public decimal MontoPendiente { get; init; }
    public DateTime? FechaCorte { get; init; }
    public DateTime? FechaActualizacionUtc { get; init; }
}
public sealed record PromesasVencidasCarteraResumenDbFila
{
    public long CantidadVencidas { get; init; }
    public decimal MontoVencido { get; init; }
    public decimal MontoPendiente { get; init; }
    public DateTime? FechaCorte { get; init; }
    public DateTime? FechaActualizacionUtc { get; init; }
}
public sealed record PromesasVencidasCarteraAntiguedadDbFila
{
    public required string ClaveAntiguedad { get; init; }
    public long CantidadPromesas { get; init; }
    public decimal MontoPromesa { get; init; }
    public decimal MontoPendiente { get; init; }
}
public sealed record PromesaVencidaCarteraDbFila
{
    public long IdPromesa { get; init; }
    public long IdDeudor { get; init; }
    public string? NombreDeudor { get; init; }
    public DateTime? FechaVencimiento { get; init; }
    public int? DiasVencimiento { get; init; }
    public decimal MontoPromesa { get; init; }
    public decimal MontoPagado { get; init; }
    public decimal MontoPendiente { get; init; }
    public required string ClaveSituacion { get; init; }
    public int? IdAsesor { get; init; }
    public string? NombreAsesor { get; init; }
    public int? IdSupervisor { get; init; }
    public string? NombreSupervisor { get; init; }
    public required string ClaveAntiguedad { get; init; }
    public DateTime? FechaCorte { get; init; }
    public DateTime? FechaActualizacionUtc { get; init; }
}
public sealed record CarteraVencidoAsesorFiltroDbFila
{
    public int IdAsesor { get; init; }
    public required string NombreAsesor { get; init; }
}
public sealed record CarteraVencidoSupervisorFiltroDbFila
{
    public int IdSupervisor { get; init; }
    public required string NombreSupervisor { get; init; }
}
public sealed record PromesasVencidasCarteraConsultaResult(
    PromesasVencidasCarteraResumenDbFila Resumen,
    IReadOnlyList<PromesasVencidasCarteraAntiguedadDbFila> Antiguedad,
    IReadOnlyList<PromesaVencidaCarteraDbFila> Elementos,
    IReadOnlyList<CarteraVencidoAsesorFiltroDbFila> Asesores,
    IReadOnlyList<CarteraVencidoSupervisorFiltroDbFila> Supervisores,
    PromesasCarteraPaginacion Paginacion);
