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
        public async Task<IEnumerable<Car>> GetAllCarsAsync()
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
                    Description,
                    ImageUrl,
                    Condition,
                    Mileage,
                    BodyType,
                    Status
                FROM Cars
                ORDER BY id;
            ";

            return await connection.QueryAsync<Car>(sql);
        }

        // ==============================
        // GET BY ID
        // ==============================
        public async Task<Car?> GetCarByIdAsync(int id)
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
                    Description,
                    ImageUrl,
                    Condition,
                    Mileage,
                    BodyType,
                    Status
                FROM Cars
                WHERE id = @Id;
            ";

            return await connection.QuerySingleOrDefaultAsync<Car>(sql, new { Id = id });
        }

        // ==============================
        // GET CARS WITH SUPPLIER AND SALES INFO
        // ==============================
        public async Task<IEnumerable<CarWithStats>> GetCarsWithStatsAsync()
        {
            var sql = @"
        SELECT 
            c.id,
            c.Model,
            c.VIN,
            c.Price,
            c.Status,
            ISNULL(s.CompanyName, '') AS SupplierName,
            (SELECT COUNT(*) FROM Orders o WHERE o.CarID = c.id) AS OrdersCount,
            (SELECT MAX(saleDate) FROM Sales sa WHERE sa.CarID = c.id) AS LastSaleDate
        FROM Cars c
        LEFT JOIN Suppliers s ON c.SupplierID = s.id
        ORDER BY c.id;
    ";

            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<CarWithStats>(sql);
        }



        // ==============================
        // CREATE
        // ==============================
        public async Task<Car?> CreateCarAsync(Car car)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"
                INSERT INTO Cars
                (MakeID, Model, Year, Price, Color, VIN, SupplierID, Description, ImageUrl, Condition, Mileage, BodyType, Status)
                OUTPUT 
                    INSERTED.id,
                    INSERTED.MakeID,
                    INSERTED.Model,
                    INSERTED.Year,
                    INSERTED.Price,
                    INSERTED.Color,
                    INSERTED.VIN,
                    INSERTED.SupplierID,
                    INSERTED.Description,
                    INSERTED.ImageUrl,
                    INSERTED.Condition,
                    INSERTED.Mileage,
                    INSERTED.BodyType,
                    INSERTED.Status
                VALUES
                (@MakeId, @Model, @Year, @Price, @Color, @Vin, @SupplierId, @Description, @ImageUrl, @Condition, @Mileage, @BodyType, @Status);
            ";

            return await connection.QuerySingleOrDefaultAsync<Car>(sql, car);
        }

        // ==============================
        // UPDATE
        // ==============================
        public async Task<Car?> UpdateCarAsync(Car car)
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
                    Description = @Description,
                    ImageUrl = @ImageUrl,
                    Condition = @Condition,
                    Mileage = @Mileage,
                    BodyType = @BodyType,
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
                    INSERTED.Description,
                    INSERTED.ImageUrl,
                    INSERTED.Condition,
                    INSERTED.Mileage,
                    INSERTED.BodyType,
                    INSERTED.Status
                WHERE id = @Id;
            ";

            return await connection.QuerySingleOrDefaultAsync<Car>(sql, car);
        }

        // ==============================
        // DELETE
        // ==============================
        public async Task<bool> DeleteCarAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = @"DELETE FROM Cars WHERE id = @Id";

            var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
            return affectedRows > 0;
        }

        public async Task<bool> ExistsByMakeIdAsync(int makeId)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = "SELECT 1 FROM Cars WHERE MakeId = @MakeId";
            var result = await connection.QueryFirstOrDefaultAsync<int?>(sql, new { MakeId = makeId });
            return result.HasValue;
        }

        public async Task<bool> VINExistsAsync(string vin)
        {
            using var connection = new SqlConnection(_connectionString);
            const string sql = "SELECT 1 FROM Cars WHERE VIN = @Vin";
            var result = await connection.QueryFirstOrDefaultAsync<int?>(sql, new { Vin = vin });
            return result.HasValue;
        }

    }
}
