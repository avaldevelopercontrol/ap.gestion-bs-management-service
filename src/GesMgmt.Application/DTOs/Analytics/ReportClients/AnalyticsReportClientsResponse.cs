using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.DTOs.Analytics;

public sealed record AnalyticsReportClientsResponse(
    int OptionId,
    IReadOnlyList<AnalyticsReportClientOption> Clients);
