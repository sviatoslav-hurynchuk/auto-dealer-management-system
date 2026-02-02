using backend.Models;
using backend.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace backend.Repositories
{
    public class MakeRepository : IMakeRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public MakeRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // ==============================
        // GET ALL
        // ==============================
        public async Task<IEnumerable<Make>> GetAllMakesAsync()
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = @"
                SELECT id, Name
                FROM Makes
                ORDER BY id;
            ";

            return await connection.QueryAsync<Make>(sql);
        }

        // ==============================
        // GET BY ID
        // ==============================
        public async Task<Make?> GetMakeByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = @"
                SELECT id, Name
                FROM Makes
                WHERE id = @Id;
            ";

            return await connection.QuerySingleOrDefaultAsync<Make>(sql, new { Id = id });
        }

        // ==============================
        // GET BY NAME (UPDATED FOR TRANSACTION)
        // ==============================
        public async Task<Make?> GetMakeByNameAsync(string name, IDbTransaction? transaction = null)
        {
            // 1. Connection management
            var connection = transaction?.Connection ?? _connectionFactory.CreateConnection();

            const string sql = @"
                SELECT id, Name
                FROM Makes
                WHERE Name = @Name;
            ";

            try
            {
                // 2. Pass transaction
                return await connection.QuerySingleOrDefaultAsync<Make>(sql, new { Name = name }, transaction: transaction);
            }
            finally
            {
                // 3. Dispose if we created it
                if (transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        // ==============================
        // EXISTS
        // ==============================
        public async Task<bool> ExistsByIdAsync(int id)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = @"SELECT 1 FROM Makes WHERE id = @Id";

            var result = await connection.QueryFirstOrDefaultAsync<int?>(sql, new { Id = id });
            return result.HasValue;
        }

        // ==============================
        // CREATE (UPDATED FOR TRANSACTION)
        // ==============================
        public async Task<Make?> CreateMakeAsync(Make make, IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? _connectionFactory.CreateConnection();

            const string sql = @"
                INSERT INTO Makes (Name)
                OUTPUT INSERTED.id, INSERTED.Name
                VALUES (@Name);
            ";

            try
            {
                return await connection.QuerySingleOrDefaultAsync<Make>(sql, make, transaction: transaction);
            }
            finally
            {
                if (transaction == null)
                {
                    connection.Dispose();
                }
            }
        }

        // ==============================
        // UPDATE
        // ==============================
        public async Task<Make?> UpdateMakeAsync(Make make)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = @"
                UPDATE Makes
                SET Name = @Name
                OUTPUT INSERTED.id, INSERTED.Name
                WHERE id = @Id;
            ";

            return await connection.QuerySingleOrDefaultAsync<Make>(sql, make);
        }

        // ==============================
        // DELETE (UPDATED FOR TRANSACTION)
        // ==============================
        public async Task<bool> DeleteMakeAsync(int id, IDbTransaction? transaction = null)
        {
            var connection = transaction?.Connection ?? _connectionFactory.CreateConnection();

            const string sql = @"DELETE FROM Makes WHERE id = @Id";

            try
            {
                var affected = await connection.ExecuteAsync(sql, new { Id = id }, transaction: transaction);
                return affected > 0;
            }
            finally
            {
                if (transaction == null)
                {
                    connection.Dispose();
                }
            }
        }
    }
}