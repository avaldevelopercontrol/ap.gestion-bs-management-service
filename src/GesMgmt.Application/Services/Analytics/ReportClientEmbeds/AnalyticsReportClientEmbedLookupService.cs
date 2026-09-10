using GesMgmt.Application.Interfaces.Analytics;
using GesMgmt.Application.Utils.Analytics;
using GesMgmt.Domain.Interfaces.Analytics;

namespace GesMgmt.Application.Services.Analytics;

public sealed class AnalyticsReportClientEmbedLookupService(
    IAnalyticsReportClientEmbedRepository repository)
    : IAnalyticsReportClientEmbedLookupService
{
    public async Task<AnalyticsReportClientEmbedLookupResult> ResolveAsync(
        int optionId,
        int clientId,
        string reportClient,
        CancellationToken cancellationToken)
    {
        var mapping = await repository.GetAsync(
            optionId,
            clientId,
            reportClient,
            cancellationToken);

        if (mapping is null)
        {
            return new AnalyticsReportClientEmbedLookupResult(
                AnalyticsReportClientEmbedLookupStatus.NotFound);
        }

        if (!PowerBiPublishToWebUrl.TryNormalize(
            mapping.EmbedUrl,
            out var embedUrl))
        {
            return new AnalyticsReportClientEmbedLookupResult(
                AnalyticsReportClientEmbedLookupStatus.InvalidConfiguration);
        }

        return new AnalyticsReportClientEmbedLookupResult(
            AnalyticsReportClientEmbedLookupStatus.Success,
            embedUrl);
    }
}
