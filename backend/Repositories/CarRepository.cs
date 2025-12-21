using backend.Services;
using backend.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace backend.Repositories
{
    public class CarRepository
    {
        private readonly DbHelper _db;

        public CarRepository(DbHelper db)
        {
            _db = db;
        }
        // Create
        public async Task<int> CreateAsync(Car car, CancellationToken token = default)
        {
            await using var conn = await _db.CreateOpenConnectionAsync(token);

            await using var idCmd = new SqlCommand("SELECT ISNULL(MAX(id), 0) + 1 FROM Cars", conn);
            car.Id = Convert.ToInt32(await idCmd.ExecuteScalarAsync(token));

            const string sql = @"
                INSERT INTO Cars (id, MakeID, Model, Year, Price, Color, VIN, SupplierID, Status)
                VALUES (@id, @MakeID, @Model, @Year, @Price, @Color, @VIN, @SupplierID, @Status);";

            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", car.Id);
            cmd.Parameters.AddWithValue("@MakeID", car.MakeID);
            cmd.Parameters.AddWithValue("@Model", car.Model);
            cmd.Parameters.AddWithValue("@Year", car.Year);
            cmd.Parameters.AddWithValue("@Price", car.Price);
            cmd.Parameters.AddWithValue("@Color", car.Color ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@VIN", car.VIN);
            cmd.Parameters.AddWithValue("@SupplierID", car.SupplierID ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", car.Status);

            await cmd.ExecuteNonQueryAsync(token);
            return car.Id;
        }
        // Read
        public async Task<Car?> GetByIdAsync(int id, CancellationToken token = default)
        {
            const string sql = "SELECT * FROM Cars WHERE id = @id";
            await using var conn = await _db.CreateOpenConnectionAsync(token);
            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            await using var reader = await cmd.ExecuteReaderAsync(token);
            if (!await reader.ReadAsync(token)) return null;

            return MapCar(reader);
        }
        // Update
        public async Task UpdateAsync(Car car, CancellationToken token = default)
        {
            const string sql = @"
                UPDATE Cars 
                SET Price = @Price, Status = @Status, Color = @Color
                WHERE id = @id";

            await using var conn = await _db.CreateOpenConnectionAsync(token);
            await using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@Price", car.Price);
            cmd.Parameters.AddWithValue("@Status", car.Status);
            cmd.Parameters.AddWithValue("@Color", car.Color ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@id", car.Id);

            await cmd.ExecuteNonQueryAsync(token);
        }

        // Pagination with Sorting
        public async Task<(List<Car> Cars, int TotalCount)> GetPagedAsync(
            int pageNumber, int pageSize, string sortBy, CancellationToken token = default)
        {
            var allowedSort = new[] { "Price", "Year", "Model" };
            var orderBy = allowedSort.Contains(sortBy) ? sortBy : "id";

            var sql = $@"
                SELECT * FROM Cars
                ORDER BY {orderBy} ASC
                OFFSET @Offset ROWS FETCH NEXT @Size ROWS ONLY;
                
                SELECT COUNT(*) FROM Cars;";

            await using var conn = await _db.CreateOpenConnectionAsync(token);
            await using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Offset", (pageNumber - 1) * pageSize);
            cmd.Parameters.AddWithValue("@Size", pageSize);

            var list = new List<Car>();
            await using var reader = await cmd.ExecuteReaderAsync(token);

            while (await reader.ReadAsync(token))
            {
                list.Add(MapCar(reader));
            }

            await reader.NextResultAsync(token);
            int total = 0;
            if (await reader.ReadAsync(token)) total = reader.GetInt32(0);

            return (list, total);
        }
        // Complex Query with Joins
        public async Task<List<string>> GetCarsWithDetailsAsync(CancellationToken token = default)
        {
            const string sql = @"
                SELECT c.Model, c.VIN, m.Name as Make, s.CompanyName as Supplier
                FROM Cars c
                JOIN Makes m ON c.MakeID = m.id
                LEFT JOIN Suppliers s ON c.SupplierID = s.id";

            var result = new List<string>();
            await using var conn = await _db.CreateOpenConnectionAsync(token);
            await using var cmd = new SqlCommand(sql, conn);
            await using var reader = await cmd.ExecuteReaderAsync(token);

            while (await reader.ReadAsync(token))
            {
                var supplier = reader.IsDBNull(3) ? "No Supplier" : reader.GetString(3);
                result.Add($"{reader.GetString(2)} {reader.GetString(0)} ({reader.GetString(1)}) - Supplied by: {supplier}");
            }
            return result;
        }
        // Delete
        public async Task DeleteAsync(int id, CancellationToken token = default)
        {
            const string sql = "DELETE FROM Cars WHERE id = @id";

            await using var conn = await _db.CreateOpenConnectionAsync(token);
            await using var cmd = new SqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@id", id);
            await cmd.ExecuteNonQueryAsync(token);
        }
        // Mapper
        private static Car MapCar(SqlDataReader reader)
        {
            return new Car
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                MakeID = reader.GetInt32(reader.GetOrdinal("MakeID")),
                Model = reader.GetString(reader.GetOrdinal("Model")),
                Year = reader.GetInt32(reader.GetOrdinal("Year")),
                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                Color = reader.IsDBNull(reader.GetOrdinal("Color")) ? null : reader.GetString(reader.GetOrdinal("Color")),
                VIN = reader.GetString(reader.GetOrdinal("VIN")),
                SupplierID = reader.IsDBNull(reader.GetOrdinal("SupplierID")) ? null : reader.GetInt32(reader.GetOrdinal("SupplierID")),
                Status = reader.GetString(reader.GetOrdinal("Status"))
            };
        }

        //Async & Cancellation
public async Task SimulateLongProcessAsync(int seconds, CancellationToken token = default)
        {
            string sql = $"WAITFOR DELAY '00:00:{seconds:D2}'";

            await using var conn = await _db.CreateOpenConnectionAsync(token);
            await using var cmd = new SqlCommand(sql, conn);

            // Встановлюємо таймаут команди трохи більшим за час очікування, 
            // щоб спрацював саме наш CancellationToken, а не таймаут SQL клієнта
            cmd.CommandTimeout = seconds + 5;

            await cmd.ExecuteNonQueryAsync(token);
        }
    }
}