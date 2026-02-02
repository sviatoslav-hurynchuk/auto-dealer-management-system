using backend.Models;
using System.Data;

namespace backend.Repositories.Interfaces
{
    public interface IMakeRepository
    {
        Task<IEnumerable<Make>> GetAllMakesAsync();
        Task<Make?> GetMakeByIdAsync(int id);
        Task<Make?> GetMakeByNameAsync(string name, IDbTransaction? transaction = null);
        Task<Make?> CreateMakeAsync(Make make, IDbTransaction? transaction = null);
        Task<Make?> UpdateMakeAsync(Make make);
        Task<bool> DeleteMakeAsync(int id, IDbTransaction? transaction = null);
        Task<bool> ExistsByIdAsync(int id);
    }
}
