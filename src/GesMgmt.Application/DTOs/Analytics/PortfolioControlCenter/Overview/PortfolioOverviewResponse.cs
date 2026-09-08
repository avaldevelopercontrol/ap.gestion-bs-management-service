namespace GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

public sealed record PortfolioOverviewResponse(
    PortfolioSummaryResponse Summary,
    PortfolioTargetProgressResponse TargetProgress,
    PortfolioPromisesResponse Promises,
    PortfolioEvolutionResponse Evolution);
