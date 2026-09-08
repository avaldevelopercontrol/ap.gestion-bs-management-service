namespace GesMgmt.Infraestructure.Persistence.Analytics
{
    public enum AnalyticsDatabaseFailureCategory
    {
        None,
        Configuration,
        TlsHandshake,
        Authentication,
        Connectivity,
        DatabaseMismatch,
        Unknown
    }

    public static class AnalyticsDatabaseFailureCategoryExtensions
    {
        public static string ToWireValue(
            this AnalyticsDatabaseFailureCategory category) =>
            category switch
            {
                AnalyticsDatabaseFailureCategory.None => "none",
                AnalyticsDatabaseFailureCategory.Configuration => "configuration",
                AnalyticsDatabaseFailureCategory.TlsHandshake => "tls_handshake",
                AnalyticsDatabaseFailureCategory.Authentication => "authentication",
                AnalyticsDatabaseFailureCategory.Connectivity => "connectivity",
                AnalyticsDatabaseFailureCategory.DatabaseMismatch => "database_mismatch",
                _ => "unknown"
            };
    }
}
