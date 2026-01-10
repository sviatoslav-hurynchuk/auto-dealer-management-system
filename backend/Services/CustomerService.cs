using backend.Exceptions;
using backend.Models;
using backend.Repositories.Interfaces;

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

        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            ValidateCustomer(customer);

            if (!string.IsNullOrWhiteSpace(customer.Email))
            {
                var existing = await _customerRepository.GetCustomerByEmailAsync(customer.Email);
                if (existing != null)
                    throw new ConflictException("Customer with this email already exists.");
            }

            return await _customerRepository.CreateCustomerAsync(customer);
        }

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

            return await _customerRepository.UpdateCustomerAsync(customer);
        }

        public async Task DeleteCustomerAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Customer id must be greater than zero.");

            var exists = await _customerRepository.ExistsByIdAsync(id);
            if (!exists)
                throw new NotFoundException("Customer not found.");

            var hasSales = await _saleRepository.ExistsByCustomerIdAsync(id);
            if (hasSales)
                throw new ConflictException(
                    "Cannot delete customer with existing sales."
                );

            var deleted = await _customerRepository.DeleteCustomerAsync(id);
            if (!deleted)
                throw new ConflictException("Failed to delete customer.");
        }


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
    }
}
