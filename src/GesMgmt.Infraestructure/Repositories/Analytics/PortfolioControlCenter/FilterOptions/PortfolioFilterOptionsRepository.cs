using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence.Analytics;
using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal sealed class PortfolioFilterOptionsRepository(
    IAnalyticsQueryExecutor queryExecutor,
    AnalyticsDatabaseOptions databaseOptions)
    : IPortfolioFilterOptionsRepository
{
    private readonly int _commandTimeoutSeconds =
        databaseOptions.CommandTimeoutSeconds;

    public async Task<PortfolioFilterOptionsDbResult?> GetFilterOptionsAsync(
        int crmClientId,
        CancellationToken cancellationToken)
    {
        var result = await queryExecutor.QueryFourAsync<
            int,
            PortfolioFilterCampaignDbRow,
            PortfolioFilterSubPortfolioCampaignDbRow,
            PortfolioFilterSupervisorContextDbRow>(
            PortfolioFilterOptionsSql.FilterOptions,
            new { CrmClientId = crmClientId },
            _commandTimeoutSeconds,
            cancellationToken);

        return result.First.Count switch
        {
            0 => null,
            1 => PortfolioFilterOptionsDbResultMapper.Map(
                result.Second,
                result.Third,
                result.Fourth),
            _ => throw new InvalidOperationException(
                "La resolución del cliente Analytics devolvió más de una fila.")
        };
    }

}
