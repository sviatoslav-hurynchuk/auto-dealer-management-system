using backend.Exceptions;
using backend.Models;
using backend.Services;
using backend.Repositories.Interfaces;

namespace backend.Services
{
    public class OrderTransactionService
    {
        private readonly CarService _carService;
        private readonly OrderService _orderService;
        private readonly ISupplierRepository _supplierRepository;

        public OrderTransactionService(
            CarService carService,
            OrderService orderService,
            ISupplierRepository supplierRepository)
        {
            _carService = carService;
            _orderService = orderService;
            _supplierRepository = supplierRepository;
        }

        // ==============================
        // CREATE ORDER WITH CAR (TRANSACTION)
        // ==============================
        public async Task<Order> CreateOrderWithCarAsync(
            string makeName,
            Car car,
            string supplierName,
            Order order)
        {
            Car? createdCar = null;
            bool carCreated = false;

            try
            {
                // =======================
                // Створюємо машину + марку
                // =======================
                createdCar = await _carService.CreateCarWithMakeAsync(makeName, car);
                carCreated = true;

                // =======================
                // Знаходимо постачальника за ім’ям
                // =======================
                var supplier = await _supplierRepository.GetSupplierByCompanyNameAsync(supplierName);
                if (supplier == null)
                    throw new NotFoundException($"Supplier with name '{supplierName}' not found.");

                // =======================
                // Присвоюємо CarId і SupplierId замовленню
                // =======================
                order.CarId = createdCar.Id;
                order.SupplierId = supplier.Id;

                // =======================
                // Створюємо замовлення
                // =======================
                var createdOrder = await _orderService.CreateOrderAsync(order);

                return createdOrder;
            }
            catch
            {
                // =======================
                // Компенсація: видаляємо машину, якщо створили
                // =======================
                if (carCreated && createdCar != null)
                {
                    try
                    {
                        await _carService.DeleteCarAsync(createdCar.Id);
                    }
                    catch
                    {
                        // Якщо не вдалося видалити — логнемо, але не кидаємо нову помилку
                        // _logger.LogError(ex, "Failed to rollback created car.");
                    }
                }

                throw; // повторно кидаємо початкову помилку
            }
        }
    }
}
