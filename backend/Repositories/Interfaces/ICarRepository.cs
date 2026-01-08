using backend.Models;

namespace backend.Repositories.Interfaces
{
    public interface ICarRepository
    {
        Task<IEnumerable<Car>> GetAllCarsAsync();
        Task<Car?> GetCarByIdAsync(int id);
        Task<Car?> CreateCarAsync(Car car);
        Task<Car?> UpdateCarAsync(Car car);
        Task<bool> DeleteCarAsync(int id);
        Task<bool> ExistsByMakeIdAsync(int makeId);
    }
}
