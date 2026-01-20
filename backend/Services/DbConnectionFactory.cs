using Microsoft.Data.SqlClient;

public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly IConfiguration _config;

    public DbConnectionFactory(IConfiguration config)
    {
        _config = config;
    }

    public SqlConnection CreateConnection()
    {
        var role = _config["DbRole"] ?? "Reader";
        var connectionString = _config.GetConnectionString(role);

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                $"No connection string configured for DB role '{role}'");

        return new SqlConnection(connectionString);
    }
}
