using backend.Exceptions;
using backend.Models;
using backend.Repositories.Interfaces;

namespace backend.Services
{
    public class EmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ISaleRepository _saleRepository;

        public EmployeeService(
            IEmployeeRepository employeeRepository,
            ISaleRepository saleRepository)
        {
            _employeeRepository = employeeRepository;
            _saleRepository = saleRepository;
        }

        // ==============================
        // GET ALL
        // ==============================
        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync(bool activeOnly = false)
        {
            return activeOnly
                ? await _employeeRepository.GetAllActiveEmployeesAsync()
                : await _employeeRepository.GetAllEmployeesAsync();
        }

        // ==============================
        // GET BY ID
        // ==============================
        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Employee id must be greater than zero.");

            var employee = await _employeeRepository.GetEmployeeByIdAsync(id);
            if (employee == null)
                throw new NotFoundException($"Employee with id {id} not found.");

            return employee;
        }

        // ==============================
        // CREATE
        // ==============================
        public async Task<Employee> CreateEmployeeAsync(Employee employee)
        {
            ValidateEmployee(employee);

            employee.IsActive = true;

            var created = await _employeeRepository.CreateEmployeeAsync(employee);
            if (created == null)
                throw new ConflictException("Failed to create employee.");

            return created;
        }

        // ==============================
        // UPDATE
        // ==============================
        public async Task<Employee> UpdateEmployeeAsync(Employee employee)
        {
            if (employee.Id <= 0)
                throw new ValidationException("Employee id is required.");

            ValidateEmployee(employee);

            var existing = await _employeeRepository.GetEmployeeByIdAsync(employee.Id);
            if (existing == null)
                throw new NotFoundException($"Employee with id {employee.Id} not found.");

            employee.IsActive = existing.IsActive;

            var updated = await _employeeRepository.UpdateEmployeeAsync(employee);
            if (updated == null)
                throw new ConflictException("Failed to update employee.");

            return updated;
        }

        // ==============================
        // DEACTIVATE (SOFT)
        // ==============================
        public async Task DeactivateEmployeeAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Employee id must be greater than zero.");

            var employee = await _employeeRepository.GetEmployeeByIdAsync(id);
            if (employee == null)
                throw new NotFoundException($"Employee with id {id} not found.");

            if (!employee.IsActive)
                throw new ConflictException("Employee is already inactive.");

            var success = await _employeeRepository.DeactivateEmployeeAsync(id);
            if (!success)
                throw new ConflictException("Failed to deactivate employee.");
        }

        // ==============================
        // DELETE (HARD)
        // ==============================
        public async Task DeleteEmployeeAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Employee id must be greater than zero.");

            var employee = await _employeeRepository.GetEmployeeByIdAsync(id);
            if (employee == null)
                throw new NotFoundException($"Employee with id {id} not found.");

            // 🔒 ключова бізнес-перевірка
            if (await _saleRepository.ExistsByEmployeeIdAsync(id))
                throw new ConflictException(
                    "Employee cannot be deleted because they have sales."
                );

            var deleted = await _employeeRepository.DeleteEmployeeAsync(id);
            if (!deleted)
                throw new ConflictException("Failed to delete employee.");
        }

        // ==============================
        // VALIDATION
        // ==============================
        private static void ValidateEmployee(Employee employee)
        {
            if (employee == null)
                throw new ValidationException("Employee payload is required.");

            if (string.IsNullOrWhiteSpace(employee.FullName))
                throw new ValidationException("FullName is required.");

            if (employee.FullName.Length > 100)
                throw new ValidationException("FullName is too long.");

            if (!string.IsNullOrWhiteSpace(employee.Email) &&
                employee.Email.Length > 100)
                throw new ValidationException("Email is too long.");
        }
    }
}
