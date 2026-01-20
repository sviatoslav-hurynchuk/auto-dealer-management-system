using backend.Models;
using backend.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;

namespace backend.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public EmployeeRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // ==============================
        // GET ALL
        // ==============================
        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
                        using var connection = _connectionFactory.CreateConnection();

            const string sql = @"
                SELECT
                    id,
                    FullName,
                    Position,
                    Phone,
                    Email,
                    IsActive
                FROM Employees
                ORDER BY id;
            ";

            return await connection.QueryAsync<Employee>(sql);
        }

        // ==============================
        // GET ALL ACTIVE
        // ==============================
        public async Task<IEnumerable<Employee>> GetAllActiveEmployeesAsync()
        {
                        using var connection = _connectionFactory.CreateConnection();

            const string sql = @"
                SELECT
                    id,
                    FullName,
                    Position,
                    Phone,
                    Email,
                    IsActive
                FROM Employees
                WHERE IsActive = 1
                ORDER BY id;
            ";

            return await connection.QueryAsync<Employee>(sql);
        }

        // ==============================
        // GET BY ID
        // ==============================
        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
                        using var connection = _connectionFactory.CreateConnection();

            const string sql = @"
                SELECT
                    id,
                    FullName,
                    Position,
                    Phone,
                    Email,
                    IsActive
                FROM Employees
                WHERE id = @Id;
            ";

            return await connection.QuerySingleOrDefaultAsync<Employee>(sql, new { Id = id });
        }
        public async Task<Employee?> GetEmployeeByEmailAsync(string email)
        {
                        using var connection = _connectionFactory.CreateConnection();

            const string sql = @"
        SELECT 
            Id,
            FullName,
            Position,
            Phone,
            Email,
            IsActive
        FROM Employees
        WHERE Email = @Email;
    ";

            return await connection.QuerySingleOrDefaultAsync<Employee>(sql, new { Email = email });
        }

        // ==============================
        // CREATE
        // ==============================
        public async Task<Employee?> CreateEmployeeAsync(Employee employee)
        {
                        using var connection = _connectionFactory.CreateConnection();

            const string sql = @"
                INSERT INTO Employees
                (FullName, Position, Phone, Email, IsActive)
                OUTPUT
                    INSERTED.id,
                    INSERTED.FullName,
                    INSERTED.Position,
                    INSERTED.Phone,
                    INSERTED.Email,
                    INSERTED.IsActive
                VALUES
                (@FullName, @Position, @Phone, @Email, 1);
            ";

            return await connection.QuerySingleOrDefaultAsync<Employee>(sql, employee);
        }

        // ==============================
        // UPDATE
        // ==============================
        public async Task<Employee?> UpdateEmployeeAsync(Employee employee)
        {
                        using var connection = _connectionFactory.CreateConnection();

            const string sql = @"
                UPDATE Employees
                SET
                    FullName = @FullName,
                    Position = @Position,
                    Phone = @Phone,
                    Email = @Email,
                    IsActive = @IsActive
                OUTPUT
                    INSERTED.id,
                    INSERTED.FullName,
                    INSERTED.Position,
                    INSERTED.Phone,
                    INSERTED.Email,
                    INSERTED.IsActive
                WHERE id = @Id;
            ";

            return await connection.QuerySingleOrDefaultAsync<Employee>(sql, employee);
        }

        // ==============================
        // HARD DELETE
        // ==============================
        public async Task<bool> DeleteEmployeeAsync(int id)
        {
                        using var connection = _connectionFactory.CreateConnection();

            const string sql = @"DELETE FROM Employees WHERE id = @Id";

            var affected = await connection.ExecuteAsync(sql, new { Id = id });
            return affected > 0;
        }

        // ==============================
        // DEACTIVATE (SOFT DELETE)
        // ==============================
        public async Task<bool> DeactivateEmployeeAsync(int id)
        {
                        using var connection = _connectionFactory.CreateConnection();

            const string sql = @"
                UPDATE Employees
                SET IsActive = 0
                WHERE id = @Id AND IsActive = 1;
            ";

            var affected = await connection.ExecuteAsync(sql, new { Id = id });
            return affected > 0;
        }

        // ==============================
        // EXISTS
        // ==============================
        public async Task<bool> ExistsByIdAsync(int id)
        {
                        using var connection = _connectionFactory.CreateConnection();

            const string sql = @"SELECT 1 FROM Employees WHERE id = @Id";

            var result = await connection.QueryFirstOrDefaultAsync<int?>(sql, new { Id = id });
            return result.HasValue;
        }
    }
}
