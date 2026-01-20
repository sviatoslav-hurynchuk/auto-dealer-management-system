using backend.Models;
using backend.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;

namespace backend.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        public CustomerRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            const string sql = "SELECT * FROM Customers";
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryAsync<Customer>(sql);
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            const string sql = "SELECT * FROM Customers WHERE Id = @Id";
            using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Customer>(sql, new { Id = id });
        }

        public async Task<Customer?> GetCustomerByEmailAsync(string email)
        {
            const string sql = "SELECT * FROM Customers WHERE Email = @Email";
                        using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Customer>(sql, new { Email = email });
        }

        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            const string sql = """
                INSERT INTO Customers (FullName, Phone, Email, Address)
                OUTPUT INSERTED.*
                VALUES (@FullName, @Phone, @Email, @Address)
            """;

                        using var connection = _connectionFactory.CreateConnection();
            return await connection.QuerySingleAsync<Customer>(sql, customer);
        }

        public async Task<Customer> UpdateCustomerAsync(Customer customer)
        {
            const string sql = """
                UPDATE Customers
                SET FullName = @FullName,
                    Phone = @Phone,
                    Email = @Email,
                    Address = @Address
                OUTPUT INSERTED.*
                WHERE Id = @Id
            """;

                        using var connection = _connectionFactory.CreateConnection();
            return await connection.QuerySingleAsync<Customer>(sql, customer);
        }

        public async Task<bool> DeleteCustomerAsync(int id)
        {
            const string sql = "DELETE FROM Customers WHERE Id = @Id";
                        using var connection = _connectionFactory.CreateConnection();
            return await connection.ExecuteAsync(sql, new { Id = id }) > 0;
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            const string sql = "SELECT 1 FROM Customers WHERE Id = @Id";
                        using var connection = _connectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<int?>(sql, new { Id = id }) != null;
        }
    }
}
