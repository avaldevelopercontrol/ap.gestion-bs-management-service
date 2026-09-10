using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Application.Utils.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;

namespace GesMgmt.Application.Services.Analytics.PortfolioControlCenter;

public sealed class PortfolioOverduePromisesService(
    IPortfolioControlCenterAccessService accessService,
    IPortfolioPromisesRepository promisesRepository,
    IPortfolioOverduePromisesRepository repository) : IPortfolioOverduePromisesService
{
    public async Task<PortfolioOperationResult<PortfolioOverduePromisesResponse>> GetAsync(
        string? campaign,
        string? subPortfolioId,
        string? page,
        string? pageSize,
        string? aging,
        string? sortBy,
        string? sortDirection,
        string? businessUnit,
        int? crmClientId,
        CancellationToken cancellationToken)
    {
        if (!PortfolioOverduePromisesRequest.TryCreate(
                campaign,
                subPortfolioId,
                page,
                pageSize,
                aging,
                sortBy,
                sortDirection,
                out var request,
                out var requestErrors))
        {
            return PortfolioOperationResult<PortfolioOverduePromisesResponse>.Validation(requestErrors);
        }

        if (!PortfolioBusinessUnitContract.TryNormalizeRequested(
                businessUnit,
                out var normalizedBusinessUnit,
                out var businessUnitErrors))
        {
            return PortfolioOperationResult<PortfolioOverduePromisesResponse>.Validation(businessUnitErrors);
        }

        var validRequest = request!;

        var clientAccess = await accessService.ResolveClientAsync(
            crmClientId,
            cancellationToken);

        if (!clientAccess.IsAllowed)
        {
            return PortfolioOperationResult<PortfolioOverduePromisesResponse>.FromAccess(clientAccess);
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
            () => promisesRepository.ResolveContextAsync(
                effectiveCrmClientId,
                validRequest.Campaign,
                validRequest.SubPortfolioId,
                validRequest.BusinessUnit,
                cancellationToken));

        if (context is null)
        {
            return PortfolioOperationResult<PortfolioOverduePromisesResponse>.Problem(
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

        var result = await PortfolioControlCenterDiagnostics.ObservePhaseAsync(
            PortfolioControlCenterDiagnostics.QueryPhase,
            () => repository.GetAsync(
                context.ClientKey,
                context.CampaignKey,
                validRequest.SubPortfolioId,
                validRequest.BusinessUnit,
                validRequest.Page,
                validRequest.PageSize,
                validRequest.Aging,
                validRequest.SortBy,
                validRequest.SortDirection,
                cancellationToken));

        return PortfolioOperationResult<PortfolioOverduePromisesResponse>.Success(
            PortfolioOverduePromisesResponseMapper.Map(
                context,
                result));

    }
}
