using backend.Exceptions;
using backend.Models;
using backend.Repositories.Interfaces;

namespace backend.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICarRepository _carRepository;
        private readonly ISupplierRepository _supplierRepository;

        public OrderService(
            IOrderRepository orderRepository,
            ICarRepository carRepository,
            ISupplierRepository supplierRepository)
        {
            _orderRepository = orderRepository;
            _carRepository = carRepository;
            _supplierRepository = supplierRepository;
        }

        // ==============================
        // GET ALL
        // ==============================
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllOrdersAsync();
        }

        // ==============================
        // GET BY ID
        // ==============================
        public async Task<Order> GetOrderByIdAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Order id must be greater than zero.");

            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null)
                throw new NotFoundException($"Order with id {id} not found.");

            return order;
        }

        // ==============================
        // CREATE
        // ==============================
        public async Task<Order> CreateOrderAsync(Order order)
        {
            ValidateOrder(order);

            // Перевірка авто
            var car = await _carRepository.GetCarByIdAsync(order.CarId);
            if (car == null)
                throw new NotFoundException("Car not found.");

            if (car.Status != "Pending")
                throw new ConflictException("Car cannot be ordered in its current status.");

            // Перевірка постачальника
            var supplier = await _supplierRepository.GetSupplierByIdAsync(order.SupplierId);
            if (supplier == null)
                throw new NotFoundException("Supplier not found.");

            var createdOrder = await _orderRepository.CreateOrderAsync(order);
            if (createdOrder == null)
                throw new InvalidOperationException("Failed to create order.");

            return createdOrder;
        }

        // ==============================
        // UPDATE
        // ==============================
        public async Task<Order> UpdateOrderAsync(Order order)
        {
            if (order.Id <= 0)
                throw new ValidationException("Order id is required.");

            ValidateOrder(order);

            var existingOrder = await _orderRepository.GetOrderByIdAsync(order.Id);
            if (existingOrder == null)
                throw new NotFoundException($"Order with id {order.Id} not found.");

            // Якщо змінився CarId, перевіряємо нове авто
            if (existingOrder.CarId != order.CarId)
            {
                var car = await _carRepository.GetCarByIdAsync(order.CarId);
                if (car == null)
                    throw new NotFoundException("Car not found.");

                if (car.Status != "Pending")
                    throw new ConflictException("Car cannot be ordered in its current status.");
            }

            // Якщо змінився SupplierId, перевіряємо нового постачальника
            if (existingOrder.SupplierId != order.SupplierId)
            {
                var supplier = await _supplierRepository.GetSupplierByIdAsync(order.SupplierId);
                if (supplier == null)
                    throw new NotFoundException("Supplier not found.");
            }

            var updated = await _orderRepository.UpdateOrderAsync(order);
            if (updated == null)
                throw new InvalidOperationException("Failed to update order.");

            return updated;
        }

        // ==============================
        // DELETE
        // ==============================
        public async Task DeleteOrderAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Order id must be greater than zero.");

            var deleted = await _orderRepository.DeleteOrderAsync(id);
            if (!deleted)
                throw new NotFoundException($"Order with id {id} not found.");
        }

        // ==============================
        // VALIDATION
        // ==============================
        private static void ValidateOrder(Order order)
        {
            if (order == null)
                throw new ValidationException("Order is required.");

            if (order.SupplierId <= 0)
                throw new ValidationException("SupplierId is required.");

            if (order.CarId <= 0)
                throw new ValidationException("CarId is required.");

            if (order.Quantity <= 0)
                throw new ValidationException("Quantity must be greater than zero.");

            if (string.IsNullOrWhiteSpace(order.Status))
                throw new ValidationException("Status is required.");

            if (order.Status != "Pending" &&
                order.Status != "Completed" &&
                order.Status != "Cancelled")
                throw new ValidationException("Invalid order status.");
        }
    }
}
