using GesMgmt.Domain.Entities.Analitica.CentroControlCartera;
using GesMgmt.Domain.Interfaces.Analitica.CentroControlCartera;
using GesMgmt.Infraestructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GesMgmt.Infraestructure.Repositories.Analitica.CentroControlCartera;

internal sealed class SeguimientoPromesasCarteraRepository(
    AnaliticaDbContext analiticaContext,
    AvalDbContext avalContext)
    : ISeguimientoPromesasCarteraRepository
{
    private const int TipoGestionTelefonica = 1;
    private const int TipoContactoCompromisoPago = 2;

    public async Task<SeguimientoPromesasCarteraConsultaResult> ObtenerAsync(
        int idClienteCrm,
        int claveCliente,
        int claveCampana,
        long? idSubCartera,
        string? unidadNegocio,
        DateOnly fechaVencimiento,
        int pagina,
        int tamanoPagina,
        string? estado,
        string ordenarPor,
        string direccionOrden,
        CancellationToken cancellationToken)
    {
        var fechaDesde = fechaVencimiento.ToDateTime(TimeOnly.MinValue);
        var fechaHastaExclusiva = fechaDesde.AddDays(1);

        var baseQuery = PromesaCarteraDetallesEfConsulta.AplicarAlcance(
            analiticaContext,
            analiticaContext.PromesaOperativaAnalitica
                .AsNoTracking()
                .Where(row =>
                    row.ClaveCliente == claveCliente
                    && row.ClaveCampana == claveCampana
                    && row.EsPromesaValida
                    && row.FechaVencimientoPromesa >= fechaDesde
                    && row.FechaVencimientoPromesa < fechaHastaExclusiva),
            idSubCartera,
            unidadNegocio);

        var metricQuery = baseQuery.Select(row => new SeguimientoMetricProjection
        {
            FechaVencimiento = row.FechaVencimientoPromesa,
            MontoPromesa = row.MontoPromesa ?? 0m,
            MontoPagado = row.MontoPagado ?? 0m,
            MontoPendiente = (row.MontoPromesa ?? 0m) > (row.MontoPagado ?? 0m)
                ? (row.MontoPromesa ?? 0m) - (row.MontoPagado ?? 0m)
                : 0m,
            ClaveEstado = row.CodigoEstado == "BROKEN"
                ? "incumplida"
                : row.CodigoEstado == "FULFILLED_OUT_OF_RANGE"
                    ? "pagada-fuera-plazo"
                    : row.CodigoEstado == "FULFILLED"
                        ? "cumplida"
                        : row.CodigoEstado == "PARTIAL"
                            ? "parcial"
                            : (row.MontoPagado ?? 0m) <= 0m
                                ? "pendiente"
                                : (row.MontoPagado ?? 0m) < (row.MontoPromesa ?? 0m)
                                    ? "parcial"
                                    : "cumplida",
            FechaActualizacionUtc = row.FechaCarga
        });

        var summaryData = await metricQuery
            .GroupBy(_ => 1)
            .Select(group => new
            {
                CantidadPromesas = group.LongCount(),
                MontoPromesa = group.Sum(row => row.MontoPromesa),
                MontoPagado = group.Sum(row => row.MontoPagado),
                MontoPendiente = group.Sum(row => row.MontoPendiente),
                FechaCorte = group.Max(row => row.FechaVencimiento),
                FechaActualizacionUtc = group.Max(row => row.FechaActualizacionUtc)
            })
            .SingleOrDefaultAsync(cancellationToken);

        var resumen = summaryData is null
            ? new SeguimientoPromesasCarteraResumenDbFila()
            : new SeguimientoPromesasCarteraResumenDbFila
            {
                CantidadPromesas = summaryData.CantidadPromesas,
                MontoPromesa = PromesaCarteraDetallesEfConsulta.RedondearMonto(summaryData.MontoPromesa),
                MontoPagado = PromesaCarteraDetallesEfConsulta.RedondearMonto(summaryData.MontoPagado),
                MontoPendiente = PromesaCarteraDetallesEfConsulta.RedondearMonto(summaryData.MontoPendiente),
                FechaCorte = summaryData.FechaCorte,
                FechaActualizacionUtc = summaryData.FechaActualizacionUtc
            };

        var statusRows = await metricQuery
            .GroupBy(row => row.ClaveEstado)
            .Select(group => new
            {
                ClaveEstado = group.Key,
                CantidadPromesas = group.LongCount(),
                MontoPromesa = group.Sum(row => row.MontoPromesa),
                MontoPagado = group.Sum(row => row.MontoPagado),
                MontoPendiente = group.Sum(row => row.MontoPendiente)
            })
            .ToListAsync(cancellationToken);

        var statusBuckets = statusRows
            .Select(row => new SeguimientoPromesasCarteraEstadoDbFila
            {
                ClaveEstado = row.ClaveEstado,
                CantidadPromesas = row.CantidadPromesas,
                MontoPromesa = PromesaCarteraDetallesEfConsulta.RedondearMonto(row.MontoPromesa),
                MontoPagado = PromesaCarteraDetallesEfConsulta.RedondearMonto(row.MontoPagado),
                MontoPendiente = PromesaCarteraDetallesEfConsulta.RedondearMonto(row.MontoPendiente)
            })
            .ToArray();

        var filteredCount = estado is null
            ? resumen.CantidadPromesas
            : await metricQuery.LongCountAsync(
                row => row.ClaveEstado == estado,
                cancellationToken);

        var itemQuery = baseQuery.Select(row => new SeguimientoItemProjection
        {
            IdPromesa = row.ClaveHechoPromesa,
            ClaveCartera = row.ClaveCartera,
            IdDeudor = row.IdDeudorOrigen,
            FechaVencimiento = row.FechaVencimientoPromesa,
            MontoPromesa = row.MontoPromesa ?? 0m,
            MontoPagado = row.MontoPagado ?? 0m,
            MontoPendiente = (row.MontoPromesa ?? 0m) > (row.MontoPagado ?? 0m)
                ? (row.MontoPromesa ?? 0m) - (row.MontoPagado ?? 0m)
                : 0m,
            FechaUltimoPago = row.FechaUltimoPago,
            ClaveEstado = row.CodigoEstado == "BROKEN"
                ? "incumplida"
                : row.CodigoEstado == "FULFILLED_OUT_OF_RANGE"
                    ? "pagada-fuera-plazo"
                    : row.CodigoEstado == "FULFILLED"
                        ? "cumplida"
                        : row.CodigoEstado == "PARTIAL"
                            ? "parcial"
                            : (row.MontoPagado ?? 0m) <= 0m
                                ? "pendiente"
                                : (row.MontoPagado ?? 0m) < (row.MontoPromesa ?? 0m)
                                    ? "parcial"
                                    : "cumplida",
            EtiquetaEstado = row.CodigoEstado == "BROKEN"
                ? "Incumplida"
                : row.CodigoEstado == "FULFILLED_OUT_OF_RANGE"
                    ? "Pagada fuera de plazo"
                    : row.CodigoEstado == "FULFILLED"
                        ? "Cumplida"
                        : row.CodigoEstado == "PARTIAL"
                            ? "Pago parcial"
                            : (row.MontoPagado ?? 0m) <= 0m
                                ? "Pendiente"
                                : (row.MontoPagado ?? 0m) < (row.MontoPromesa ?? 0m)
                                    ? "Pago parcial"
                                    : "Cumplida",
            IdAsesor = row.ClaveAsesor,
            NombreAsesor = analiticaContext.PromesaOperativaSupervisorAnalitica
                .AsNoTracking()
                .Where(attribution =>
                    attribution.ClaveCliente == row.ClaveCliente
                    && attribution.ClaveCampana == row.ClaveCampana
                    && attribution.ClaveHechoPromesa == row.ClaveHechoPromesa)
                .OrderBy(attribution => attribution.ClaveSupervisor == null ? 1 : 0)
                .ThenByDescending(attribution => attribution.ClaveSupervisorAsesor)
                .ThenByDescending(attribution => attribution.FechaCarga)
                .Select(attribution => attribution.NombreAsesor == null
                    || attribution.NombreAsesor.Trim() == string.Empty
                        ? null
                        : attribution.NombreAsesor.Trim())
                .FirstOrDefault(),
            IdSupervisor = analiticaContext.PromesaOperativaSupervisorAnalitica
                .AsNoTracking()
                .Where(attribution =>
                    attribution.ClaveCliente == row.ClaveCliente
                    && attribution.ClaveCampana == row.ClaveCampana
                    && attribution.ClaveHechoPromesa == row.ClaveHechoPromesa)
                .OrderBy(attribution => attribution.ClaveSupervisor == null ? 1 : 0)
                .ThenByDescending(attribution => attribution.ClaveSupervisorAsesor)
                .ThenByDescending(attribution => attribution.FechaCarga)
                .Select(attribution => attribution.ClaveSupervisor)
                .FirstOrDefault(),
            NombreSupervisor = analiticaContext.PromesaOperativaSupervisorAnalitica
                .AsNoTracking()
                .Where(attribution =>
                    attribution.ClaveCliente == row.ClaveCliente
                    && attribution.ClaveCampana == row.ClaveCampana
                    && attribution.ClaveHechoPromesa == row.ClaveHechoPromesa)
                .OrderBy(attribution => attribution.ClaveSupervisor == null ? 1 : 0)
                .ThenByDescending(attribution => attribution.ClaveSupervisorAsesor)
                .ThenByDescending(attribution => attribution.FechaCarga)
                .Select(attribution => attribution.NombreSupervisor == null
                    || attribution.NombreSupervisor.Trim() == string.Empty
                        ? null
                        : attribution.NombreSupervisor.Trim())
                .FirstOrDefault(),
            FechaActualizacionUtc = row.FechaCarga
        });

        if (estado is not null)
        {
            itemQuery = itemQuery.Where(row => row.ClaveEstado == estado);
        }

        var orderedItems = AplicarOrden(itemQuery, ordenarPor, direccionOrden)
            .ThenBy(row => row.IdPromesa);

        var itemRows = await orderedItems
            .Skip(PromesaCarteraDetallesEfConsulta.ObtenerOffset(pagina, tamanoPagina))
            .Take(tamanoPagina)
            .ToListAsync(cancellationToken);

        var elementos = await EnriquecerElementosAsync(
            itemRows,
            idClienteCrm,
            claveCliente,
            claveCampana,
            fechaDesde,
            fechaHastaExclusiva,
            cancellationToken);

        return new SeguimientoPromesasCarteraConsultaResult(
            fechaVencimiento,
            resumen,
            statusBuckets,
            elementos,
            PromesasCarteraPaginacion.Crear(pagina, tamanoPagina, filteredCount));
    }

    private async Task<IReadOnlyList<SeguimientoPromesaCarteraDbFila>> EnriquecerElementosAsync(
        IReadOnlyList<SeguimientoItemProjection> itemRows,
        int idClienteCrm,
        int claveCliente,
        int claveCampana,
        DateTime fechaDesde,
        DateTime fechaHastaExclusiva,
        CancellationToken cancellationToken)
    {
        if (itemRows.Count == 0)
        {
            return [];
        }

        var clavesCartera = itemRows
            .Select(row => row.ClaveCartera)
            .Distinct()
            .ToArray();

        var carterasOrigen = await analiticaContext.CarterasAnalitica
            .AsNoTracking()
            .Where(row => clavesCartera.Contains(row.ClaveCartera))
            .Select(row => new
            {
                row.ClaveCartera,
                row.IdCarteraOrigen
            })
            .ToListAsync(cancellationToken);

        var idCarteraOrigenPorClave = carterasOrigen
            .Where(row => row.IdCarteraOrigen.HasValue)
            .ToDictionary(row => row.ClaveCartera, row => row.IdCarteraOrigen!.Value);

        var idsDeudorOrigen = itemRows
            .Select(row => row.IdDeudor)
            .Where(id => id is > 0 and <= int.MaxValue)
            .Select(id => (int)id)
            .Distinct()
            .ToArray();

        var nombresPorDeudor = await PromesasCarteraDeudorEfConsulta.ObtenerNombresAsync(
            avalContext,
            itemRows.Select(row => row.IdDeudor),
            cancellationToken);

        var contactosPorDeudor = await ObtenerContactosAsync(
            itemRows,
            claveCliente,
            claveCampana,
            fechaDesde,
            fechaHastaExclusiva,
            cancellationToken);

        var actividadPorDeudor = await ObtenerActividadOperativaAsync(
            idClienteCrm,
            idsDeudorOrigen,
            idCarteraOrigenPorClave.Values.Distinct().ToArray(),
            fechaDesde,
            fechaHastaExclusiva,
            cancellationToken);

        return itemRows
            .Select(row =>
            {
                var claveContacto = contactosPorDeudor.GetValueOrDefault(
                    (row.ClaveCartera, row.IdDeudor),
                    "sin-gestion");

                ActividadOperativa? actividad = null;
                if (row.IdDeudor is > 0 and <= int.MaxValue
                    && idCarteraOrigenPorClave.TryGetValue(row.ClaveCartera, out var idCarteraOrigen))
                {
                    actividadPorDeudor.TryGetValue(
                        ((int)row.IdDeudor, idCarteraOrigen),
                        out actividad);
                }

                var cantidadGestiones = actividad?.CantidadGestiones ?? 0;

                bool? confirmoPago;
                if (actividad?.ConfirmoPago == true)
                {
                    confirmoPago = true;
                }
                else if (claveContacto == "directo")
                {
                    confirmoPago = false;
                }
                else
                {
                    confirmoPago = null;
                }

                return new SeguimientoPromesaCarteraDbFila
                {
                    IdPromesa = row.IdPromesa,
                    IdDeudor = row.IdDeudor,
                    NombreDeudor = nombresPorDeudor.GetValueOrDefault(row.IdDeudor),
                    FechaVencimiento = row.FechaVencimiento,
                    MontoPromesa = PromesaCarteraDetallesEfConsulta.RedondearMonto(row.MontoPromesa),
                    MontoPagado = PromesaCarteraDetallesEfConsulta.RedondearMonto(row.MontoPagado),
                    MontoPendiente = PromesaCarteraDetallesEfConsulta.RedondearMonto(row.MontoPendiente),
                    FechaUltimoPago = row.FechaUltimoPago,
                    ClaveEstado = row.ClaveEstado,
                    Gestionado = cantidadGestiones > 0,
                    CantidadGestiones = cantidadGestiones,
                    CantidadLlamadas = actividad?.CantidadLlamadas ?? 0,
                    ClaveContacto = claveContacto,
                    ConfirmoPago = confirmoPago,
                    FechaUltimaGestion = actividad?.FechaUltimaGestion,
                    IdAsesor = row.IdAsesor,
                    NombreAsesor = PromesaCarteraDetallesEfConsulta.NormalizarNombre(row.NombreAsesor),
                    IdSupervisor = row.IdSupervisor,
                    NombreSupervisor = PromesaCarteraDetallesEfConsulta.NormalizarNombre(row.NombreSupervisor),
                    FechaActualizacionUtc = row.FechaActualizacionUtc
                };
            })
            .ToArray();
    }

    private async Task<IReadOnlyDictionary<(long ClaveCartera, long IdDeudor), string>> ObtenerContactosAsync(
        IReadOnlyCollection<SeguimientoItemProjection> itemRows,
        int claveCliente,
        int claveCampana,
        DateTime fechaDesde,
        DateTime fechaHastaExclusiva,
        CancellationToken cancellationToken)
    {
        var clavesCartera = itemRows.Select(row => row.ClaveCartera).Distinct().ToArray();
        var idsDeudor = itemRows.Select(row => row.IdDeudor).Distinct().ToArray();

        var rows = await analiticaContext.ContactoDiarioDeudorSupervisorAnalitica
            .AsNoTracking()
            .Where(row =>
                row.ClaveCliente == claveCliente
                && row.ClaveCampana == claveCampana
                && row.FechaCalendario >= fechaDesde
                && row.FechaCalendario < fechaHastaExclusiva
                && clavesCartera.Contains(row.ClaveCartera)
                && idsDeudor.Contains(row.IdDeudorOrigen))
            .Select(row => new
            {
                row.ClaveCartera,
                row.IdDeudorOrigen,
                row.TuvoContactoDirecto,
                row.TuvoContactoIndirecto,
                row.TuvoSinContacto
            })
            .ToListAsync(cancellationToken);

        return rows
            .GroupBy(row => (row.ClaveCartera, row.IdDeudorOrigen))
            .ToDictionary(
                group => group.Key,
                group => group.Any(row => row.TuvoContactoDirecto)
                    ? "directo"
                    : group.Any(row => row.TuvoContactoIndirecto)
                        ? "indirecto"
                        : group.Any(row => row.TuvoSinContacto)
                            ? "sin-contacto"
                            : "sin-gestion");
    }

    private async Task<IReadOnlyDictionary<(int IdDeudor, int IdCartera), ActividadOperativa>> ObtenerActividadOperativaAsync(
        int idClienteCrm,
        IReadOnlyCollection<int> idsDeudor,
        IReadOnlyCollection<int> idsCartera,
        DateTime fechaDesde,
        DateTime fechaHastaExclusiva,
        CancellationToken cancellationToken)
    {
        if (idsDeudor.Count == 0 || idsCartera.Count == 0)
        {
            return new Dictionary<(int IdDeudor, int IdCartera), ActividadOperativa>();
        }

        var operaciones = avalContext.av_DocxCobrarOpes
            .AsNoTracking()
            .Where(row =>
                row.nId_Cliente == idClienteCrm
                && row.bEstado == true
                && row.dDocCobOpe_FecIni >= fechaDesde
                && row.dDocCobOpe_FecIni < fechaHastaExclusiva
                && idsDeudor.Contains(row.nId_PersDeudor)
                && row.nId_Cartera.HasValue
                && idsCartera.Contains(row.nId_Cartera.Value));

        var resultados = avalContext.av_OpeCodCliOuts
            .AsNoTracking()
            .Where(row => row.nId_Cliente == idClienteCrm);

        var rows = await (
                from operacion in operaciones
                join resultado in resultados
                    on operacion.nId_OpeCodCliOut equals resultado.nId_OpeCodCliOut into catalogo
                from resultado in catalogo.DefaultIfEmpty()
                select new ActividadOperativaFila(
                    operacion.nId_PersDeudor,
                    operacion.nId_Cartera!.Value,
                    operacion.nId_TipoGestion,
                    operacion.dDocCobOpe_FecIni,
                    operacion.dFechCompromisoPago,
                    operacion.monto_comp,
                    resultado == null ? null : resultado.nId_TipoContacto))
            .ToListAsync(cancellationToken);

        return rows
            .GroupBy(row => (row.IdDeudor, row.IdCartera))
            .ToDictionary(
                group => group.Key,
                group => new ActividadOperativa(
                    group.LongCount(),
                    group.LongCount(row => row.IdTipoGestion == TipoGestionTelefonica),
                    group.Any(row =>
                        row.IdTipoContacto == TipoContactoCompromisoPago
                        && row.FechaCompromisoPago.HasValue
                        && (row.MontoCompromiso ?? 0m) > 0m),
                    group.Max(row => row.FechaGestion)));
    }

    private static IOrderedQueryable<SeguimientoItemProjection> AplicarOrden(
        IQueryable<SeguimientoItemProjection> query,
        string ordenarPor,
        string direccionOrden)
    {
        var ascending = string.Equals(direccionOrden, "asc", StringComparison.OrdinalIgnoreCase);

        return ordenarPor.ToLowerInvariant() switch
        {
            "debtorid" => ascending
                ? query.OrderBy(row => row.IdDeudor)
                : query.OrderByDescending(row => row.IdDeudor),
            "promiseamount" => ascending
                ? query.OrderBy(row => row.MontoPromesa)
                : query.OrderByDescending(row => row.MontoPromesa),
            "paidamount" => ascending
                ? query.OrderBy(row => row.MontoPagado)
                : query.OrderByDescending(row => row.MontoPagado),
            "statuslabel" => ascending
                ? query.OrderBy(row => row.EtiquetaEstado)
                : query.OrderByDescending(row => row.EtiquetaEstado),
            "lastpaymentdate" => ascending
                ? query.OrderBy(row => row.FechaUltimoPago)
                : query.OrderByDescending(row => row.FechaUltimoPago),
            "advisorname" => ascending
                ? query.OrderBy(row => row.NombreAsesor)
                : query.OrderByDescending(row => row.NombreAsesor),
            "supervisorname" => ascending
                ? query.OrderBy(row => row.NombreSupervisor)
                : query.OrderByDescending(row => row.NombreSupervisor),
            _ => ascending
                ? query.OrderBy(row => row.MontoPendiente)
                : query.OrderByDescending(row => row.MontoPendiente)
        };
    }

    private sealed class SeguimientoMetricProjection
    {
        public DateTime? FechaVencimiento { get; init; }
        public decimal MontoPromesa { get; init; }
        public decimal MontoPagado { get; init; }
        public decimal MontoPendiente { get; init; }
        public string ClaveEstado { get; init; } = string.Empty;
        public DateTime? FechaActualizacionUtc { get; init; }
    }

    private sealed class SeguimientoItemProjection
    {
        public long IdPromesa { get; init; }
        public long ClaveCartera { get; init; }
        public long IdDeudor { get; init; }
        public DateTime? FechaVencimiento { get; init; }
        public decimal MontoPromesa { get; init; }
        public decimal MontoPagado { get; init; }
        public decimal MontoPendiente { get; init; }
        public DateTime? FechaUltimoPago { get; init; }
        public string ClaveEstado { get; init; } = string.Empty;
        public string EtiquetaEstado { get; init; } = string.Empty;
        public int? IdAsesor { get; init; }
        public string? NombreAsesor { get; init; }
        public int? IdSupervisor { get; init; }
        public string? NombreSupervisor { get; init; }
        public DateTime? FechaActualizacionUtc { get; init; }
    }

    private sealed record ActividadOperativaFila(
        int IdDeudor,
        int IdCartera,
        int? IdTipoGestion,
        DateTime? FechaGestion,
        DateTime? FechaCompromisoPago,
        decimal? MontoCompromiso,
        int? IdTipoContacto);

    private sealed record ActividadOperativa(
        long CantidadGestiones,
        long CantidadLlamadas,
        bool ConfirmoPago,
        DateTime? FechaUltimaGestion);
}
