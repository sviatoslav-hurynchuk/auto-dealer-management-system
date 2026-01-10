using backend.Models;
using backend.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;

namespace backend.Repositories
{
    public class SaleRepository : ISaleRepository
    {
        private readonly string _connectionString;

        public SaleRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // ==============================
        // GET ALL
        // ==============================
        public async Task<IEnumerable<Sale>> GetAllSalesAsync()
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT
                    Id,
                    CarId,
                    CustomerId,
                    EmployeeId,
                    SaleDate,
                    FinalPrice,
                    Status
                FROM Sales
                ORDER BY Id DESC;
            ";

            return await connection.QueryAsync<Sale>(sql);
        }

        // ==============================
        // GET BY ID
        // ==============================
        public async Task<Sale?> GetSaleByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT
                    Id,
                    CarId,
                    CustomerId,
                    EmployeeId,
                    SaleDate,
                    FinalPrice,
                    Status
                FROM Sales
                WHERE Id = @Id;
            ";

            return await connection.QuerySingleOrDefaultAsync<Sale>(sql, new { Id = id });
        }

        // ==============================
        // CREATE
        // ==============================
        public async Task<Sale?> CreateSaleAsync(Sale sale)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                INSERT INTO Sales
                (CarId, CustomerId, EmployeeId, SaleDate, FinalPrice, Status)
                OUTPUT
                    INSERTED.Id,
                    INSERTED.CarId,
                    INSERTED.CustomerId,
                    INSERTED.EmployeeId,
                    INSERTED.SaleDate,
                    INSERTED.FinalPrice,
                    INSERTED.Status
                VALUES
                (@CarId, @CustomerId, @EmployeeId, @SaleDate, @FinalPrice, @Status);
            ";

            return await connection.QuerySingleOrDefaultAsync<Sale>(sql, sale);
        }

        // ==============================
        // UPDATE
        // ==============================
        public async Task<Sale?> UpdateSaleAsync(Sale sale)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                UPDATE Sales
                SET
                    CarId = @CarId,
                    CustomerId = @CustomerId,
                    EmployeeId = @EmployeeId,
                    SaleDate = @SaleDate,
                    FinalPrice = @FinalPrice,
                    Status = @Status
                OUTPUT
                    INSERTED.Id,
                    INSERTED.CarId,
                    INSERTED.CustomerId,
                    INSERTED.EmployeeId,
                    INSERTED.SaleDate,
                    INSERTED.FinalPrice,
                    INSERTED.Status
                WHERE Id = @Id;
            ";

            return await connection.QuerySingleOrDefaultAsync<Sale>(sql, sale);
        }

        // ==============================
        // DELETE
        // ==============================
        public async Task<bool> DeleteSaleAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"DELETE FROM Sales WHERE Id = @Id";

            var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
            return affectedRows > 0;
        }

        public async Task<bool> ExistsByCarIdAsync(int carId)
        {
            const string sql = """
            SELECT 1
            FROM Sales
            WHERE CarId = @CarId
            """;

            using var connection = new SqlConnection(_connectionString);
            var result = await connection.QueryFirstOrDefaultAsync<int?>(sql, new { CarId = carId });
            return result.HasValue;
        }

        public async Task<bool> ExistsByEmployeeIdAsync(int employeeId)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
        SELECT 1
        FROM Sales
        WHERE EmployeeId = @EmployeeId
    ";

            var result = await connection.QueryFirstOrDefaultAsync<int?>(
                sql,
                new { EmployeeId = employeeId }
            );

            return result.HasValue;
        }

        public async Task<bool> ExistsByCustomerIdAsync(int customerId)
        {
            const string sql = """
        SELECT 1
        FROM Sales
        WHERE CustomerId = @CustomerId
    """;

            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<int?>(
                sql,
                new { CustomerId = customerId }
            ) != null;
        }

    }
}
