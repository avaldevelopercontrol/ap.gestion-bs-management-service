using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;
using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Application.Utils.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;

namespace GesMgmt.Application.Services.Analytics.PortfolioControlCenter;

public sealed class PortfolioCampaignPerformanceService(
    IPortfolioControlCenterAccessService accessService,
    IPortfolioCampaignPerformanceRepository repository) : IPortfolioCampaignPerformanceService
{
    public async Task<PortfolioOperationResult<PortfolioCampaignPerformanceResponse>> GetAsync(
        string? campaign,
        string? subPortfolioId,
        string? supervisorId,
        string? dateFrom,
        string? dateTo,
        string? businessUnit,
        int? crmClientId,
        CancellationToken cancellationToken)
    {
        if (!PortfolioCampaignPerformanceRequest.TryCreate(
                campaign,
                subPortfolioId,
                supervisorId,
                dateFrom,
                dateTo,
                out var request,
                out var requestErrors))
        {
            return PortfolioOperationResult<PortfolioCampaignPerformanceResponse>.Validation(requestErrors);
        }

        if (!PortfolioBusinessUnitContract.TryNormalizeRequested(
                businessUnit,
                out var normalizedBusinessUnit,
                out var businessUnitErrors))
        {
            return PortfolioOperationResult<PortfolioCampaignPerformanceResponse>.Validation(businessUnitErrors);
        }

        var clientAccess = await accessService.ResolveClientAsync(
            crmClientId,
            cancellationToken);

        if (!clientAccess.IsAllowed)
        {
            return PortfolioOperationResult<PortfolioCampaignPerformanceResponse>.FromAccess(clientAccess);
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
            () => repository.GetCampaignPerformanceAsync(
                effectiveCrmClientId,
                validRequest,
                cancellationToken));

        if (rows is null)
        {
            return PortfolioOperationResult<PortfolioCampaignPerformanceResponse>.Problem(
                statusCode: 404,
                title: "Cliente Analytics no encontrado",
                detail: "No existe un cliente Analytics para el scope autorizado.");
        }

        return PortfolioOperationResult<PortfolioCampaignPerformanceResponse>.Success(
            PortfolioCampaignPerformanceResponseMapper.Map(rows));

    }
}
