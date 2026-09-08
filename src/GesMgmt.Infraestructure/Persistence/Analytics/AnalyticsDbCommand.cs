namespace GesMgmt.Infraestructure.Persistence.Analytics
{
    internal sealed record AnalyticsDbCommand(
        string Sql,
        object? Parameters);
}
