using backend.Exceptions;
using backend.Models;
using backend.Repositories.Interfaces;

namespace backend.Services
{
    public class MakeService
    {
        private readonly IMakeRepository _makeRepository;
        private readonly ICarRepository _carRepository;

        public MakeService(
            IMakeRepository makeRepository,
            ICarRepository carRepository)
        {
            _makeRepository = makeRepository;
            _carRepository = carRepository;
        }

        // ==============================
        // GET ALL
        // ==============================
        public async Task<IEnumerable<Make>> GetAllMakesAsync()
        {
            return await _makeRepository.GetAllMakesAsync();
        }

        // ==============================
        // GET BY ID
        // ==============================
        public async Task<Make> GetMakeByIdAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Make id must be greater than zero.");

            var make = await _makeRepository.GetMakeByIdAsync(id);
            if (make == null)
                throw new ValidationException($"Make with id {id} not found.");

            return make;
        }

        // ==============================
        // CREATE
        // ==============================
        public async Task<Make> CreateMakeAsync(Make make)
        {
            ValidateMake(make);

            var existing = await _makeRepository.GetMakeByNameAsync(make.Name);
            if (existing != null)
                throw new ValidationException("Make with this name already exists.");

            var created = await _makeRepository.CreateMakeAsync(make);
            if (created == null)
                throw new ValidationException("Failed to create make.");

            return created;
        }

        // ==============================
        // UPDATE
        // ==============================
        public async Task<Make> UpdateMakeAsync(Make make)
        {
            if (make.Id <= 0)
                throw new ValidationException("Make id is required.");

            ValidateMake(make);

            var existing = await _makeRepository.GetMakeByIdAsync(make.Id);
            if (existing == null)
                throw new ValidationException($"Make with id {make.Id} not found.");

            var duplicate = await _makeRepository.GetMakeByNameAsync(make.Name);
            if (duplicate != null && duplicate.Id != make.Id)
                throw new ValidationException("Another make with this name already exists.");

            var updated = await _makeRepository.UpdateMakeAsync(make);
            if (updated == null)
                throw new ValidationException("Failed to update make.");

            return updated;
        }

        // ==============================
        // DELETE
        // ==============================
        public async Task DeleteMakeAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Make id must be greater than zero.");

            var exists = await _makeRepository.ExistsByIdAsync(id);
            if (!exists)
                throw new ValidationException($"Make with id {id} not found.");

            if (await _carRepository.ExistsByMakeIdAsync(id))
                throw new ValidationException("Make cannot be deleted because it is used by cars.");

            var deleted = await _makeRepository.DeleteMakeAsync(id);
            if (!deleted)
                throw new ValidationException("Failed to delete make.");
        }

        // ==============================
        // VALIDATION
        // ==============================
        private static void ValidateMake(Make make)
        {
            if (make == null)
                throw new ValidationException("Make payload is required.");

            if (string.IsNullOrWhiteSpace(make.Name))
                throw new ValidationException("Make name is required.");

            if (make.Name.Length > 100)
                throw new ValidationException("Make name is too long.");
        }
    }
}
