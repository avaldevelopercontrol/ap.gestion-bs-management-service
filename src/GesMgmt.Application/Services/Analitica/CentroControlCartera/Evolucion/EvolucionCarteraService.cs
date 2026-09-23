using GesMgmt.Application.DTOs.Analitica.CentroControlCartera;
using GesMgmt.Application.Interfaces.Analitica.CentroControlCartera;
using GesMgmt.Application.Utils.Analitica.CentroControlCartera;
using GesMgmt.Domain.Entities.Analitica.CentroControlCartera;
using GesMgmt.Domain.Interfaces.Analitica.CentroControlCartera;

namespace GesMgmt.Application.Services.Analitica.CentroControlCartera;

public sealed class EvolucionCarteraService(
    IAccesoCentroControlCarteraService accessService,
    IEvolucionCarteraRepository repository) : IEvolucionCarteraService
{
    private const int CantidadMesesHistoricos = 6;

    public async Task<CarteraOperacionResult<EvolucionCarteraResponse>> ObtenerAsync(
        string? campana,
        string? idSubCartera,
        string? fechaDesde,
        string? fechaHasta,
        string? unidadNegocio,
        int? idClienteCrm,
        CancellationToken cancellationToken)
    {
        var preparation = await PrepararConsultaAsync(
            campana,
            idSubCartera,
            fechaDesde,
            fechaHasta,
            unidadNegocio,
            idClienteCrm,
            cancellationToken);

        if (preparation.Error is not null)
        {
            return preparation.Error.ToResult<EvolucionCarteraResponse>();
        }

        var query = preparation.Consulta!;
        var rows = await DiagnosticosCentroControlCartera.ObservarFaseAsync(
            DiagnosticosCentroControlCartera.FaseConsulta,
            () => repository.ObtenerEvolucionAsync(
                query.Contexto.ClaveCliente,
                query.Contexto.ClaveCampana,
                query.Solicitud.IdSubCartera,
                query.Solicitud.UnidadNegocio,
                query.Rango,
                cancellationToken));

        return CarteraOperacionResult<EvolucionCarteraResponse>.Exito(
            EvolucionCarteraResponseMapper.Map(
                query.Contexto,
                query.Rango,
                rows));
    }

    public async Task<CarteraOperacionResult<EvolucionCarteraComparativaResponse>> ObtenerComparativaAsync(
        string? campana,
        string? idSubCartera,
        string? fechaDesde,
        string? fechaHasta,
        string? unidadNegocio,
        int? idClienteCrm,
        CancellationToken cancellationToken)
    {
        var preparation = await PrepararConsultaAsync(
            campana,
            idSubCartera,
            fechaDesde,
            fechaHasta,
            unidadNegocio,
            idClienteCrm,
            cancellationToken);

        if (preparation.Error is not null)
        {
            return preparation.Error.ToResult<EvolucionCarteraComparativaResponse>();
        }

        var query = preparation.Consulta!;
        var historico = await DiagnosticosCentroControlCartera.ObservarFaseAsync(
            DiagnosticosCentroControlCartera.FaseConsulta,
            () => repository.ObtenerHistoricoComparableAsync(
                query.Contexto,
                query.Rango,
                query.Solicitud.IdSubCartera,
                query.Solicitud.UnidadNegocio,
                CantidadMesesHistoricos,
                cancellationToken));

        return CarteraOperacionResult<EvolucionCarteraComparativaResponse>.Exito(
            EvolucionCarteraComparativaResponseMapper.Map(
                query.Contexto,
                query.Rango,
                historico));
    }

    private async Task<PreparacionResultado> PrepararConsultaAsync(
        string? campana,
        string? idSubCartera,
        string? fechaDesde,
        string? fechaHasta,
        string? unidadNegocio,
        int? idClienteCrm,
        CancellationToken cancellationToken)
    {
        if (!EvolucionCarteraRequest.IntentarCrear(
                campana,
                idSubCartera,
                fechaDesde,
                fechaHasta,
                out var request,
                out var requestErrors))
        {
            return PreparacionResultado.DesdeValidacion(requestErrors);
        }

        if (!UnidadNegocioCarteraContrato.IntentarNormalizarSolicitado(
                unidadNegocio,
                out var normalizedBusinessUnit,
                out var businessUnitErrors))
        {
            return PreparacionResultado.DesdeValidacion(businessUnitErrors);
        }

        var clientAccess = await accessService.ResolverClienteAsync(
            idClienteCrm,
            cancellationToken);

        if (!clientAccess.EstaPermitido)
        {
            return PreparacionResultado.DesdeProblema(
                clientAccess.CodigoEstadoError ?? 403,
                clientAccess.TituloError ?? "Acceso Analítica denegado",
                clientAccess.DetalleError
                    ?? "No se pudo validar el acceso a Centro de Control de Cartera.");
        }

        var validRequest = request! with
        {
            UnidadNegocio = UnidadNegocioCarteraPolicy.ResolverSolicitadoOPredeterminado(
                clientAccess.IdClienteCrm!.Value,
                normalizedBusinessUnit)
        };

        var context = await DiagnosticosCentroControlCartera.ObservarFaseAsync(
            DiagnosticosCentroControlCartera.FaseContexto,
            () => repository.ResolverContextoAsync(
                clientAccess.IdClienteCrm.Value,
                validRequest.Campana,
                validRequest.IdSubCartera,
                validRequest.UnidadNegocio,
                cancellationToken));

        if (context is null)
        {
            return PreparacionResultado.DesdeProblema(
                404,
                "Campaña Analítica no encontrada",
                ConstruirDetalleCampanaNoEncontrada(validRequest));
        }

        if (!validRequest.IntentarResolverRango(
                context,
                out var range,
                out var rangeErrors))
        {
            return PreparacionResultado.DesdeValidacion(rangeErrors);
        }

        return PreparacionResultado.Exito(
            new ConsultaPreparada(validRequest, context, range));
    }

    private static string ConstruirDetalleCampanaNoEncontrada(
        EvolucionCarteraRequest request) =>
        request.IdSubCartera is not null
            ? request.Campana is null
                ? $"No existe una campaña disponible para la subcartera Analítica '{request.IdSubCartera}' dentro del cliente autorizado."
                : $"No existe la campaña '{request.Campana}' asociada a la subcartera Analítica '{request.IdSubCartera}' dentro del cliente autorizado."
            : request.Campana is null
                ? "No existe una campaña disponible para el cliente autorizado."
                : $"No existe la campaña '{request.Campana}' para el cliente autorizado.";

    private sealed record ConsultaPreparada(
        EvolucionCarteraRequest Solicitud,
        EvolucionCarteraContexto Contexto,
        RangoEvolucionCartera Rango);

    private sealed record PreparacionResultado(
        ConsultaPreparada? Consulta,
        PreparacionError? Error)
    {
        public static PreparacionResultado Exito(ConsultaPreparada consulta) =>
            new(consulta, null);

        public static PreparacionResultado DesdeValidacion(
            IReadOnlyDictionary<string, string[]> errors) =>
            new(null, new PreparacionError(
                400,
                "Solicitud inválida",
                "Uno o más parámetros no son válidos.",
                errors));

        public static PreparacionResultado DesdeProblema(
            int statusCode,
            string title,
            string detail) =>
            new(null, new PreparacionError(
                statusCode,
                title,
                detail,
                null));
    }

    private sealed record PreparacionError(
        int CodigoEstado,
        string Titulo,
        string Detalle,
        IReadOnlyDictionary<string, string[]>? Validaciones)
    {
        public CarteraOperacionResult<T> ToResult<T>() =>
            Validaciones is not null
                ? CarteraOperacionResult<T>.Validacion(Validaciones)
                : CarteraOperacionResult<T>.Problema(
                    CodigoEstado,
                    Titulo,
                    Detalle);
    }
}
