using Microsoft.Data.SqlClient;

public interface IDbConnectionFactory
{
    SqlConnection CreateConnection();
}
