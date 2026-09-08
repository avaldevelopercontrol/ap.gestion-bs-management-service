using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace GesMgmt.Infraestructure.Persistence.Analytics
{
    internal static class SqlDatabaseConnectionString
    {
        public static string GetRequired(
            IConfiguration configuration,
            string connectionStringName,
            string expectedDatabase)
        {
            var connectionString =
                configuration.GetConnectionString(connectionStringName);

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    $"No se configuró ConnectionStrings:{connectionStringName}.");
            }

            SqlConnectionStringBuilder builder;

            try
            {
                builder = new SqlConnectionStringBuilder(connectionString);
            }
            catch (ArgumentException exception)
            {
                throw new InvalidOperationException(
                    $"ConnectionStrings:{connectionStringName} no tiene un formato SQL Server válido.",
                    exception);
            }

            if (!string.Equals(
                    builder.InitialCatalog,
                    expectedDatabase,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"ConnectionStrings:{connectionStringName} debe apuntar a la base {expectedDatabase}.");
            }

            return connectionString;
        }
    }
}
