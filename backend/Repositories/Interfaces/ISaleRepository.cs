using backend.Models;

namespace backend.Repositories.Interfaces
{
    public interface ISaleRepository
    {
        Task<IEnumerable<Sale>> GetAllSalesAsync();
        Task<Sale?> GetSaleByIdAsync(int id);
        Task<IEnumerable<EmployeeSalesStats>> GetEmployeeSalesStatsAsync();
        Task<Sale?> CreateSaleAsync(Sale sale);
        Task<Sale?> UpdateSaleAsync(Sale sale);
        Task<bool> DeleteSaleAsync(int id);
        Task<bool> ExistsByCarIdAsync(int carId);
        Task<bool> ExistsByEmployeeIdAsync(int employeeId);
        Task<bool> ExistsByCustomerIdAsync(int customerId);
    }
}
