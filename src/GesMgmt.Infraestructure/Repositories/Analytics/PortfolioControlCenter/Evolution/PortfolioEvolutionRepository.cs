using GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;
using GesMgmt.Domain.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Infraestructure.Persistence.Analytics;
using GesMgmt.Application.Interfaces.Analytics.PortfolioControlCenter;
using GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

namespace GesMgmt.Infraestructure.Repositories.Analytics.PortfolioControlCenter;

internal sealed class PortfolioEvolutionRepository(
    IAnalyticsQueryExecutor queryExecutor,
    AnalyticsDatabaseOptions databaseOptions)
    : IPortfolioEvolutionRepository
{
    private readonly int _commandTimeoutSeconds =
        databaseOptions.CommandTimeoutSeconds;

    public async Task<PortfolioEvolutionContext?> ResolveContextAsync(
        int crmClientId,
        string? campaignCode,
        long? subPortfolioId,
        string? businessUnit,
        CancellationToken cancellationToken)
    {
        var row = await queryExecutor.QuerySingleOrDefaultAsync<ContextDbRow>(
            PortfolioEvolutionSql.ResolveContext,
            new
            {
                CrmClientId = crmClientId,
                CampaignCode = campaignCode,
                SubPortfolioId = subPortfolioId,
                BusinessUnit = businessUnit
            },
            _commandTimeoutSeconds,
            cancellationToken);

        if (row is null)
        {
            return null;
        }

        return new PortfolioEvolutionContext(
            row.ClientKey,
            row.CampaignKey,
            row.CampaignCode,
            row.CampaignName,
            DateOnly.FromDateTime(row.StartDate),
            DateOnly.FromDateTime(row.EndDate),
            row.LatestEvolutionDate.HasValue
                ? DateOnly.FromDateTime(row.LatestEvolutionDate.Value)
                : null);
    }

    public async Task<IReadOnlyList<PortfolioEvolutionDbRow>> GetEvolutionAsync(
        int clientKey,
        int campaignKey,
        long? subPortfolioId,
        string? businessUnit,
        PortfolioEvolutionRange range,
        CancellationToken cancellationToken)
    {
        var dateFrom = range.DateFrom.ToDateTime(TimeOnly.MinValue);
        var dateToExclusive = range.DateTo
            .AddDays(1)
            .ToDateTime(TimeOnly.MinValue);

        return await queryExecutor.QueryAsync<PortfolioEvolutionDbRow>(
            PortfolioEvolutionSql.Evolution,
            new
            {
                ClientKey = clientKey,
                CampaignKey = campaignKey,
                SubPortfolioId = subPortfolioId,
                BusinessUnit = businessUnit,
                DateFrom = dateFrom,
                DateToExclusive = dateToExclusive
            },
            _commandTimeoutSeconds,
            cancellationToken);
    }

    private sealed record ContextDbRow
    {
        public int ClientKey { get; init; }
        public int CampaignKey { get; init; }
        public required string CampaignCode { get; init; }
        public required string CampaignName { get; init; }
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public DateTime? LatestEvolutionDate { get; init; }
    }
}
