namespace GesMgmt.Infraestructure.Persistence.Analytics
{
    public sealed class AnalyticsDatabaseOptions
    {
        public const string SectionName = "AnalyticsDatabase";
        public const string ConnectionStringName = "AvalAnalyticsConnection";
        public const string DatabaseName = "aval_analytics";

        public int CommandTimeoutSeconds { get; init; } = 15;
    }
}
