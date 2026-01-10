using backend.Models;

namespace backend.Repositories.Interfaces
{
    public interface IServiceRequestRepository
    {
        Task<IEnumerable<ServiceRequest>> GetAllAsync();
        Task<ServiceRequest?> GetByIdAsync(int id);
        Task<IEnumerable<ServiceRequest>> GetByCarIdAsync(int carId);

        Task<ServiceRequest?> CreateAsync(ServiceRequest request);
        Task<ServiceRequest?> UpdateAsync(ServiceRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
