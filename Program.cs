using Microsoft.Data.SqlClient;

if (args.Length != 3)
{
    Console.WriteLine("Uso: sql-probe <server> <database> <user>");
    return;
}

var server = args[0];
var database = args[1];
var user = args[2];
var password = Environment.GetEnvironmentVariable("SQL_PASSWORD");

if (string.IsNullOrEmpty(password))
{
    Console.WriteLine("SQL_PASSWORD no está definido.");
    return;
}

var builder = new SqlConnectionStringBuilder
{
    DataSource = server,
    InitialCatalog = database,
    UserID = user,
    Password = password,
    Encrypt = false,
    TrustServerCertificate = true,
    ConnectTimeout = 10
};

try
{
    await using var connection = new SqlConnection(builder.ConnectionString);
    await connection.OpenAsync();

    await using var command = connection.CreateCommand();
    command.CommandText = "SELECT DB_NAME(), SUSER_SNAME();";

    await using var reader = await command.ExecuteReaderAsync();

    if (await reader.ReadAsync())
    {
        Console.WriteLine($"OK");
        Console.WriteLine($"Database: {reader.GetString(0)}");
        Console.WriteLine($"Login:    {reader.GetString(1)}");
    }
}
catch (SqlException ex)
{
    Console.WriteLine($"SQL ERROR Number: {ex.Number}");
    Console.WriteLine($"SQL ERROR State:  {ex.State}");
    Console.WriteLine($"SQL ERROR Class:  {ex.Class}");
    Console.WriteLine($"Message: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"{ex.GetType().FullName}: {ex.Message}");
}
