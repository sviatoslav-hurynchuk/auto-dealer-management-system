using backend.Models;
using backend.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;

namespace backend.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly string _connectionString;

        public EmployeeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // ==============================
        // GET ALL
        // ==============================
        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT
                    id,
                    FullName,
                    Position,
                    Phone,
                    Email
                FROM Employees
                ORDER BY id DESC;
            ";

            return await connection.QueryAsync<Employee>(sql);
        }

        // ==============================
        // GET BY ID
        // ==============================
        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT
                    id,
                    FullName,
                    Position,
                    Phone,
                    Email
                FROM Employees
                WHERE id = @Id;
            ";

            return await connection.QuerySingleOrDefaultAsync<Employee>(sql, new { Id = id });
        }

        // ==============================
        // CREATE
        // ==============================
        public async Task<Employee?> CreateEmployeeAsync(Employee employee)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                INSERT INTO Employees
                (FullName, Position, Phone, Email)
                OUTPUT
                    INSERTED.id,
                    INSERTED.FullName,
                    INSERTED.Position,
                    INSERTED.Phone,
                    INSERTED.Email
                VALUES
                (@FullName, @Position, @Phone, @Email);
            ";

            return await connection.QuerySingleOrDefaultAsync<Employee>(sql, employee);
        }

        // ==============================
        // UPDATE
        // ==============================
        public async Task<Employee?> UpdateEmployeeAsync(Employee employee)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                UPDATE Employees
                SET
                    FullName = @FullName,
                    Position = @Position,
                    Phone = @Phone,
                    Email = @Email
                OUTPUT
                    INSERTED.id,
                    INSERTED.FullName,
                    INSERTED.Position,
                    INSERTED.Phone,
                    INSERTED.Email
                WHERE id = @Id;
            ";

            return await connection.QuerySingleOrDefaultAsync<Employee>(sql, employee);
        }

        // ==============================
        // DELETE
        // ==============================
        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"DELETE FROM Employees WHERE id = @Id";

            var affected = await connection.ExecuteAsync(sql, new { Id = id });
            return affected > 0;
        }

        // ==============================
        // EXISTS
        // ==============================
        public async Task<bool> ExistsByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"SELECT 1 FROM Employees WHERE id = @Id";

            var result = await connection.QueryFirstOrDefaultAsync<int?>(sql, new { Id = id });
            return result.HasValue;
        }
    }
}
