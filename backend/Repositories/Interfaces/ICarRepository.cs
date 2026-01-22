using backend.Models;

namespace backend.Repositories.Interfaces
{
    public interface ICarRepository
    {
        Task<List<Car>> SearchCarsAsync(CarSearchParams filter);
        Task<IEnumerable<Car>> GetAllCarsAsync();
        Task<Car?> GetCarByIdAsync(int id);
        Task<IEnumerable<CarWithStats>> GetCarsWithStatsAsync();
        Task<Car?> CreateCarAsync(Car car);
        Task<Car?> UpdateCarAsync(Car car);
        Task<bool> DeleteCarAsync(int id);
        Task<bool> ExistsByMakeIdAsync(int makeId);
        Task<bool> VINExistsAsync(string vin);
    }
}
