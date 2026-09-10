namespace GesMgmt.Domain.Entities.Analytics.PortfolioControlCenter;

public sealed record PortfolioFilterCampaignDbRow
{
    public required string CampaignCode { get; init; }
    public required string CampaignName { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public DateTime AvailableDateFrom { get; init; }
    public DateTime AvailableDateTo { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
public sealed record PortfolioFilterSubPortfolioDbRow
{
    public long SubPortfolioId { get; init; }
    public required string SubPortfolioName { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
public sealed record PortfolioFilterSubPortfolioCampaignDbRow
{
    public long SubPortfolioId { get; init; }
    public required string SubPortfolioName { get; init; }
    public string? BusinessUnitCode { get; init; }
    public required string CampaignCode { get; init; }
    public DateTime AvailableDateFrom { get; init; }
    public DateTime AvailableDateTo { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
    public DateTime? SubPortfolioUpdatedAtUtc { get; init; }
    public long SubPortfolioSortOrder { get; init; }
}
public sealed record PortfolioFilterSupervisorDbRow
{
    public int SupervisorId { get; init; }
    public required string SupervisorName { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
}
public sealed record PortfolioFilterSupervisorContextDbRow
{
    public int SupervisorId { get; init; }
    public required string SupervisorName { get; init; }
    public long SubPortfolioId { get; init; }
    public required string CampaignCode { get; init; }
    public DateTime AvailableDateFrom { get; init; }
    public DateTime AvailableDateTo { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
    public DateTime? SupervisorUpdatedAtUtc { get; init; }
    public long SupervisorSortOrder { get; init; }
}
public sealed record PortfolioFilterOptionsDbResult(
    IReadOnlyList<PortfolioFilterCampaignDbRow> Campaigns,
    IReadOnlyList<PortfolioFilterSubPortfolioDbRow> SubPortfolios,
    IReadOnlyList<PortfolioFilterSubPortfolioCampaignDbRow> SubPortfolioCampaigns,
    IReadOnlyList<PortfolioFilterSupervisorDbRow> Supervisors,
    IReadOnlyList<PortfolioFilterSupervisorContextDbRow> SupervisorContexts)
{
    public IReadOnlyList<string> BusinessUnits { get; init; } = [];
}
