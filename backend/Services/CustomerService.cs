using backend.Exceptions;
using backend.Models;
using backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace backend.Services
{
    public class CustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ISaleRepository _saleRepository;

        public CustomerService(
            ICustomerRepository customerRepository,
            ISaleRepository saleRepository)
        {
            _customerRepository = customerRepository;
            _saleRepository = saleRepository;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
            => await _customerRepository.GetAllCustomersAsync();

        public async Task<Customer> GetCustomerByIdAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Customer id must be greater than zero.");

            var customer = await _customerRepository.GetCustomerByIdAsync(id);
            if (customer == null)
                throw new NotFoundException($"Customer with id {id} not found.");

            return customer;
        }

        // ==============================
        // CREATE
        // ==============================
        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            ValidateCustomer(customer);

            if (!string.IsNullOrWhiteSpace(customer.Email))
            {
                var existing = await _customerRepository.GetCustomerByEmailAsync(customer.Email);
                if (existing != null)
                    throw new ConflictException("Customer with this email already exists.");
            }

            try
            {
                return await _customerRepository.CreateCustomerAsync(customer);
            }
            catch (ConflictException)
            {
                throw;
            }
            catch (Exception ex) when (IsUniqueViolation(ex))
            {
                throw new ConflictException("Customer with this email already exists.");
            }
        }

        // ==============================
        // UPDATE
        // ==============================
        public async Task<Customer> UpdateCustomerAsync(Customer customer)
        {
            if (customer.Id <= 0)
                throw new ValidationException("Customer id is required.");

            ValidateCustomer(customer);

            var exists = await _customerRepository.ExistsByIdAsync(customer.Id);
            if (!exists)
                throw new NotFoundException("Customer not found.");

            if (!string.IsNullOrWhiteSpace(customer.Email))
            {
                var duplicate = await _customerRepository.GetCustomerByEmailAsync(customer.Email);
                if (duplicate != null && duplicate.Id != customer.Id)
                    throw new ConflictException("Another customer with this email already exists.");
            }

            try
            {
                return await _customerRepository.UpdateCustomerAsync(customer);
            }
            catch (ConflictException)
            {
                throw;
            }
            catch (Exception ex) when (IsUniqueViolation(ex))
            {
                throw new ConflictException("Another customer with this email already exists.");
            }
        }

        // ==============================
        // DELETE
        // ==============================
        public async Task DeleteCustomerAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Customer id must be greater than zero.");

            var exists = await _customerRepository.ExistsByIdAsync(id);
            if (!exists)
                throw new NotFoundException("Customer not found.");

            var hasSales = await _saleRepository.ExistsByCustomerIdAsync(id);
            if (hasSales)
                throw new ConflictException("Cannot delete customer with existing sales.");

            try
            {
                var deleted = await _customerRepository.DeleteCustomerAsync(id);
                if (!deleted)
                    throw new ConflictException("Failed to delete customer.");
            }
            catch (ConflictException)
            {
                throw;
            }
            catch (Exception ex) when (IsForeignKeyViolation(ex))
            {
                throw new ConflictException("Cannot delete customer because related records exist (e.g., sales).");
            }
        }

        // ==============================
        // SQL FOREIGN KEY VIOLATION DETECTOR
        // ==============================
        private static bool IsForeignKeyViolation(Exception ex)
        {
            if (ex is SqlException sqlEx)
                return sqlEx.Number == 547; // Foreign key violation in SQL Server

            if (ex.InnerException != null)
                return IsForeignKeyViolation(ex.InnerException);

            return false;
        }


        // ==============================
        // VALIDATION
        // ==============================
        private static void ValidateCustomer(Customer customer)
        {
            if (customer == null)
                throw new ValidationException("Customer payload is required.");

            if (string.IsNullOrWhiteSpace(customer.FullName))
                throw new ValidationException("Full name is required.");

            if (customer.FullName.Length > 150)
                throw new ValidationException("Full name is too long.");

            if (customer.Email?.Length > 100)
                throw new ValidationException("Email is too long.");

            if (customer.Phone?.Length > 20)
                throw new ValidationException("Phone is too long.");
        }

        // ==============================
        // SQL UNIQUE VIOLATION DETECTOR
        // ==============================
        private static bool IsUniqueViolation(Exception ex)
        {
            if (ex is SqlException sqlEx)
                return sqlEx.Number == 2627 || sqlEx.Number == 2601;

            if (ex.InnerException != null)
                return IsUniqueViolation(ex.InnerException);

            return false;
        }
    }
}
