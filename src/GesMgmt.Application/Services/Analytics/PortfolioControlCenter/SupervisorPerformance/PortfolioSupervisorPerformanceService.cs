using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Application.Utils.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;

namespace GesMgmt.Application.Services.Analytics.PortfolioControlCenter;

public sealed class PortfolioSupervisorPerformanceService(
    IPortfolioControlCenterAccessService accessService,
    IPortfolioSupervisorPerformanceRepository repository) : IPortfolioSupervisorPerformanceService
{
    public async Task<PortfolioOperationResult<PortfolioSupervisorPerformanceResponse>> GetAsync(
        string? campaign,
        string? subPortfolioId,
        string? supervisorId,
        string? dateFrom,
        string? dateTo,
        string? businessUnit,
        int? crmClientId,
        CancellationToken cancellationToken)
    {
        if (!PortfolioSupervisorPerformanceRequest.TryCreate(
                campaign,
                subPortfolioId,
                supervisorId,
                dateFrom,
                dateTo,
                out var request,
                out var requestErrors))
        {
            return PortfolioOperationResult<PortfolioSupervisorPerformanceResponse>.Validation(requestErrors);
        }

        if (!PortfolioBusinessUnitContract.TryNormalizeRequested(
                businessUnit,
                out var normalizedBusinessUnit,
                out var businessUnitErrors))
        {
            return PortfolioOperationResult<PortfolioSupervisorPerformanceResponse>.Validation(businessUnitErrors);
        }

        var clientAccess = await accessService.ResolveClientAsync(
            crmClientId,
            cancellationToken);

        if (!clientAccess.IsAllowed)
        {
            return PortfolioOperationResult<PortfolioSupervisorPerformanceResponse>.FromAccess(clientAccess);
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
            () => repository.GetSupervisorPerformanceAsync(
                effectiveCrmClientId,
                validRequest,
                cancellationToken));

        if (rows is null)
        {
            return PortfolioOperationResult<PortfolioSupervisorPerformanceResponse>.Problem(
                statusCode: 404,
                title: "Cliente Analytics no encontrado",
                detail: "No existe un cliente Analytics para el scope autorizado.");
        }

        return PortfolioOperationResult<PortfolioSupervisorPerformanceResponse>.Success(
            PortfolioSupervisorPerformanceResponseMapper.Map(rows));

    }
}
