using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence.Analytics;
using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal sealed class PortfolioAdvisorPerformanceRepository(
    IAnalyticsQueryExecutor queryExecutor,
    AnalyticsDatabaseOptions databaseOptions)
    : IPortfolioAdvisorPerformanceRepository
{
    private readonly int _commandTimeoutSeconds =
        databaseOptions.CommandTimeoutSeconds;

    public async Task<IReadOnlyList<PortfolioAdvisorPerformanceDbRow>?> GetAdvisorPerformanceAsync(
        int crmClientId,
        PortfolioAdvisorPerformanceRequest request,
        CancellationToken cancellationToken)
    {
        var result = await queryExecutor.QueryTwoAsync<int, PortfolioAdvisorPerformanceDbRow>(
            PortfolioAdvisorPerformanceSql.AdvisorPerformance,
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
