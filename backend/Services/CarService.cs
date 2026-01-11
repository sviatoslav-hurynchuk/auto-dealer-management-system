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
        private readonly IMakeRepository _makeRepository;
        /// <summary>
        /// Initializes a new instance of <see cref="CarService"/> with the required repository dependencies.
        /// </summary>
        /// <param name="carRepository">Repository for car persistence and retrieval.</param>
        /// <param name="saleRepository">Repository for sale-related checks and operations.</param>
        /// <param name="orderRepository">Repository for order-related checks and operations.</param>
        /// <param name="makeRepository">Repository for make (manufacturer) persistence and retrieval.</param>
        public CarService(ICarRepository carRepository, ISaleRepository saleRepository,IOrderRepository orderRepository, IMakeRepository makeRepository)
        {
            _carRepository = carRepository;
            _saleRepository = saleRepository;
            _orderRepository = orderRepository;
            _makeRepository = makeRepository;
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
        /// <summary>
        /// Create a new car and persist it to the repository.
        /// </summary>
        /// <param name="car">The car to create; must satisfy the service validation rules (required fields such as MakeId, Model, Year, Price, VIN, and Status).</param>
        /// <returns>The persisted <see cref="Car"/> with repository-assigned values (for example, the Id).</returns>
        /// <exception cref="ValidationException">Thrown when the provided <paramref name="car"/> fails validation.</exception>
        /// <exception cref="ConflictException">Thrown when the repository fails to create the car.</exception>
        public async Task<Car> CreateCarAsync(Car car)
        {
            ValidateCar(car);

            var createdCar = await _carRepository.CreateCarAsync(car);
            if (createdCar == null)
                throw new ConflictException("Failed to create car.");

            return createdCar;
        }
        /// <summary>
        /// Ensures a make with the given name exists (creating it if necessary) and creates the provided car associated with that make.
        /// </summary>
        /// <param name="makeName">The name of the make to associate with the car; must be non-empty.</param>
        /// <param name="car">The car to create. Its MakeId will be set to the ensured make's Id.</param>
        /// <returns>The newly created <see cref="Car"/> with its <c>MakeId</c> assigned.</returns>
        /// <exception cref="ValidationException">Thrown when <paramref name="makeName"/> is null, empty, or whitespace.</exception>
        /// <exception cref="ConflictException">Thrown if creating a new make fails.</exception>
        public async Task<Car> CreateCarWithMakeAsync(string makeName, Car car)
        {
            if (string.IsNullOrWhiteSpace(makeName))
                throw new ValidationException("Make name is required.");

            Make? make = null;
            bool isNewMake = false;

            try
            {
                make = await _makeRepository.GetMakeByNameAsync(makeName);

                if (make == null)
                {
                    make = new Make { Name = makeName };
                    make = await _makeRepository.CreateMakeAsync(make);
                    if (make == null)
                        throw new ConflictException("Failed to create make.");
                    isNewMake = true; // <-- прапорець не ставиться

                }

                // Присвоюємо MakeId машини
                car.MakeId = make.Id;

                // Створюємо машину
                var createdCar = await CreateCarAsync(car);
                return createdCar;
            }
            catch
            {
                if (isNewMake && make != null)
                {
                    try
                    {
                        await _makeRepository.DeleteMakeAsync(make.Id);
                    }
                    catch
                    {
                        // Якщо видалити марку не вдалося — логнемо, але не кидаємо нову помилку
                        // Можна додати логування:
                        // _logger.LogError(ex, "Failed to rollback newly created make.");
                    }
                }

                // Повторно кидаємо початкову помилку
                throw;
            }
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