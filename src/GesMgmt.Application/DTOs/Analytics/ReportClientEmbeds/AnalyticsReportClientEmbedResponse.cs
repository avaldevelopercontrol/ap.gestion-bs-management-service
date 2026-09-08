using GesMgmt.Domain.Entities.Analytics;

namespace GesMgmt.Application.DTOs.Analytics;

public sealed record AnalyticsReportClientEmbedResponse(
    int OptionId,
    int ClientId,
    string Name,
    string EmbedUrl);
