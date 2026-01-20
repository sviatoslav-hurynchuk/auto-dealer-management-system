using backend.Models;
using backend.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;

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
        // GET BY NAME
        // ==============================
        public async Task<Make?> GetMakeByNameAsync(string name)
        {
                        using var connection = _connectionFactory.CreateConnection();

            const string sql = @"
                SELECT id, Name
                FROM Makes
                WHERE Name = @Name;
            ";

            return await connection.QuerySingleOrDefaultAsync<Make>(sql, new { Name = name });
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
        // CREATE
        // ==============================
        public async Task<Make?> CreateMakeAsync(Make make)
        {
                        using var connection = _connectionFactory.CreateConnection();

            const string sql = @"
                INSERT INTO Makes (Name)
                OUTPUT INSERTED.id, INSERTED.Name
                VALUES (@Name);
            ";

            return await connection.QuerySingleOrDefaultAsync<Make>(sql, make);
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
        // DELETE
        // ==============================
        public async Task<bool> DeleteMakeAsync(int id)
        {
                        using var connection = _connectionFactory.CreateConnection();

            const string sql = @"DELETE FROM Makes WHERE id = @Id";

            var affected = await connection.ExecuteAsync(sql, new { Id = id });
            return affected > 0;
        }
    }
}
