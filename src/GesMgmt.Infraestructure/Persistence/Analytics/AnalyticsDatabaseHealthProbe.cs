namespace GesMgmt.Infraestructure.Persistence.Analytics
{
    internal sealed class AnalyticsDatabaseHealthProbe(
        IAnalyticsQueryExecutor queryExecutor,
        AnalyticsDatabaseOptions options) : IAnalyticsDatabaseHealthProbe
    {
        public async Task<DatabaseHealthProbeResult> CheckAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                var databaseName = await queryExecutor.QuerySingleAsync<string>(
                    "SELECT DB_NAME();",
                    parameters: null,
                    options.CommandTimeoutSeconds,
                    cancellationToken);

                if (!string.Equals(
                        databaseName,
                        options.ExpectedDatabase,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return Failure(
                        AnalyticsDatabaseFailureCategory.DatabaseMismatch,
                        "La conexión Analytics respondió, pero apunta a una base distinta de la esperada.");
                }

                return Success("Base Analytics disponible.");
            }
            catch (Exception exception)
            {
                var category =
                    AnalyticsDatabaseFailureClassifier.Classify(exception);

                return Failure(category, GetSafeDescription(category));
            }
        }

        private static DatabaseHealthProbeResult Success(string description) =>
            new(
                true,
                AnalyticsDatabaseFailureCategory.None.ToWireValue(),
                description);

        private static DatabaseHealthProbeResult Failure(
            AnalyticsDatabaseFailureCategory category,
            string description) =>
            new(false, category.ToWireValue(), description);

        private static string GetSafeDescription(
            AnalyticsDatabaseFailureCategory category) =>
            category switch
            {
                AnalyticsDatabaseFailureCategory.Configuration =>
                    "La conexión Analytics no está configurada correctamente en el runtime.",
                AnalyticsDatabaseFailureCategory.TlsHandshake =>
                    "SQL es alcanzable, pero falló la negociación TLS durante PRELOGIN.",
                AnalyticsDatabaseFailureCategory.Authentication =>
                    "SQL respondió, pero rechazó la autenticación de la conexión Analytics.",
                AnalyticsDatabaseFailureCategory.Connectivity =>
                    "No se pudo establecer la conectividad necesaria con SQL Analytics.",
                _ =>
                    "No se pudo validar la conexión con la base Analytics."
            };
    }
}
