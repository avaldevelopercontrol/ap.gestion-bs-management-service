using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.DTOs.Analytics;

public sealed record AnalyticsOptionConfigResponse(
    int OptionId,
    string OptionCode,
    string OptionName);
