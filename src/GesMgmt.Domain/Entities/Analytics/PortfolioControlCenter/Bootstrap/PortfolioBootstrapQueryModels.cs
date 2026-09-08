
namespace GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;

public sealed record PortfolioBootstrapSource(
    PortfolioFilterOptionsDbResult FilterOptions,
    PortfolioOverviewContext? OverviewContext);

public sealed record PortfolioBootstrapOverviewContextDbRow
{
    public int ClientKey { get; init; }
    public int CampaignKey { get; init; }
    public required string CampaignCode { get; init; }
    public required string CampaignName { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public DateTime? LatestDataDate { get; init; }
    public bool OperationalSubPortfolioAvailable { get; init; }
}
