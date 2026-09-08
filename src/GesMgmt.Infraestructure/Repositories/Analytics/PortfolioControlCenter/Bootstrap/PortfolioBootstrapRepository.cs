using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence.Analytics;
using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal sealed class PortfolioBootstrapRepository(
    IAnalyticsQueryExecutor queryExecutor,
    AnalyticsDatabaseOptions databaseOptions)
    : IPortfolioBootstrapRepository
{
    private readonly int _commandTimeoutSeconds =
        databaseOptions.CommandTimeoutSeconds;

    public async Task<PortfolioBootstrapSource?> ResolveAsync(
        int crmClientId,
        string? campaignCode,
        long? subPortfolioId,
        string? businessUnit,
        CancellationToken cancellationToken)
    {
        var result = await queryExecutor.QueryFiveAsync<
            int,
            PortfolioFilterCampaignDbRow,
            PortfolioFilterSubPortfolioCampaignDbRow,
            PortfolioFilterSupervisorContextDbRow,
            PortfolioBootstrapOverviewContextDbRow>(
                PortfolioBootstrapSql.FilterOptionsAndContext,
                new
                {
                    CrmClientId = crmClientId,
                    CampaignCode = campaignCode,
                    SubPortfolioId = subPortfolioId,
                    BusinessUnit = businessUnit
                },
                _commandTimeoutSeconds,
                cancellationToken);

        if (result.First.Count == 0)
        {
            return null;
        }

        if (result.First.Count != 1)
        {
            throw new InvalidOperationException(
                "La resolución del cliente Analytics devolvió más de una fila.");
        }

        var context = result.Fifth.Count switch
        {
            0 => null,
            1 => MapContext(result.Fifth[0]),
            _ => throw new InvalidOperationException(
                "La resolución del contexto Portfolio devolvió más de una fila.")
        };

        return new PortfolioBootstrapSource(
            PortfolioFilterOptionsDbResultMapper.Map(
                result.Second,
                result.Third,
                result.Fourth),
            context);
    }

    private static PortfolioOverviewContext MapContext(
        PortfolioBootstrapOverviewContextDbRow row) =>
        new(
            new PortfolioSummaryContext(
                row.ClientKey,
                row.CampaignKey,
                row.CampaignCode,
                row.CampaignName,
                DateOnly.FromDateTime(row.StartDate),
                DateOnly.FromDateTime(row.EndDate),
                row.LatestDataDate.HasValue
                    ? DateOnly.FromDateTime(row.LatestDataDate.Value)
                    : null),
            row.OperationalSubPortfolioAvailable);
}
