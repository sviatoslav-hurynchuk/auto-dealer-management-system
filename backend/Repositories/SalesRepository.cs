using backend.Services;
using Microsoft.Data.SqlClient;

namespace backend.Repositories
{
    public class SalesRepository
    {
        private readonly DbHelper _db;
        public SalesRepository(DbHelper db) { _db = db; }

        // Transaction
        public async Task SellCarAsync(int carId, int customerId, int employeeId, decimal finalPrice, CancellationToken token)
        {
            await using var conn = await _db.CreateOpenConnectionAsync(token);
            await using var transaction = (SqlTransaction)await conn.BeginTransactionAsync(token);

            try
            {
                // Status update
                var updateCarSql = "UPDATE Cars SET Status = 'Sold' WHERE id = @id";
                await using var cmd1 = new SqlCommand(updateCarSql, conn, transaction);
                cmd1.Parameters.AddWithValue("@id", carId);
                await cmd1.ExecuteNonQueryAsync(token);

                // Sale record insertion
                var insertSaleSql = @"
                    INSERT INTO Sales (CarID, CustomerID, EmployeeID, SaleDate, FinalPrice)
                    VALUES (@CarID, @CustID, @EmpID, GETDATE(), @Price)";

                await using var cmd2 = new SqlCommand(insertSaleSql, conn, transaction);
                cmd2.Parameters.AddWithValue("@CarID", carId);
                cmd2.Parameters.AddWithValue("@CustID", customerId);
                cmd2.Parameters.AddWithValue("@EmpID", employeeId);
                cmd2.Parameters.AddWithValue("@Price", finalPrice);
                await cmd2.ExecuteNonQueryAsync(token);

                // 3. Commit
                await transaction.CommitAsync(token);
                Console.WriteLine("Транзакція успішна: Авто продано.");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(token);
                Console.WriteLine("Помилка! Транзакцію відхилено.");
                throw;
            }
        }
    }
}