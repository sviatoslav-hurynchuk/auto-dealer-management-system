using backend.Exceptions;
using backend.Models;
using backend.Repositories;
using backend.Repositories.Interfaces;

namespace backend.Services
{
    public class CarService
    {
        private readonly ICarRepository _carRepository;
        private readonly ISaleRepository _saleRepository;
        private readonly IOrderRepository _orderRepository;
        public CarService(ICarRepository carRepository, ISaleRepository saleRepository,IOrderRepository orderRepository)
        {
            _carRepository = carRepository;
            _saleRepository = saleRepository;
            _orderRepository = orderRepository;
        }

        // ==============================
        // GET ALL
        // ==============================
        public async Task<IEnumerable<Car>> GetAllCarsAsync()
        {
            return await _carRepository.GetAllCarsAsync();
        }

        // ==============================
        // GET BY ID
        // ==============================
        public async Task<Car> GetCarByIdAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Car id must be greater than zero.");

            var car = await _carRepository.GetCarByIdAsync(id);
            if (car == null)
                throw new NotFoundException($"Car with id {id} not found.");

            return car;
        }

        // ==============================
        // CREATE
        // ==============================
        public async Task<Car> CreateCarAsync(Car car)
        {
            ValidateCar(car);

            var createdCar = await _carRepository.CreateCarAsync(car);
            if (createdCar == null)
                throw new ConflictException("Failed to create car.");

            return createdCar;
        }

        // ==============================
        // UPDATE
        // ==============================
        public async Task<Car> UpdateCarAsync(Car car)
        {
            if (car.Id <= 0)
                throw new ValidationException("Car id must be specified for update.");

            ValidateCar(car);

            var existingCar = await _carRepository.GetCarByIdAsync(car.Id);
            if (existingCar == null)
                throw new NotFoundException($"Car with id {car.Id} not found.");

            var updatedCar = await _carRepository.UpdateCarAsync(car);
            if (updatedCar == null)
                throw new ConflictException("Failed to update car.");

            return updatedCar;
        }

        // ==============================
        // DELETE
        // ==============================
        public async Task DeleteCarAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Car id must be greater than zero.");

            var car = await _carRepository.GetCarByIdAsync(id);
            if (car == null)
                throw new NotFoundException($"Car with id {id} not found.");

            if (await _saleRepository.ExistsByCarIdAsync(id))
                throw new ConflictException("Car cannot be deleted because it has sales.");

            if (await _orderRepository.ExistsByCarIdAsync(id))
                throw new ConflictException("Car cannot be deleted because it has orders.");

            var deleted = await _carRepository.DeleteCarAsync(id);
            if (!deleted)
                throw new ConflictException("Failed to delete car.");
        }

        // ==============================
        // VALIDATION
        // ==============================
        private static void ValidateCar(Car car)
        {
            if (car == null)
                throw new ValidationException("Car payload is required.");

            if (car.MakeId <= 0)
                throw new ValidationException("MakeId is required.");

            if (string.IsNullOrWhiteSpace(car.Model))
                throw new ValidationException("Model is required.");

            if (car.Year < 1900 || car.Year > DateTime.UtcNow.Year + 1)
                throw new ValidationException("Year is invalid.");

            if (car.Price < 0)
                throw new ValidationException("Price cannot be negative.");

            if (string.IsNullOrWhiteSpace(car.Vin))
                throw new ValidationException("VIN is required.");

            if (string.IsNullOrWhiteSpace(car.Status))
                throw new ValidationException("Status is required.");

            if (car.Status != "Pending" &&
                car.Status != "In stock" &&
                car.Status != "Sold" &&
                car.Status != "Archived")
                throw new ValidationException("Invalid car status.");

            if (!string.IsNullOrWhiteSpace(car.Condition) &&
                car.Condition != "New" &&
                car.Condition != "Used")
                throw new ValidationException("Condition must be 'New' or 'Used'.");
        }
    }
}
