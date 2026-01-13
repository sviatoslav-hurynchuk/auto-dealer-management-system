using backend.Exceptions;
using backend.Models;
using backend.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Net.Mail;

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
                throw new ValidationException($"Customer with id {id} not found.");

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
                ValidateEmailFormat(customer.Email);

                var existing = await _customerRepository.GetCustomerByEmailAsync(customer.Email);
                if (existing != null)
                    throw new ValidationException("Customer with this email already exists.");
            }

            try
            {
                var created = await _customerRepository.CreateCustomerAsync(customer);

                if (created == null)
                    throw new ValidationException("Failed to create customer.");

                return created;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (SqlException ex)
            {
                throw new ValidationException($"Database error while creating customer: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new ValidationException($"Unexpected error while creating customer: {ex.Message}");
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
                throw new ValidationException("Customer not found.");

            if (!string.IsNullOrWhiteSpace(customer.Email))
            {
                ValidateEmailFormat(customer.Email);

                var duplicate = await _customerRepository.GetCustomerByEmailAsync(customer.Email);
                if (duplicate != null && duplicate.Id != customer.Id)
                    throw new ValidationException("Another customer with this email already exists.");
            }

            try
            {
                var updated = await _customerRepository.UpdateCustomerAsync(customer);

                if (updated == null)
                    throw new ValidationException("Failed to update customer.");

                return updated;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (SqlException ex)
            {
                throw new ValidationException($"Database error while updating customer: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new ValidationException($"Unexpected error while updating customer: {ex.Message}");
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
                throw new ValidationException("Customer not found.");

            var hasSales = await _saleRepository.ExistsByCustomerIdAsync(id);
            if (hasSales)
                throw new ValidationException("Cannot delete customer with existing sales.");

            try
            {
                var deleted = await _customerRepository.DeleteCustomerAsync(id);
                if (!deleted)
                    throw new ValidationException("Failed to delete customer.");
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (SqlException ex)
            {
                throw new ValidationException($"Database error while deleting customer: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new ValidationException($"Unexpected error while deleting customer: {ex.Message}");
            }
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

        private static void ValidateEmailFormat(string email)
        {
            try
            {
                _ = new MailAddress(email);
            }
            catch
            {
                throw new ValidationException("Email format is invalid.");
            }
        }
    }
}
