using GesMgmt.Application.DTOs.Analytics;
using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Application.Validators.Analytics;
using GesMgmt.Domain.Constants.Analytics;
using GesMgmt.Domain.Entities.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;

namespace GesMgmt.Application.Services.Analytics;

public static class AnalyticsReportClientConfigurationContractMapper
{
    public static AnalyticsOptionReportClientEmbed Map(
        AnalyticsReportClientConfiguration configuration) =>
        new(
            configuration.ClientId,
            configuration.Name,
            configuration.IsAvailable,
            configuration.GroupResolution,
            configuration.HasExplicitGroupConfiguration,
            configuration.GroupIds,
            configuration.CandidateGroups
                .Select(group => new AnalyticsOptionReportClientGroup(
                    group.GroupId,
                    group.Name))
                .ToArray(),
            string.IsNullOrWhiteSpace(configuration.EmbedUrl)
                ? null
                : configuration.EmbedUrl.Trim(),
            configuration.IsReady);
}
