using backend.Models;

namespace backend.Repositories.Interfaces
{
    public interface ISaleRepository
    {
        Task<IEnumerable<Sale>> GetAllSalesAsync();
        Task<Sale?> GetSaleByIdAsync(int id);
        Task<Sale?> CreateSaleAsync(Sale sale);
        Task<Sale?> UpdateSaleAsync(Sale sale);
        Task<bool> DeleteSaleAsync(int id);
    }
}
