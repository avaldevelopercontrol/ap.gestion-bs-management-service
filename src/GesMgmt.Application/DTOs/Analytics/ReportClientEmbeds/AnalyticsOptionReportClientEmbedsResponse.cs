using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.DTOs.Analytics;

public sealed record AnalyticsOptionReportClientEmbedsResponse(
    int OptionId,
    IReadOnlyList<AnalyticsOptionReportClientEmbed> Clients);
