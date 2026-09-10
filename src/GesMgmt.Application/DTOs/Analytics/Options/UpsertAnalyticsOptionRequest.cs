using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.DTOs.Analytics;

public sealed record UpsertAnalyticsOptionRequest(
    string OptionCode,
    string OptionName,
    bool IsActive);
