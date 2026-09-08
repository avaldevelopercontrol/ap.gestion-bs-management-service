using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.DTOs.Analytics;

public sealed class AnalyticsAdministrationOptions
{
    public const string SectionName = "AnalyticsAdministration";

    public int[] AdministratorUserIds { get; init; } = [];
}
