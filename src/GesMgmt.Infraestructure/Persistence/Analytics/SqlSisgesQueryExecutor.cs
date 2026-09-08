using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace GesMgmt.Infraestructure.Persistence.Analytics
{
    internal sealed class SqlSisgesQueryExecutor(
        IConfiguration configuration,
        SisgesDatabaseOptions options,
        AnalyticsDatabaseTelemetry telemetry) : ISisgesQueryExecutor
    {
        public async Task<IReadOnlyList<T>> QueryAsync<T>(
            string sql,
            object? parameters,
            CancellationToken cancellationToken)
        {
            var connectionString = SqlDatabaseConnectionString.GetRequired(
                configuration,
                "AvalCobConnection",
                options.ExpectedDatabase);

            await using var connection = new SqlConnection(connectionString);

            await telemetry.ObserveAsync(
                "sisges",
                "connection.open",
                () => connection.OpenAsync(cancellationToken));

            var result = await telemetry.ObserveAsync(
                "sisges",
                "query",
                () => connection.QueryAsync<T>(
                    new CommandDefinition(
                        sql,
                        parameters,
                        commandTimeout: options.CommandTimeoutSeconds,
                        cancellationToken: cancellationToken)));

            return result.AsList();
        }
    }
}
