using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Application.Utils.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;

namespace GesMgmt.Application.Services.Analytics.PortfolioControlCenter;

public sealed class PortfolioTargetProgressService(
    IPortfolioControlCenterAccessService accessService,
    IPortfolioTargetProgressRepository repository) : IPortfolioTargetProgressService
{
    public async Task<PortfolioOperationResult<PortfolioTargetProgressResponse>> GetAsync(
        string? campaign,
        string? subPortfolioId,
        string? dateTo,
        string? businessUnit,
        int? crmClientId,
        CancellationToken cancellationToken)
    {
        if (!PortfolioTargetProgressRequest.TryCreate(
                campaign,
                subPortfolioId,
                dateTo,
                out var request,
                out var requestErrors))
        {
            return PortfolioOperationResult<PortfolioTargetProgressResponse>.Validation(requestErrors);
        }

        if (!PortfolioBusinessUnitContract.TryNormalizeRequested(
                businessUnit,
                out var normalizedBusinessUnit,
                out var businessUnitErrors))
        {
            return PortfolioOperationResult<PortfolioTargetProgressResponse>.Validation(businessUnitErrors);
        }

        var validRequest = request!;

        var clientAccess = await accessService.ResolveClientAsync(
            crmClientId,
            cancellationToken);

        if (!clientAccess.IsAllowed)
        {
            return PortfolioOperationResult<PortfolioTargetProgressResponse>.FromAccess(clientAccess);
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
                PortfolioBusinessUnitPolicy.CanUseClientLevelTarget(
                    effectiveCrmClientId,
                    validRequest.BusinessUnit),
                cancellationToken));

        if (context is null)
        {
            return PortfolioOperationResult<PortfolioTargetProgressResponse>.Problem(
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

        if (!validRequest.TryResolveDateTo(
                context,
                out var effectiveDateTo,
                out var dateErrors))
        {
            return PortfolioOperationResult<PortfolioTargetProgressResponse>.Validation(dateErrors);
        }

        PortfolioTargetProgressDbRow? row = null;

        if (validRequest.SubPortfolioId is null
            && PortfolioBusinessUnitPolicy.CanUseClientLevelTarget(
                effectiveCrmClientId,
                validRequest.BusinessUnit))
        {
            row = await PortfolioControlCenterDiagnostics.ObservePhaseAsync(
                PortfolioControlCenterDiagnostics.QueryPhase,
                () => repository.GetTargetProgressAsync(
                    context.ClientKey,
                    context.CampaignKey,
                    validRequest.BusinessUnit,
                    effectiveDateTo,
                    cancellationToken));
        }

        return PortfolioOperationResult<PortfolioTargetProgressResponse>.Success(
            PortfolioTargetProgressResponseMapper.Map(
                context,
                effectiveDateTo,
                row));

    }
}
