using backend.Models;
using backend.Repositories.Interfaces;
using backend.Exceptions;

namespace backend.Services
{
    public class SaleService
    {
        private readonly ISaleRepository _saleRepository;
        private readonly ICarRepository _carRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICustomerRepository _customerRepository;

        public SaleService(
        ISaleRepository saleRepository,
        ICarRepository carRepository,
        IEmployeeRepository employeeRepository,
        ICustomerRepository customerRepository)
        {
            _saleRepository = saleRepository;
            _carRepository = carRepository;
            _employeeRepository = employeeRepository;
            _customerRepository = customerRepository;
        }


        // ==============================
        // GET ALL
        // ==============================
        public async Task<IEnumerable<Sale>> GetAllSalesAsync()
        {
            return await _saleRepository.GetAllSalesAsync();
        }

        // ==============================
        // GET BY ID
        // ==============================
        public async Task<Sale> GetSaleByIdAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Sale id must be greater than zero");

            var sale = await _saleRepository.GetSaleByIdAsync(id);
            if (sale == null)
                throw new NotFoundException($"Sale with id {id} not found");

            return sale;
        }

        // ==============================
        // CREATE
        // ==============================
        public async Task<Sale> CreateSaleAsync(Sale sale)
        {
            ValidateSale(sale);

            var customerExists = await _customerRepository.ExistsByIdAsync(sale.CustomerId);
            if (!customerExists)
                throw new ValidationException("Customer not found.");

            var car = await _carRepository.GetCarByIdAsync(sale.CarId);
            if (car == null)
                throw new ValidationException("Car not found.");

            if (car.Status == "Sold")
                throw new ValidationException("Car is already sold.");

            var employee = await _employeeRepository.GetEmployeeByIdAsync(sale.EmployeeId);
            if (employee == null)
                throw new ValidationException("Employee not found.");

            if (!employee.IsActive)
                throw new ValidationException("Employee is inactive.");

            var createdSale = await _saleRepository.CreateSaleAsync(sale);
            if (createdSale == null)
                throw new ValidationException("Failed to create sale.");

            if (sale.Status == "Completed")
            {
                car.Status = "Sold";

                var updatedCar = await _carRepository.UpdateCarAsync(car);
                if (updatedCar == null)
                    throw new ValidationException("Failed to update car status after sale creation.");
            }

            return createdSale;
        }


        // ==============================
        // UPDATE
        // ==============================
        public async Task<Sale> UpdateSaleAsync(Sale sale)
        {
            if (sale.Id <= 0)
                throw new ValidationException("Sale id must be specified for update.");

            ValidateSale(sale);

            var existingSale = await _saleRepository.GetSaleByIdAsync(sale.Id);
            if (existingSale == null)
                throw new ValidationException($"Sale with id {sale.Id} not found.");

            if (sale.CustomerId != existingSale.CustomerId)
            {
                var customerExists = await _customerRepository.ExistsByIdAsync(sale.CustomerId);
                if (!customerExists)
                    throw new ValidationException("Customer not found.");
            }

            if (sale.EmployeeId != existingSale.EmployeeId)
            {
                var employee = await _employeeRepository.GetEmployeeByIdAsync(sale.EmployeeId);
                if (employee == null)
                    throw new ValidationException("Employee not found.");

                if (!employee.IsActive)
                    throw new ValidationException("Employee is inactive.");
            }

            if (sale.CarId != existingSale.CarId)
            {
                if (existingSale.Status == "Completed")
                    throw new ValidationException("Cannot change car for completed sale.");

                var car = await _carRepository.GetCarByIdAsync(sale.CarId);
                if (car == null)
                    throw new ValidationException("Car not found.");

                if (car.Status == "Sold")
                    throw new ValidationException("Car is already sold.");
            }

            var updatedSale = await _saleRepository.UpdateSaleAsync(sale);
            if (updatedSale == null)
                throw new ValidationException("Failed to update sale.");

            if (sale.Status == "Completed" && existingSale.Status != "Completed")
            {
                var car = await _carRepository.GetCarByIdAsync(sale.CarId);
                if (car == null)
                    throw new ValidationException("Car not found.");

                if (car.Status == "Sold")
                    throw new ValidationException("Car is already sold.");

                car.Status = "Sold";

                var updatedCar = await _carRepository.UpdateCarAsync(car);
                if (updatedCar == null)
                    throw new ValidationException("Failed to update car status after sale completion.");
            }

            return updatedSale;
        }



        // ==============================
        // DELETE
        // ==============================
        public async Task DeleteSaleAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Sale id must be greater than zero");

            var sale = await _saleRepository.GetSaleByIdAsync(id);
            if (sale == null)
                throw new NotFoundException($"Sale with id {id} not found");

            var deleted = await _saleRepository.DeleteSaleAsync(id);
            if (!deleted)
                throw new ConflictException("Failed to delete sale");
        }

        // ==============================
        // VALIDATION
        // ==============================
        private static void ValidateSale(Sale sale)
        {
            if (sale == null)
                throw new ValidationException("Sale data is required");

            if (sale.CarId <= 0)
                throw new ValidationException("CarId is required");

            if (sale.CustomerId <= 0)
                throw new ValidationException("CustomerId is required");

            if (sale.EmployeeId <= 0)
                throw new ValidationException("EmployeeId is required");

            if (sale.FinalPrice < 0)
                throw new ValidationException("FinalPrice cannot be negative");

            if (string.IsNullOrWhiteSpace(sale.Status))
                throw new ValidationException("Status is required");

            if (sale.Status != "Pending" &&
                sale.Status != "Completed" &&
                sale.Status != "Cancelled")
                throw new ValidationException("Invalid sale status");
        }
    }
}
