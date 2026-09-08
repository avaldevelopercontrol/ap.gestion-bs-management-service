namespace GesMgmt.Application.DTOs.Analytics.PortfolioControlCenter;

public sealed class PortfolioControlCenterPerformanceOptions
{
    public const string SectionName = "PortfolioControlCenter:Performance";

    public const int DefaultMaxConcurrentRequests = 80;
    public const int DefaultQueueLimit = 240;
    public const int DefaultRequestTimeoutSeconds = 60;
    public const int DefaultDetailCacheSeconds = 30;
    public const int DefaultDetailCacheMaxEntries = 512;

    public int MaxConcurrentRequests { get; init; } = DefaultMaxConcurrentRequests;
    public int QueueLimit { get; init; } = DefaultQueueLimit;
    public int RequestTimeoutSeconds { get; init; } = DefaultRequestTimeoutSeconds;
    public int DetailCacheSeconds { get; init; } = DefaultDetailCacheSeconds;
    public int DetailCacheMaxEntries { get; init; } = DefaultDetailCacheMaxEntries;

    public void Validate()
    {
        if (MaxConcurrentRequests is < 1 or > 512)
        {
            throw new InvalidOperationException(
                $"{SectionName}:MaxConcurrentRequests debe estar entre 1 y 512.");
        }

        if (QueueLimit is < 0 or > 4096)
        {
            throw new InvalidOperationException(
                $"{SectionName}:QueueLimit debe estar entre 0 y 4096.");
        }

        if (RequestTimeoutSeconds is < 1 or > 300)
        {
            throw new InvalidOperationException(
                $"{SectionName}:RequestTimeoutSeconds debe estar entre 1 y 300.");
        }

        if (DetailCacheSeconds is < 0 or > 300)
        {
            throw new InvalidOperationException(
                $"{SectionName}:DetailCacheSeconds debe estar entre 0 y 300.");
        }

        if (DetailCacheMaxEntries is < 1 or > 4096)
        {
            throw new InvalidOperationException(
                $"{SectionName}:DetailCacheMaxEntries debe estar entre 1 y 4096.");
        }
    }
}
