using backend.Models;
using backend.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;

namespace backend.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly string _connectionString;

        public SupplierRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Supplier>> GetAllSuppliersAsync()
        {
            const string sql = "SELECT * FROM Suppliers";
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Supplier>(sql);
        }

        public async Task<Supplier?> GetSupplierByIdAsync(int? id)
        {
            const string sql = "SELECT * FROM Suppliers WHERE Id = @Id";
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Supplier>(sql, new { Id = id });
        }

        public async Task<Supplier?> GetSupplierByCompanyNameAsync(string companyName)
        {
            const string sql = "SELECT * FROM Suppliers WHERE CompanyName = @CompanyName";
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Supplier>(sql, new { CompanyName = companyName });
        }

        public async Task<Supplier> CreateSupplierAsync(Supplier supplier)
        {
            const string sql = """
                INSERT INTO Suppliers (CompanyName, ContactName, Phone, Email)
                OUTPUT INSERTED.*
                VALUES (@CompanyName, @ContactName, @Phone, @Email)
            """;

            using var connection = new SqlConnection(_connectionString);
            return await connection.QuerySingleAsync<Supplier>(sql, supplier);
        }

        public async Task<Supplier> UpdateSupplierAsync(Supplier supplier)
        {
            const string sql = """
                UPDATE Suppliers
                SET CompanyName = @CompanyName,
                    ContactName = @ContactName,
                    Phone = @Phone,
                    Email = @Email
                OUTPUT INSERTED.*
                WHERE Id = @Id
            """;

            using var connection = new SqlConnection(_connectionString);
            return await connection.QuerySingleAsync<Supplier>(sql, supplier);
        }

        public async Task<bool> DeleteSupplierAsync(int id)
        {
            const string sql = "DELETE FROM Suppliers WHERE Id = @Id";
            using var connection = new SqlConnection(_connectionString);
            return await connection.ExecuteAsync(sql, new { Id = id }) > 0;
        }

        public async Task<bool> ExistsByIdAsync(int? id)
        {
            const string sql = "SELECT 1 FROM Suppliers WHERE Id = @Id";
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<int?>(sql, new { Id = id }) != null;
        }

        public async Task<bool> HasOrdersAsync(int supplierId)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
            SELECT CASE WHEN EXISTS (
                SELECT 1
                FROM Orders
                WHERE SupplierId = @SupplierId
            ) THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END
        ";

            return await connection.QuerySingleAsync<bool>(sql, new { SupplierId = supplierId });
        }

        // ==============================
        // Перевірка на наявність машин
        // ==============================
        public async Task<bool> HasCarsAsync(int supplierId)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
            SELECT CASE WHEN EXISTS (
                SELECT 1
                FROM Cars
                WHERE SupplierId = @SupplierId
            ) THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END
        ";

            return await connection.QuerySingleAsync<bool>(sql, new { SupplierId = supplierId });
        }
    }
}
