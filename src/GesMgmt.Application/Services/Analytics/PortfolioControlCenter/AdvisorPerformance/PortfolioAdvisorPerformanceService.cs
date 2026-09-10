using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Application.Utils.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;

namespace GesMgmt.Application.Services.Analytics.PortfolioControlCenter;

public sealed class PortfolioAdvisorPerformanceService(
    IPortfolioControlCenterAccessService accessService,
    IPortfolioAdvisorPerformanceRepository repository) : IPortfolioAdvisorPerformanceService
{
    public async Task<PortfolioOperationResult<PortfolioAdvisorPerformanceResponse>> GetAsync(
        string? campaign,
        string? subPortfolioId,
        string? supervisorId,
        string? dateFrom,
        string? dateTo,
        string? businessUnit,
        int? crmClientId,
        CancellationToken cancellationToken)
    {
        if (!PortfolioAdvisorPerformanceRequest.TryCreate(
                campaign,
                subPortfolioId,
                supervisorId,
                dateFrom,
                dateTo,
                out var request,
                out var requestErrors))
        {
            return PortfolioOperationResult<PortfolioAdvisorPerformanceResponse>.Validation(requestErrors);
        }

        if (!PortfolioBusinessUnitContract.TryNormalizeRequested(
                businessUnit,
                out var normalizedBusinessUnit,
                out var businessUnitErrors))
        {
            return PortfolioOperationResult<PortfolioAdvisorPerformanceResponse>.Validation(businessUnitErrors);
        }

        var clientAccess = await accessService.ResolveClientAsync(
            crmClientId,
            cancellationToken);

        if (!clientAccess.IsAllowed)
        {
            return PortfolioOperationResult<PortfolioAdvisorPerformanceResponse>.FromAccess(clientAccess);
        }

        var effectiveCrmClientId = clientAccess.CrmClientId!.Value;
        var validRequest = request! with
        {
            BusinessUnit = PortfolioBusinessUnitPolicy.ResolveRequestedOrDefault(
                effectiveCrmClientId,
                normalizedBusinessUnit)
        };
        var rows = await PortfolioControlCenterDiagnostics.ObservePhaseAsync(
            PortfolioControlCenterDiagnostics.QueryPhase,
            () => repository.GetAdvisorPerformanceAsync(
                effectiveCrmClientId,
                validRequest,
                cancellationToken));

        if (rows is null)
        {
            return PortfolioOperationResult<PortfolioAdvisorPerformanceResponse>.Problem(
                statusCode: 404,
                title: "Cliente Analytics no encontrado",
                detail: "No existe un cliente Analytics para el scope autorizado.");
        }

        return PortfolioOperationResult<PortfolioAdvisorPerformanceResponse>.Success(
            PortfolioAdvisorPerformanceResponseMapper.Map(rows));

    }
}
