namespace GesMgmt.Infraestructure.Persistence.Analytics
{
    public sealed class AnalyticsDatabaseOptions
    {
        public const string SectionName = "AnalyticsDatabase";

        public string ExpectedDatabase { get; init; } = "aval_analytics";

        public int CommandTimeoutSeconds { get; init; } = 15;
    }
}
