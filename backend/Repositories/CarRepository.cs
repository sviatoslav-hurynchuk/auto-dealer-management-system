using backend.Models;
using backend.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;

namespace backend.Repositories
{
    public class CarRepository : ICarRepository
    {
        private readonly string _connectionString;

        public CarRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // ==============================
        // GET ALL
        // ==============================
        public async Task<IEnumerable<Car>> GetAllAsync()
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT 
                    c.id,
                    c.MakeID,
                    c.Model,
                    c.Year,
                    c.Price,
                    c.Color,
                    c.VIN,
                    c.SupplierID,
                    c.Status
                FROM Cars c
                ORDER BY c.id DESC;
            ";

            return await connection.QueryAsync<Car>(sql);
        }

        // ==============================
        // GET BY ID
        // ==============================
        public async Task<Car?> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                SELECT 
                    id,
                    MakeID,
                    Model,
                    Year,
                    Price,
                    Color,
                    VIN,
                    SupplierID,
                    Status
                FROM Cars
                WHERE id = @Id;
            ";

            return await connection.QuerySingleOrDefaultAsync<Car>(sql, new { Id = id });
        }

        // ==============================
        // CREATE
        // ==============================
        public async Task<Car?> CreateAsync(Car car)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                INSERT INTO Cars
                (MakeID, Model, Year, Price, Color, VIN, SupplierID, Status)
                OUTPUT 
                    INSERTED.id,
                    INSERTED.MakeID,
                    INSERTED.Model,
                    INSERTED.Year,
                    INSERTED.Price,
                    INSERTED.Color,
                    INSERTED.VIN,
                    INSERTED.SupplierID,
                    INSERTED.Status
                VALUES
                (@MakeId, @Model, @Year, @Price, @Color, @Vin, @SupplierId, @Status);
            ";

            return await connection.QuerySingleOrDefaultAsync<Car>(sql, car);
        }

        // ==============================
        // UPDATE
        // ==============================
        public async Task<Car?> UpdateAsync(Car car)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                UPDATE Cars
                SET
                    MakeID = @MakeId,
                    Model = @Model,
                    Year = @Year,
                    Price = @Price,
                    Color = @Color,
                    VIN = @Vin,
                    SupplierID = @SupplierId,
                    Status = @Status
                OUTPUT 
                    INSERTED.id,
                    INSERTED.MakeID,
                    INSERTED.Model,
                    INSERTED.Year,
                    INSERTED.Price,
                    INSERTED.Color,
                    INSERTED.VIN,
                    INSERTED.SupplierID,
                    INSERTED.Status
                WHERE id = @Id;
            ";

            return await connection.QuerySingleOrDefaultAsync<Car>(sql, car);
        }

        // ==============================
        // DELETE
        // ==============================
        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"DELETE FROM Cars WHERE id = @Id";

            var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
            return affectedRows > 0;
        }
    }
}
