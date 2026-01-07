using backend.Models;

namespace backend.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int id);
        Task<Order?> CreateOrderAsync(Order order);
        Task<Order?> UpdateOrderAsync(Order order);
        Task<bool> DeleteOrderAsync(int id);
        Task<bool> ExistsByCarIdAsync(int carId);
    }
}
