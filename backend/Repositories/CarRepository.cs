using backend.Models;
using backend.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Text;

namespace backend.Repositories
{
    public class CarRepository : ICarRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public CarRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // ======================
        // SEARCH
        // ======================
        public async Task<List<Car>> SearchCarsAsync(CarSearchParams filter)
        {
            var cars = new List<Car>();

            using var connection = _connectionFactory.CreateConnection();
            using var cmd = connection.CreateCommand();

            var sql = new StringBuilder(@"
        SELECT Id, MakeId, Model, Year, Price, VIN, Status
        FROM Cars
        WHERE 1 = 1
    ");

            // 🔹 FILTERS
            if (filter.MakeId.HasValue)
            {
                sql.Append(" AND MakeId = @MakeId");
                cmd.Parameters.AddWithValue("@MakeId", filter.MakeId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Model))
            {
                sql.Append(" AND Model LIKE @Model");
                cmd.Parameters.AddWithValue("@Model", $"%{filter.Model}%");
            }

            if (filter.PriceFrom.HasValue)
            {
                sql.Append(" AND Price >= @PriceFrom");
                cmd.Parameters.AddWithValue("@PriceFrom", filter.PriceFrom.Value);
            }

            if (filter.PriceTo.HasValue)
            {
                sql.Append(" AND Price <= @PriceTo");
                cmd.Parameters.AddWithValue("@PriceTo", filter.PriceTo.Value);
            }

            if (filter.YearFrom.HasValue)
            {
                sql.Append(" AND Year >= @YearFrom");
                cmd.Parameters.AddWithValue("@YearFrom", filter.YearFrom.Value);
            }

            if (filter.YearTo.HasValue)
            {
                sql.Append(" AND Year <= @YearTo");
                cmd.Parameters.AddWithValue("@YearTo", filter.YearTo.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                sql.Append(" AND Status = @Status");
                cmd.Parameters.AddWithValue("@Status", filter.Status);
            }

            // 🔹 SORT (WHITELIST!)
            var sortColumn = filter.SortBy?.ToLower() switch
            {
                "price" => "Price",
                "year" => "Year",
                "model" => "Model",
                _ => "Id"
            };

            var sortDir = filter.SortDirection?.ToLower() == "desc"
                ? "DESC"
                : "ASC";

            sql.Append($" ORDER BY {sortColumn} {sortDir}");

            // 🔹 PAGINATION
            sql.Append(" OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY");

            var page = filter.Page > 0 ? filter.Page : 1;
            var pageSize = filter.PageSize > 0 ? filter.PageSize : 20;

            cmd.Parameters.AddWithValue("@Offset", (page - 1) * pageSize);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);


            cmd.CommandText = sql.ToString();

            await connection.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                cars.Add(new Car
                {
                    Id = reader.GetInt32(0),
                    MakeId = reader.GetInt32(1),
                    Model = reader.GetString(2),
                    Year = reader.GetInt32(3),
                    Price = reader.GetDecimal(4),
                    Vin = reader.GetString(5),
                    Status = reader.GetString(6)
                });
            }

            return cars;
        }



        // ==============================
        // GET ALL
        // ==============================
        public async Task<IEnumerable<Car>> GetAllCarsAsync()
        {
            using var connection = _connectionFactory.CreateConnection();

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
            using var connection = _connectionFactory.CreateConnection();

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

                        using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<CarWithStats>(sql);
        }



        // ==============================
        // CREATE
        // ==============================
        public async Task<Car?> CreateCarAsync(Car car)
        {
                        using var connection = _connectionFactory.CreateConnection();

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
                        using var connection = _connectionFactory.CreateConnection();

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
                        using var connection = _connectionFactory.CreateConnection();

            const string sql = @"DELETE FROM Cars WHERE id = @Id";

            var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
            return affectedRows > 0;
        }

        public async Task<bool> ExistsByMakeIdAsync(int makeId)
        {
                        using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT 1 FROM Cars WHERE MakeId = @MakeId";
            var result = await connection.QueryFirstOrDefaultAsync<int?>(sql, new { MakeId = makeId });
            return result.HasValue;
        }

        public async Task<bool> VINExistsAsync(string vin)
        {
                        using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT 1 FROM Cars WHERE VIN = @Vin";
            var result = await connection.QueryFirstOrDefaultAsync<int?>(sql, new { Vin = vin });
            return result.HasValue;
        }

    }
}
