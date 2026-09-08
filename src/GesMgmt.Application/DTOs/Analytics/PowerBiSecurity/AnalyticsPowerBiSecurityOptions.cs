using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.DTOs.Analytics;

public sealed class AnalyticsPowerBiSecurityOptions
{
    public const string SectionName = "AnalyticsPowerBiSecurity";

    // Publish to web is public by design. Keep this switch enabled only while
    // the application intentionally relies on public Power BI publications.
    public bool AllowPublishToWeb { get; init; } = true;
}
