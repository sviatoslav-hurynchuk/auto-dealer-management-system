using backend.Exceptions;
using backend.Models;
using backend.Repositories;
using backend.Repositories.Interfaces;

namespace backend.Services
{
    public class CarService
    {
        private readonly ICarRepository _carRepository;

        public CarService(ICarRepository carRepository)
        {
            _carRepository = carRepository;
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
        public async Task<Car?> GetCarByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Car id must be greater than zero.");

            return await _carRepository.GetCarByIdAsync(id);
        }

        // ==============================
        // CREATE
        // ==============================
        public async Task<Car> CreateCarAsync(Car car)
        {
            ValidateCar(car, isUpdate: false);

            var createdCar = await _carRepository.CreateCarAsync(car);
            if (createdCar == null)
                throw new InvalidOperationException("Failed to create car.");

            return createdCar;
        }

        // ==============================
        // UPDATE
        // ==============================
        public async Task<Car> UpdateCarAsync(Car car)
        {
            if (car.Id <= 0)
                throw new ArgumentException("Car id must be specified for update.");

            ValidateCar(car, isUpdate: true);

            var updatedCar = await _carRepository.UpdateCarAsync(car);
            if (updatedCar == null)
                throw new KeyNotFoundException($"Car with id {car.Id} not found.");

            return updatedCar;
        }

        // ==============================
        // DELETE
        // ==============================
        public async Task DeleteCarAsync(int id)//////TODO: Add logic for sales, orders and service requests before deleting a car when their crud operations are ready
        {
            if (id <= 0)
                throw new ArgumentException("Car id must be greater than zero.");

            /*var hasSales = await _saleRepository.ExistsByCarIdAsync(id);
            if (hasSales)
                throw new ConflictException("Car cannot be deleted because it has sales");
            */
            var deleted = await _carRepository.DeleteCarAsync(id);
            if (!deleted)
                throw new KeyNotFoundException($"Car with id {id} not found.");
        }

        // ==============================
        // VALIDATION
        // ==============================
        private static void ValidateCar(Car car, bool isUpdate)
        {
            if (car == null)
                throw new ArgumentNullException(nameof(car));

            if (car.MakeId <= 0)
                throw new ArgumentException("MakeId is required.");

            if (string.IsNullOrWhiteSpace(car.Model))
                throw new ArgumentException("Model is required.");

            if (car.Year < 1900 || car.Year > DateTime.UtcNow.Year + 1)
                throw new ArgumentException("Year is invalid.");

            if (car.Price < 0)
                throw new ArgumentException("Price cannot be negative.");

            if (string.IsNullOrWhiteSpace(car.Vin))
                throw new ArgumentException("VIN is required.");

            if (string.IsNullOrWhiteSpace(car.Status))
                throw new ArgumentException("Status is required.");

            if (!string.IsNullOrWhiteSpace(car.Condition) && car.Condition != "New" && car.Condition != "Used")
                throw new ArgumentException("Condition must be 'New' or 'Used'.");
        }

    }
}
