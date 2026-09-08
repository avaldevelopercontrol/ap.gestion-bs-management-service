using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence.Analytics;
using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal sealed class PortfolioSupervisorPerformanceRepository(
    IAnalyticsQueryExecutor queryExecutor,
    AnalyticsDatabaseOptions databaseOptions)
    : IPortfolioSupervisorPerformanceRepository
{
    private readonly int _commandTimeoutSeconds =
        databaseOptions.CommandTimeoutSeconds;

    public async Task<IReadOnlyList<PortfolioSupervisorPerformanceDbRow>?> GetSupervisorPerformanceAsync(
        int crmClientId,
        PortfolioSupervisorPerformanceRequest request,
        CancellationToken cancellationToken)
    {
        var result = await queryExecutor.QueryTwoAsync<int, PortfolioSupervisorPerformanceDbRow>(
            PortfolioSupervisorPerformanceSql.SupervisorPerformance,
            new
            {
                CrmClientId = crmClientId,
                CampaignCode = request.Campaign,
                SubPortfolioId = request.SubPortfolioId,
                BusinessUnit = request.BusinessUnit,
                SupervisorId = request.SupervisorId,
                DateFrom = request.DateFrom?.ToDateTime(TimeOnly.MinValue),
                DateTo = request.DateTo?.ToDateTime(TimeOnly.MinValue)
            },
            _commandTimeoutSeconds,
            cancellationToken);

        return result.First.Count switch
        {
            0 => null,
            1 => result.Second,
            _ => throw new InvalidOperationException(
                "La resolución del cliente Analytics devolvió más de una fila.")
        };
    }
}
