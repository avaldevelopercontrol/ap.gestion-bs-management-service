using System.Data.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace GesMgmt.Infraestructure.Persistence.Analytics
{
    internal sealed class SqlAnalyticsDbConnectionFactory(
        IConfiguration configuration,
        AnalyticsDatabaseOptions options,
        AnalyticsDatabaseTelemetry telemetry) : IAnalyticsDbConnectionFactory
    {
        public async Task<DbConnection> OpenConnectionAsync(
            CancellationToken cancellationToken = default)
        {
            var connectionString = SqlDatabaseConnectionString.GetRequired(
                configuration,
                "Analytics",
                options.ExpectedDatabase);

            var connection = new SqlConnection(connectionString);

            try
            {
                await telemetry.ObserveAsync(
                    "analytics",
                    "connection.open",
                    () => connection.OpenAsync(cancellationToken));

                return connection;
            }
            catch
            {
                await connection.DisposeAsync();
                throw;
            }
        }
    }
}
