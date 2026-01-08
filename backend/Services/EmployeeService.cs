using backend.Exceptions;
using backend.Models;
using backend.Repositories.Interfaces;

namespace backend.Services
{
    public class EmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        // ==============================
        // GET ALL
        // ==============================
        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _employeeRepository.GetAllEmployeesAsync();
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

            var exists = await _employeeRepository.ExistsByIdAsync(employee.Id);
            if (!exists)
                throw new NotFoundException($"Employee with id {employee.Id} not found.");

            var updated = await _employeeRepository.UpdateEmployeeAsync(employee);
            if (updated == null)
                throw new ConflictException("Failed to update employee.");

            return updated;
        }

        // ==============================
        // DELETE
        // ==============================
        public async Task DeleteEmployeeAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Employee id must be greater than zero.");

            var deleted = await _employeeRepository.DeleteEmployeeAsync(id);
            if (!deleted)
                throw new NotFoundException($"Employee with id {id} not found.");
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
