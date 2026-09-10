using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Application.Utils.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;

namespace GesMgmt.Application.Services.Analytics.PortfolioControlCenter;

public sealed class PortfolioOverviewService(
    IPortfolioControlCenterAccessService accessService,
    IPortfolioOverviewRepository repository) : IPortfolioOverviewService
{
    public async Task<PortfolioOperationResult<PortfolioOverviewResponse>> GetAsync(
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
            return PortfolioOperationResult<PortfolioOverviewResponse>.Validation(requestErrors);
        }

        if (!PortfolioBusinessUnitContract.TryNormalizeRequested(
                businessUnit,
                out var normalizedBusinessUnit,
                out var businessUnitErrors))
        {
            return PortfolioOperationResult<PortfolioOverviewResponse>.Validation(businessUnitErrors);
        }

        var validRequest = request!;

        var clientAccess = await accessService.ResolveClientAsync(
            crmClientId,
            cancellationToken);

        if (!clientAccess.IsAllowed)
        {
            return PortfolioOperationResult<PortfolioOverviewResponse>.FromAccess(clientAccess);
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
            return PortfolioOperationResult<PortfolioOverviewResponse>.Problem(
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

        if (validRequest.SubPortfolioId is not null
            && !context.OperationalSubPortfolioAvailable)
        {
            return PortfolioOperationResult<PortfolioOverviewResponse>.Problem(
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
            return PortfolioOperationResult<PortfolioOverviewResponse>.Validation(rangeErrors);
        }

        var rows = await PortfolioControlCenterDiagnostics.ObservePhaseAsync(
            PortfolioControlCenterDiagnostics.QueryPhase,
            () => repository.GetOverviewAsync(
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
            return PortfolioOperationResult<PortfolioOverviewResponse>.Problem(
                statusCode: 422,
                title: "Snapshot de cartera no disponible",
                detail:
                    "No existe un snapshot real de cartera dentro del rango solicitado. " +
                    "El resumen no fabricará assigned/managed/pending desde filas carry-forward.");
        }

        return PortfolioOperationResult<PortfolioOverviewResponse>.Success(
            PortfolioOverviewResponseMapper.Map(
                summaryContext,
                range,
                rows));

    }
}
