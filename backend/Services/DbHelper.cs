using Microsoft.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace backend.Services
{
    public class DbHelper
    {
        private readonly string _connectionString;

        public DbHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<SqlConnection> CreateOpenConnectionAsync(CancellationToken token = default)
        {
            var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(token);
            return connection;
        }
    }
}