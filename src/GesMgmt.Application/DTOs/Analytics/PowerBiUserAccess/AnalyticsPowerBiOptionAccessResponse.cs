using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.DTOs.Analytics;

public sealed record AnalyticsPowerBiOptionAccessResponse(
    int OptionId,
    bool Allowed,
    bool RequiresClientSelection);
