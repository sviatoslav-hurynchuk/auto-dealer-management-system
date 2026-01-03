using backend.Models;

namespace backend.Repositories.Interfaces
{
    public interface ICarRepository
    {
        Task<IEnumerable<Car>> GetAllAsync();
        Task<Car?> GetByIdAsync(int id);
        Task<Car?> CreateAsync(Car car);
        Task<Car?> UpdateAsync(Car car);
        Task<bool> DeleteAsync(int id);
    }
}
