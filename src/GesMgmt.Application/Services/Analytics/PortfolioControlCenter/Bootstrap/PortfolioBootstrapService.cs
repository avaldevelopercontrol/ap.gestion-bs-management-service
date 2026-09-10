using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Application.Utils.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;

namespace GesMgmt.Application.Services.Analytics.PortfolioControlCenter;

public sealed class PortfolioBootstrapService(
    IPortfolioControlCenterAccessService accessService,
    IPortfolioBootstrapRepository bootstrapRepository,
    IPortfolioOverviewRepository overviewRepository) : IPortfolioBootstrapService
{
    public async Task<PortfolioOperationResult<PortfolioBootstrapResponse>> GetAsync(
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
            return PortfolioOperationResult<PortfolioBootstrapResponse>.Validation(requestErrors);
        }

        if (!PortfolioBusinessUnitContract.TryNormalizeRequested(
                businessUnit,
                out var normalizedBusinessUnit,
                out var businessUnitErrors))
        {
            return PortfolioOperationResult<PortfolioBootstrapResponse>.Validation(businessUnitErrors);
        }

        var validRequest = request!;
        var clientAccess = await accessService.ResolveClientAsync(
            crmClientId,
            cancellationToken);

        if (!clientAccess.IsAllowed)
        {
            return PortfolioOperationResult<PortfolioBootstrapResponse>.FromAccess(clientAccess);
        }

        var effectiveCrmClientId = clientAccess.CrmClientId!.Value;
        validRequest = validRequest with
        {
            BusinessUnit = PortfolioBusinessUnitPolicy.ResolveRequestedOrDefault(
                effectiveCrmClientId,
                normalizedBusinessUnit)
        };
        var source = await PortfolioControlCenterDiagnostics.ObservePhaseAsync(
            PortfolioControlCenterDiagnostics.ContextPhase,
            () => bootstrapRepository.ResolveAsync(
                effectiveCrmClientId,
                validRequest.Campaign,
                validRequest.SubPortfolioId,
                validRequest.BusinessUnit,
                cancellationToken));

        if (source is null)
        {
            return PortfolioOperationResult<PortfolioBootstrapResponse>.Problem(
                statusCode: 404,
                title: "Cliente Analytics no encontrado",
                detail: "No existe un cliente Analytics para el scope autorizado.");
        }

        if (!PortfolioBusinessUnitContract.TryResolve(
                normalizedBusinessUnit,
                PortfolioBusinessUnitPolicy.GetBackwardCompatibleDefault(
                    effectiveCrmClientId),
                source.FilterOptions.BusinessUnits,
                out var businessUnitSelection,
                out var selectionErrors))
        {
            return PortfolioOperationResult<PortfolioBootstrapResponse>.Validation(selectionErrors);
        }

        validRequest = validRequest with
        {
            BusinessUnit = businessUnitSelection.SelectedBusinessUnit
        };

        var filterOptions = PortfolioFilterOptionsResponseMapper.Map(
            source.FilterOptions,
            effectiveCrmClientId,
            validRequest.BusinessUnit);

        var context = source.OverviewContext;
        if (context is null)
        {
            if (validRequest.Campaign is not null
                || validRequest.SubPortfolioId is not null)
            {
                return CampaignNotFound(validRequest);
            }

            return PortfolioOperationResult<PortfolioBootstrapResponse>.Success(
                new PortfolioBootstrapResponse(
                    filterOptions,
                    Overview: null));
        }

        if (validRequest.SubPortfolioId is not null
            && !context.OperationalSubPortfolioAvailable)
        {
            return PortfolioOperationResult<PortfolioBootstrapResponse>.Problem(
                statusCode: 404,
                title: "Campaña Analytics no encontrada",
                detail: $"No existe la campaña '{context.Summary.CampaignCode}' asociada a la subcartera Analytics '{validRequest.SubPortfolioId}' dentro del cliente autorizado.");
        }

        var summaryContext = context.Summary;
        if (!validRequest.TryResolveRange(
                summaryContext,
                out var range,
                out var rangeErrors))
        {
            return PortfolioOperationResult<PortfolioBootstrapResponse>.Validation(rangeErrors);
        }

        var rows = await PortfolioControlCenterDiagnostics.ObservePhaseAsync(
            PortfolioControlCenterDiagnostics.QueryPhase,
            () => overviewRepository.GetOverviewAsync(
                summaryContext.ClientKey,
                summaryContext.CampaignKey,
                validRequest.SubPortfolioId,
                validRequest.BusinessUnit,
                PortfolioBusinessUnitPolicy.CanUseClientLevelTarget(
                    effectiveCrmClientId,
                    validRequest.BusinessUnit),
                range,
                cancellationToken));

        if (!rows.Summary.SnapshotDate.HasValue)
        {
            return PortfolioOperationResult<PortfolioBootstrapResponse>.Problem(
                statusCode: 422,
                title: "Snapshot de cartera no disponible",
                detail:
                    "No existe un snapshot real de cartera dentro del rango solicitado. " +
                    "El resumen no fabricará assigned/managed/pending desde filas carry-forward.");
        }

        return PortfolioOperationResult<PortfolioBootstrapResponse>.Success(
            new PortfolioBootstrapResponse(
                filterOptions,
                PortfolioOverviewResponseMapper.Map(
                    summaryContext,
                    range,
                    rows)));

    }


    private static PortfolioOperationResult<PortfolioBootstrapResponse> CampaignNotFound(
        PortfolioSummaryRequest request) =>
        PortfolioOperationResult<PortfolioBootstrapResponse>.Problem(
            statusCode: 404,
            title: "Campaña Analytics no encontrada",
            detail: request.SubPortfolioId is not null
                ? request.Campaign is null
                    ? $"No existe una campaña disponible para la subcartera Analytics '{request.SubPortfolioId}' dentro del cliente autorizado."
                    : $"No existe la campaña '{request.Campaign}' asociada a la subcartera Analytics '{request.SubPortfolioId}' dentro del cliente autorizado."
                : request.Campaign is null
                    ? "No existe una campaña disponible para el cliente autorizado."
                    : $"No existe la campaña '{request.Campaign}' para el cliente autorizado.");
}
