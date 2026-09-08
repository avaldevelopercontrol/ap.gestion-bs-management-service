using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Application.Utils.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;

namespace GesMgmt.Application.Services.Analytics.PortfolioControlCenter;

public sealed class PortfolioSummaryService(
    IPortfolioControlCenterAccessService accessService,
    IPortfolioSummaryRepository repository) : IPortfolioSummaryService
{
    public async Task<PortfolioOperationResult<PortfolioSummaryResponse>> GetAsync(
        string? campaign,
        string? subPortfolioId,
        string? dateFrom,
        string? dateTo,
        string? businessUnit,
        int? crmClientId,
        CancellationToken cancellationToken)
    {
        if (!PortfolioSummaryRequest.TryCreate(
                campaign,
                subPortfolioId,
                dateFrom,
                dateTo,
                out var request,
                out var requestErrors))
        {
            return PortfolioOperationResult<PortfolioSummaryResponse>.Validation(requestErrors);
        }

        if (!PortfolioBusinessUnitContract.TryNormalizeRequested(
                businessUnit,
                out var normalizedBusinessUnit,
                out var businessUnitErrors))
        {
            return PortfolioOperationResult<PortfolioSummaryResponse>.Validation(businessUnitErrors);
        }

        var validRequest = request!;

        var clientAccess = await accessService.ResolveClientAsync(
            crmClientId,
            cancellationToken);

        if (!clientAccess.IsAllowed)
        {
            return PortfolioOperationResult<PortfolioSummaryResponse>.FromAccess(clientAccess);
        }

        var effectiveCrmClientId = clientAccess.CrmClientId!.Value;
        validRequest = validRequest with
        {
            BusinessUnit = PortfolioBusinessUnitPolicy.ResolveRequestedOrDefault(
                effectiveCrmClientId,
                normalizedBusinessUnit)
        };

        var context = await PortfolioControlCenterDiagnostics.ObservePhaseAsync(
            PortfolioControlCenterDiagnostics.ContextPhase,
            () => repository.ResolveContextAsync(
                effectiveCrmClientId,
                validRequest.Campaign,
                validRequest.SubPortfolioId,
                validRequest.BusinessUnit,
                cancellationToken));

        if (context is null)
        {
            return PortfolioOperationResult<PortfolioSummaryResponse>.Problem(
                statusCode: 404,
                title: "Campaña Analytics no encontrada",
                detail: validRequest.SubPortfolioId is not null
                    ? validRequest.Campaign is null
                        ? $"No existe una campaña disponible para la subcartera Analytics '{validRequest.SubPortfolioId}' dentro del cliente autorizado."
                        : $"No existe la campaña '{validRequest.Campaign}' asociada a la subcartera Analytics '{validRequest.SubPortfolioId}' dentro del cliente autorizado."
                    : validRequest.Campaign is null
                        ? "No existe una campaña disponible para el cliente autorizado."
                        : $"No existe la campaña '{validRequest.Campaign}' para el cliente autorizado.");
        }

        if (!validRequest.TryResolveRange(
                context,
                out var range,
                out var rangeErrors))
        {
            return PortfolioOperationResult<PortfolioSummaryResponse>.Validation(rangeErrors);
        }

        var row = await PortfolioControlCenterDiagnostics.ObservePhaseAsync(
            PortfolioControlCenterDiagnostics.QueryPhase,
            () => repository.GetSummaryAsync(
                context.ClientKey,
                context.CampaignKey,
                validRequest.SubPortfolioId,
                validRequest.BusinessUnit,
                range,
                cancellationToken));

        if (!row.SnapshotDate.HasValue)
        {
            return PortfolioOperationResult<PortfolioSummaryResponse>.Problem(
                statusCode: 422,
                title: "Snapshot de cartera no disponible",
                detail:
                    "No existe un snapshot real de cartera dentro del rango solicitado. " +
                    "El resumen no fabricará assigned/managed/pending desde filas carry-forward.");
        }

        return PortfolioOperationResult<PortfolioSummaryResponse>.Success(
            PortfolioSummaryResponseMapper.Map(
                context,
                range,
                row));

    }
}
