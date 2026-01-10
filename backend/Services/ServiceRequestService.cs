using backend.Models;
using backend.Repositories.Interfaces;
using backend.Exceptions;

namespace backend.Services
{
    public class ServiceRequestService
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly ICarRepository _carRepository;

        public ServiceRequestService(
            IServiceRequestRepository serviceRequestRepository,
            ICarRepository carRepository)
        {
            _serviceRequestRepository = serviceRequestRepository;
            _carRepository = carRepository;
        }

        // ==============================
        // GET ALL
        // ==============================
        public async Task<IEnumerable<ServiceRequest>> GetAllAsync()
        {
            return await _serviceRequestRepository.GetAllAsync();
        }

        // ==============================
        // GET BY ID
        // ==============================
        public async Task<ServiceRequest> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("ServiceRequest id must be greater than zero");

            var request = await _serviceRequestRepository.GetByIdAsync(id);
            if (request == null)
                throw new NotFoundException($"ServiceRequest with id {id} not found");

            return request;
        }

        // ==============================
        // GET BY CAR
        // ==============================
        public async Task<IEnumerable<ServiceRequest>> GetByCarIdAsync(int carId)
        {
            if (carId <= 0)
                throw new ValidationException("CarId must be greater than zero");

            var car = await _carRepository.GetCarByIdAsync(carId);
            if (car == null)
                throw new NotFoundException("Car not found");

            return await _serviceRequestRepository.GetByCarIdAsync(carId);
        }

        // ==============================
        // CREATE
        // ==============================
        public async Task<ServiceRequest> CreateAsync(ServiceRequest request)
        {
            Validate(request);

            var car = await _carRepository.GetCarByIdAsync(request.CarId);
            if (car == null)
                throw new NotFoundException("Car not found");

            var created = await _serviceRequestRepository.CreateAsync(request);
            if (created == null)
                throw new ConflictException("Failed to create service request");

            return created;
        }

        // ==============================
        // UPDATE
        // ==============================
        public async Task<ServiceRequest> UpdateAsync(ServiceRequest request)
        {
            if (request.Id <= 0)
                throw new ValidationException("ServiceRequest id is required");

            Validate(request);

            var existing = await _serviceRequestRepository.GetByIdAsync(request.Id);
            if (existing == null)
                throw new NotFoundException($"ServiceRequest with id {request.Id} not found");

            var updated = await _serviceRequestRepository.UpdateAsync(request);
            if (updated == null)
                throw new ConflictException("Failed to update service request");

            return updated;
        }

        // ==============================
        // DELETE
        // ==============================
        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("ServiceRequest id must be greater than zero");

            var existing = await _serviceRequestRepository.GetByIdAsync(id);
            if (existing == null)
                throw new NotFoundException($"ServiceRequest with id {id} not found");

            var deleted = await _serviceRequestRepository.DeleteAsync(id);
            if (!deleted)
                throw new ConflictException("Failed to delete service request");
        }

        // ==============================
        // VALIDATION
        // ==============================
        private static void Validate(ServiceRequest request)
        {
            if (request == null)
                throw new ValidationException("ServiceRequest data is required");

            if (request.CarId <= 0)
                throw new ValidationException("CarId is required");

            if (string.IsNullOrWhiteSpace(request.ServiceType))
                throw new ValidationException("ServiceType is required");

            if (string.IsNullOrWhiteSpace(request.Status))
                throw new ValidationException("Status is required");

            if (request.Status != "Pending" &&
                request.Status != "InProgress" &&
                request.Status != "Completed" &&
                request.Status != "Cancelled")
                throw new ValidationException("Invalid service request status");
        }
    }
}
