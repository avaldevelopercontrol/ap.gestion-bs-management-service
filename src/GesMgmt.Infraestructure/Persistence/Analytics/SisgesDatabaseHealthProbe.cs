namespace GesMgmt.Infraestructure.Persistence.Analytics
{
    internal sealed class SisgesDatabaseHealthProbe(
        ISisgesQueryExecutor queryExecutor,
        SisgesDatabaseOptions options) : ISisgesDatabaseHealthProbe
    {
        public async Task<DatabaseHealthProbeResult> CheckAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                var rows = await queryExecutor.QueryAsync<string>(
                    "SELECT DB_NAME();",
                    parameters: null,
                    cancellationToken);

                var databaseName = rows.SingleOrDefault();

                if (!string.Equals(
                        databaseName,
                        options.ExpectedDatabase,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return Failure(
                        AnalyticsDatabaseFailureCategory.DatabaseMismatch,
                        "La conexión SISGES respondió, pero apunta a una base distinta de la esperada.");
                }

                return Success("Base SISGES disponible.");
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
                    "La conexión SISGES no está configurada correctamente en el runtime.",
                AnalyticsDatabaseFailureCategory.TlsHandshake =>
                    "SQL es alcanzable, pero falló la negociación TLS durante PRELOGIN de SISGES.",
                AnalyticsDatabaseFailureCategory.Authentication =>
                    "SQL respondió, pero rechazó la autenticación de la conexión SISGES.",
                AnalyticsDatabaseFailureCategory.Connectivity =>
                    "No se pudo establecer la conectividad necesaria con SQL SISGES.",
                _ =>
                    "No se pudo validar la conexión con la base SISGES."
            };
    }
}
