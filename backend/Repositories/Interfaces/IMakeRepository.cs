using backend.Models;

namespace backend.Repositories.Interfaces
{
    public interface IMakeRepository
    {
        Task<IEnumerable<Make>> GetAllMakesAsync();
        Task<Make?> GetMakeByIdAsync(int id);
        Task<Make?> GetMakeByNameAsync(string name);
        Task<Make?> CreateMakeAsync(Make make);
        Task<Make?> UpdateMakeAsync(Make make);
        Task<bool> DeleteMakeAsync(int id);
        Task<bool> ExistsByIdAsync(int id);
    }
}
