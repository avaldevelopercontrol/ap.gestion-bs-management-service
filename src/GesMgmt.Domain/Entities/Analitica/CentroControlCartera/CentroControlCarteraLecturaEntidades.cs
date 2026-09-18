namespace GesMgmt.Domain.Entities.Analitica.CentroControlCartera;

public sealed class DimensionCampanaAnalitica
{
    public int ClaveCampana { get; set; }
    public int ClaveCliente { get; set; }
    public string CodigoCampana { get; set; } = string.Empty;
    public string NombreCampana { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
}

public sealed class ResumenDiarioCampanaAnalitica
{
    public int ClaveCliente { get; set; }
    public int ClaveCampana { get; set; }
    public DateTime FechaCalendario { get; set; }
    public DateTime? FechaCarga { get; set; }
}

public sealed class EvolucionDiariaCampanaAnalitica
{
    public int ClaveCliente { get; set; }
    public int ClaveCampana { get; set; }
    public DateTime FechaCalendario { get; set; }
    public int? ClientesAsignados { get; set; }
    public int? ClientesGestionados { get; set; }
    public int? ClientesPendientes { get; set; }
    public int? ClientesContactados { get; set; }
    public decimal? MontoRecuperadoAcumulado { get; set; }
    public DateTime? FechaCarga { get; set; }
}

public sealed class EvolucionDiariaCarteraAnalitica
{
    public int ClaveCliente { get; set; }
    public int ClaveCampana { get; set; }
    public long ClaveCartera { get; set; }
    public DateTime FechaCalendario { get; set; }
    public int? ClientesAsignados { get; set; }
    public int? ClientesGestionados { get; set; }
    public int? ClientesPendientes { get; set; }
    public decimal? MontoRecuperadoAcumulado { get; set; }
    public DateTime? FechaCarga { get; set; }
}

public sealed class HechoDiarioCarteraAnalitica
{
    public int ClaveCliente { get; set; }
    public int ClaveCampana { get; set; }
    public long ClaveCartera { get; set; }
    public int ClaveFecha { get; set; }
    public int? ClientesAsignadosCorte { get; set; }
    public int? ClientesGestionadosCorte { get; set; }
    public int? ClientesPendientesCorte { get; set; }
    public int? ClientesContactadosCorte { get; set; }
    public bool TieneCorteOrigen { get; set; }
    public DateTime? FechaCarga { get; set; }
}

public sealed class HechoEvolucionDiariaCarteraAnalitica
{
    public int ClaveCliente { get; set; }
    public int ClaveCampana { get; set; }
    public long ClaveCartera { get; set; }
    public int ClaveFecha { get; set; }
    public int? ClientesAsignados { get; set; }
    public int? ClientesGestionados { get; set; }
    public int? ClientesPendientes { get; set; }
    public DateTime? FechaCarga { get; set; }
}

public sealed class DimensionCarteraAnalitica
{
    public long ClaveCartera { get; set; }
    public int? IdCarteraOrigen { get; set; }
    public string NombreCartera { get; set; } = string.Empty;
    public string? UnidadNegocioOrigen { get; set; }
}

public sealed class DimensionFechaAnalitica
{
    public int ClaveFecha { get; set; }
    public DateTime FechaCalendario { get; set; }
}

public sealed class AtribucionDiariaSupervisorAsesorAnalitica
{
    public int ClaveCliente { get; set; }
    public int ClaveCampana { get; set; }
    public long ClaveCartera { get; set; }
    public int ClaveAsesor { get; set; }
    public string NombreAsesor { get; set; } = string.Empty;
    public int? ClaveSupervisor { get; set; }
    public string? NombreSupervisor { get; set; }
    public DateTime FechaCalendario { get; set; }
    public int? EventosGestion { get; set; }
    public decimal? MontoRecuperado { get; set; }
    public DateTime? FechaCarga { get; set; }
}

public sealed class ContactoDiarioDeudorSupervisorAnalitica
{
    public int ClaveCliente { get; set; }
    public int ClaveCampana { get; set; }
    public long ClaveCartera { get; set; }
    public long IdDeudorOrigen { get; set; }
    public int? ClaveAsesor { get; set; }
    public int? ClaveSupervisor { get; set; }
    public DateTime FechaCalendario { get; set; }
    public bool TuvoContactoDirecto { get; set; }
    public bool TuvoContactoIndirecto { get; set; }
    public bool TuvoSinContacto { get; set; }
    public DateTime? FechaCarga { get; set; }
}

public sealed class PromesaOperativaSupervisorAnalitica
{
    public long ClaveHechoPromesa { get; set; }
    public long ClaveSupervisorAsesor { get; set; }
    public int ClaveCliente { get; set; }
    public int ClaveCampana { get; set; }
    public long ClaveCartera { get; set; }
    public long IdDeudorOrigen { get; set; }
    public DateTime? FechaVencimientoPromesa { get; set; }
    public int? ClaveAsesor { get; set; }
    public string? NombreAsesor { get; set; }
    public int? ClaveSupervisor { get; set; }
    public string? NombreSupervisor { get; set; }
    public bool EsPromesaValida { get; set; }
    public DateTime FechaGestion { get; set; }
    public string CodigoEstado { get; set; } = string.Empty;
    public decimal? MontoPagado { get; set; }
    public decimal? MontoPromesa { get; set; }
    public DateTime? FechaCarga { get; set; }
}

public sealed class PagoDiarioDeudorSupervisorAnalitica
{
    public int ClaveCliente { get; set; }
    public int ClaveCampana { get; set; }
    public long ClaveCartera { get; set; }
    public long IdDeudorOrigen { get; set; }
    public int? ClaveAsesor { get; set; }
    public int? ClaveSupervisor { get; set; }
    public DateTime FechaCalendario { get; set; }
    public DateTime? FechaCarga { get; set; }
}

public sealed class SupervisorActualAsesorAnalitica
{
    public int ClaveCliente { get; set; }
    public int ClaveAsesor { get; set; }
    public int? ClaveSupervisor { get; set; }
    public string? NombreSupervisor { get; set; }
}

public sealed class EstadoResumenDiarioCarteraAnalitica
{
    public int ClaveCliente { get; set; }
    public int ClaveCampana { get; set; }
    public long ClaveCartera { get; set; }
    public DateTime FechaCalendario { get; set; }
}

public sealed class MetricaDiariaCarteraAnalitica
{
    public int ClaveCliente { get; set; }
    public int ClaveCampana { get; set; }
    public long ClaveCartera { get; set; }
    public DateTime FechaCalendario { get; set; }
    public bool TieneCorteOrigen { get; set; }
    public int? ClientesAsignadosCorte { get; set; }
    public int? ClientesGestionadosCorte { get; set; }
    public int? ClientesPendientesCorte { get; set; }
    public int? ClientesContactadosCorte { get; set; }
    public int? EventosGestionDia { get; set; }
    public decimal? MontoRecuperadoDia { get; set; }
    public DateTime? FechaCarga { get; set; }
}

public sealed class HechoContactoDiarioDeudorAnalitica
{
    public int ClaveCliente { get; set; }
    public int ClaveCampana { get; set; }
    public long ClaveCartera { get; set; }
    public long IdDeudorOrigen { get; set; }
    public int ClaveFecha { get; set; }
    public bool TuvoContactoDirecto { get; set; }
    public bool TuvoContactoIndirecto { get; set; }
    public bool TuvoSinContacto { get; set; }
    public DateTime? FechaCarga { get; set; }
}

public sealed class HechoPromesaAnalitica
{
    public int ClaveCliente { get; set; }
    public int ClaveCampana { get; set; }
    public long ClaveCartera { get; set; }
    public long IdDeudorOrigen { get; set; }
    public bool EsPromesaValida { get; set; }
    public DateTime FechaGestion { get; set; }
    public string CodigoEstado { get; set; } = string.Empty;
    public decimal? MontoPagado { get; set; }
    public decimal? MontoPromesa { get; set; }
    public DateTime? FechaCarga { get; set; }
}

public sealed class HechoPagoDiarioDeudorAnalitica
{
    public int ClaveCliente { get; set; }
    public int ClaveCampana { get; set; }
    public long ClaveCartera { get; set; }
    public long IdDeudorOrigen { get; set; }
    public int ClaveFecha { get; set; }
    public DateTime? FechaCarga { get; set; }
}

public sealed class ControlCargaAnalitica
{
    public string CodigoOrigen { get; set; } = string.Empty;
    public DateTime? FechaHoraUltimoOrigen { get; set; }
    public DateTime? FechaUltimoExito { get; set; }
}


public sealed class HechoMetaMensualAnalitica
{
    public int ClaveCliente { get; set; }
    public int ClaveCampana { get; set; }
    public long? ClaveCartera { get; set; }
    public decimal? MontoMetaRecuperacion { get; set; }
    public DateTime? FechaCorteOrigen { get; set; }
}

public sealed class AvanceMetaCampanaAnalitica
{
    public int ClaveCliente { get; set; }
    public int ClaveCampana { get; set; }
    public DateTime FechaCalendario { get; set; }
    public decimal? MontoMetaRecuperacion { get; set; }
    public decimal? MontoEsperadoAcumulado { get; set; }
    public DateTime? FechaCorteMetaOrigen { get; set; }
}

public sealed class PromesaOperativaAnalitica
{
    public long ClaveHechoPromesa { get; set; }
    public int ClaveCliente { get; set; }
    public int ClaveCampana { get; set; }
    public long ClaveCartera { get; set; }
    public long IdDeudorOrigen { get; set; }
    public DateTime? FechaVencimientoPromesa { get; set; }
    public DateTime? FechaUltimoPago { get; set; }
    public int? ClaveAsesor { get; set; }
    public bool EsPromesaValida { get; set; }
    public string CodigoEstado { get; set; } = string.Empty;
    public bool VenceHoy { get; set; }
    public bool EstaRota { get; set; }
    public bool EstaCumplidaOParcial { get; set; }
    public decimal? MontoPromesa { get; set; }
    public decimal? MontoPagado { get; set; }
    public DateTime? FechaCarga { get; set; }
}
