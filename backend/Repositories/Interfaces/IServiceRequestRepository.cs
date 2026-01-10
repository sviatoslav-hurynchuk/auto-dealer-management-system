using backend.Models;

namespace backend.Repositories.Interfaces
{
    public interface IServiceRequestRepository
    {
        Task<IEnumerable<ServiceRequest>> GetAllRequestsAsync();
        Task<ServiceRequest?> GetRequestByIdAsync(int id);
        Task<IEnumerable<ServiceRequest>> GetRequestsByCarIdAsync(int carId);

        Task<ServiceRequest?> CreateRequestAsync(ServiceRequest request);
        Task<ServiceRequest?> UpdateRequestAsync(ServiceRequest request);
        Task<bool> DeleteRequestAsync(int id);
    }
}
