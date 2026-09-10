
namespace GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;

public sealed record PortfolioOverviewContext(
    PortfolioSummaryContext Summary,
    bool OperationalSubPortfolioAvailable);

public sealed record PortfolioOverviewDbRows(
    PortfolioSummaryDbRow Summary,
    PortfolioTargetProgressDbRow? TargetProgress,
    PortfolioPromisesDbRow Promises,
    IReadOnlyList<PortfolioEvolutionDbRow> Evolution);
