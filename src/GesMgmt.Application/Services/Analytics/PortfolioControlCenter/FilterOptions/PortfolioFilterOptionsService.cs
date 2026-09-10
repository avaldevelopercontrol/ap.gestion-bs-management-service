using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Application.Utils.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;

namespace GesMgmt.Application.Services.Analytics.PortfolioControlCenter;

public sealed class PortfolioFilterOptionsService(
    IPortfolioControlCenterAccessService accessService,
    IPortfolioFilterOptionsRepository repository) : IPortfolioFilterOptionsService
{
    public async Task<PortfolioOperationResult<PortfolioFilterOptionsResponse>> GetAsync(
        string? businessUnit,
        int? crmClientId,
        CancellationToken cancellationToken)
    {
        if (!PortfolioBusinessUnitContract.TryNormalizeRequested(
                businessUnit,
                out var normalizedBusinessUnit,
                out var businessUnitErrors))
        {
            return PortfolioOperationResult<PortfolioFilterOptionsResponse>.Validation(businessUnitErrors);
        }

        var clientAccess = await accessService.ResolveClientAsync(
            crmClientId,
            cancellationToken);

        if (!clientAccess.IsAllowed)
        {
            return PortfolioOperationResult<PortfolioFilterOptionsResponse>.FromAccess(clientAccess);
        }

        var effectiveCrmClientId = clientAccess.CrmClientId!.Value;
        var source = await PortfolioControlCenterDiagnostics.ObservePhaseAsync(
            PortfolioControlCenterDiagnostics.QueryPhase,
            () => repository.GetFilterOptionsAsync(
                effectiveCrmClientId,
                cancellationToken));

        if (source is null)
        {
            return PortfolioOperationResult<PortfolioFilterOptionsResponse>.Problem(
                statusCode: 404,
                title: "Cliente Analytics no encontrado",
                detail: "No existe un cliente Analytics para el scope autorizado.");
        }

        if (!PortfolioBusinessUnitContract.TryResolve(
                normalizedBusinessUnit,
                PortfolioBusinessUnitPolicy.GetBackwardCompatibleDefault(
                    effectiveCrmClientId),
                source.BusinessUnits,
                out var businessUnitSelection,
                out var selectionErrors))
        {
            return PortfolioOperationResult<PortfolioFilterOptionsResponse>.Validation(selectionErrors);
        }

        return PortfolioOperationResult<PortfolioFilterOptionsResponse>.Success(
            PortfolioFilterOptionsResponseMapper.Map(
                source,
                effectiveCrmClientId,
                businessUnitSelection.SelectedBusinessUnit));

    }
}
