namespace GesMgmt.Application.Interfaces.Analytics;

public interface IAnalyticsReportClientEmbedLookupService
{
    Task<AnalyticsReportClientEmbedLookupResult> ResolveAsync(
        int optionId,
        int clientId,
        string reportClient,
        CancellationToken cancellationToken);
}

public enum AnalyticsReportClientEmbedLookupStatus
{
    Success,
    NotFound,
    InvalidConfiguration
}

public sealed record AnalyticsReportClientEmbedLookupResult(
    AnalyticsReportClientEmbedLookupStatus Status,
    string? EmbedUrl = null);
