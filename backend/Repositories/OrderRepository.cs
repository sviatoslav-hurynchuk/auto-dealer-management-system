using backend.Models;
using backend.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;

namespace backend.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;

        public OrderRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            const string sql = "SELECT * FROM Orders";

            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<Order>(sql);
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            const string sql = """
            SELECT * FROM Orders
            WHERE Id = @Id
            """;

            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Order>(sql, new { Id = id });
        }

        public async Task<Order?> CreateOrderAsync(Order order)
        {
            const string sql = """
            INSERT INTO Orders (SupplierId, CarId, OrderDate, Quantity, Status)
            VALUES (@SupplierId, @CarId, @OrderDate, @Quantity, @Status);

            SELECT * FROM Orders WHERE Id = SCOPE_IDENTITY();
            """;

            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Order>(sql, order);
        }

        public async Task<Order?> UpdateOrderAsync(Order order)
        {
            const string sql = """
            UPDATE Orders
            SET SupplierId = @SupplierId,
                CarId = @CarId,
                OrderDate = @OrderDate,
                Quantity = @Quantity,
                Status = @Status
            WHERE Id = @Id;

            SELECT * FROM Orders WHERE Id = @Id;
            """;

            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<Order>(sql, order);
        }

        public async Task<bool> DeleteOrderAsync(int id)
        {
            const string sql = """
            DELETE FROM Orders
            WHERE Id = @Id
            """;

            using var connection = new SqlConnection(_connectionString);
            var affected = await connection.ExecuteAsync(sql, new { Id = id });
            return affected > 0;
        }

        public async Task<bool> ExistsByCarIdAsync(int carId)
        {
            const string sql = """
            SELECT 1
            FROM Orders
            WHERE CarId = @CarId
            """;

            using var connection = new SqlConnection(_connectionString);
            var result = await connection.QueryFirstOrDefaultAsync<int?>(sql, new { CarId = carId });
            return result.HasValue;
        }
    }
}
