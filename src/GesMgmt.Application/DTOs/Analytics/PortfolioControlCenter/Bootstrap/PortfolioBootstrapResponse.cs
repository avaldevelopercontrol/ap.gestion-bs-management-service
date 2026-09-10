namespace GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

public sealed record PortfolioBootstrapResponse(
    PortfolioFilterOptionsResponse FilterOptions,
    PortfolioOverviewResponse? Overview);
