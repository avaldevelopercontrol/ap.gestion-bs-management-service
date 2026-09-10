using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.DTOs.Analytics;

public sealed record AnalyticsPowerBiAccessResponse(
    IReadOnlyList<AnalyticsPowerBiOptionAccessResponse> Options);
