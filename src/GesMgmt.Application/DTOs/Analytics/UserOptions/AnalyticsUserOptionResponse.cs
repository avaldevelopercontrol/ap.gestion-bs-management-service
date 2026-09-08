using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.DTOs.Analytics;

public sealed record AnalyticsUserOptionResponse(
    int OptionId,
    string OptionCode,
    string OptionName);
