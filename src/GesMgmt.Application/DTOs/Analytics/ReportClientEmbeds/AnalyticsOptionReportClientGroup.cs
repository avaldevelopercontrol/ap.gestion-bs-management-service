using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.DTOs.Analytics;

public sealed record AnalyticsOptionReportClientGroup(
    int GroupId,
    string Name);
