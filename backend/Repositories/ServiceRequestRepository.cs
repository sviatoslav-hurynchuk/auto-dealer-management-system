using backend.Models;
using backend.Repositories.Interfaces;
using Dapper;
using Microsoft.Data.SqlClient;

namespace backend.Repositories
{
    public class ServiceRequestRepository : IServiceRequestRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ServiceRequestRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // ==============================
        // GET ALL
        // ==============================
        public async Task<IEnumerable<ServiceRequest>> GetAllRequestsAsync()
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = """
                SELECT
                    Id,
                    CarId,
                    ServiceType,
                    Status,
                    UpdatedAt
                FROM ServiceRequests
                ORDER BY Id;
            """;

            return await connection.QueryAsync<ServiceRequest>(sql);
        }

        // ==============================
        // GET BY ID
        // ==============================
        public async Task<ServiceRequest?> GetRequestByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = """
                SELECT
                    Id,
                    CarId,
                    ServiceType,
                    Status,
                    UpdatedAt
                FROM ServiceRequests
                WHERE Id = @Id;
            """;

            return await connection.QuerySingleOrDefaultAsync<ServiceRequest>(
                sql,
                new { Id = id }
            );
        }

        // ==============================
        // GET BY CAR ID
        // ==============================
        public async Task<IEnumerable<ServiceRequest>> GetRequestsByCarIdAsync(int carId)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = """
                SELECT
                    Id,
                    CarId,
                    ServiceType,
                    Status,
                    UpdatedAt
                FROM ServiceRequests
                WHERE CarId = @CarId
                ORDER BY Id DESC;
            """;

            return await connection.QueryAsync<ServiceRequest>(
                sql,
                new { CarId = carId }
            );
        }

        // ==============================
        // CREATE
        // ==============================
        public async Task<ServiceRequest?> CreateRequestAsync(ServiceRequest request)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = """
                INSERT INTO ServiceRequests
                (CarId, ServiceType, Status)
                OUTPUT
                    INSERTED.Id,
                    INSERTED.CarId,
                    INSERTED.ServiceType,
                    INSERTED.Status,
                    INSERTED.UpdatedAt
                VALUES
                (@CarId, @ServiceType, @Status);
            """;

            return await connection.QuerySingleOrDefaultAsync<ServiceRequest>(sql, request);
        }

        // ==============================
        // UPDATE
        // ==============================
        public async Task<ServiceRequest?> UpdateRequestAsync(ServiceRequest request)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = """
                UPDATE ServiceRequests
                SET
                    ServiceType = @ServiceType,
                    Status = @Status,
                    UpdatedAt = GETDATE()
                OUTPUT
                    INSERTED.Id,
                    INSERTED.CarId,
                    INSERTED.ServiceType,
                    INSERTED.Status,
                    INSERTED.UpdatedAt
                WHERE Id = @Id;
            """;

            return await connection.QuerySingleOrDefaultAsync<ServiceRequest>(sql, request);
        }

        // ==============================
        // DELETE
        // ==============================
        public async Task<bool> DeleteRequestAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);

            const string sql = "DELETE FROM ServiceRequests WHERE Id = @Id";

            var affected = await connection.ExecuteAsync(sql, new { Id = id });
            return affected > 0;
        }
    }
}
