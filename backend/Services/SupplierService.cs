using backend.Exceptions;
using backend.Models;
using backend.Repositories.Interfaces;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace backend.Services
{
    public class SupplierService
    {
        private readonly ISupplierRepository _supplierRepository;

        public SupplierService(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public async Task<IEnumerable<Supplier>> GetAllSuppliersAsync()
            => await _supplierRepository.GetAllSuppliersAsync();

        public async Task<Supplier> GetSupplierByIdAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Supplier id must be greater than zero.");

            var supplier = await _supplierRepository.GetSupplierByIdAsync(id);
            if (supplier == null)
                throw new ValidationException($"Supplier with id {id} not found.");

            return supplier;
        }

        public async Task<Supplier> GetSupplierByCompanyNameAsync(string companyName)
        {
            if (string.IsNullOrWhiteSpace(companyName))
                throw new ValidationException("Company name is required.");

            var supplier = await _supplierRepository.GetSupplierByCompanyNameAsync(companyName);
            if (supplier == null)
                throw new ValidationException($"Supplier with company name '{companyName}' not found.");

            return supplier;
        }

        public async Task<Supplier> CreateSupplierAsync(Supplier supplier)
        {
            ValidateSupplier(supplier);

            var existing = await _supplierRepository.GetSupplierByCompanyNameAsync(supplier.CompanyName);
            if (existing != null)
                throw new ValidationException("Supplier with this company name already exists.");

            return await _supplierRepository.CreateSupplierAsync(supplier);
        }

        public async Task<Supplier> UpdateSupplierAsync(Supplier supplier)
        {
            if (supplier.Id <= 0)
                throw new ValidationException("Supplier id is required.");

            ValidateSupplierUpdate(supplier);

            var existing = await _supplierRepository.GetSupplierByIdAsync(supplier.Id);
            if (existing == null)
                throw new ValidationException("Supplier not found.");

            var duplicate = await _supplierRepository.GetSupplierByCompanyNameAsync(supplier.CompanyName);
            if (duplicate != null && duplicate.Id != supplier.Id)
                throw new ValidationException("Another supplier with this company name already exists.");

            return await _supplierRepository.UpdateSupplierAsync(supplier);
        }

        public async Task DeleteSupplierAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Supplier id must be greater than zero.");

            var supplier = await _supplierRepository.GetSupplierByIdAsync(id);
            if (supplier == null)
                throw new ValidationException("Supplier not found.");

            if (await _supplierRepository.HasOrdersAsync(id))
                throw new ValidationException("Cannot delete supplier with existing orders.");

            if (await _supplierRepository.HasCarsAsync(id))
                throw new ValidationException("Cannot delete supplier with linked cars.");

            var deleted = await _supplierRepository.DeleteSupplierAsync(id);
            if (!deleted)
                throw new ValidationException("Failed to delete supplier.");
        }

        // ==============================
        // VALIDATION
        // ==============================
        private static void ValidateSupplier(Supplier supplier)
        {
            if (supplier == null)
                throw new ValidationException("Supplier payload is required.");

            if (string.IsNullOrWhiteSpace(supplier.CompanyName))
                throw new ValidationException("Company name is required.");

            if (supplier.CompanyName.Length > 100)
                throw new ValidationException("Company name is too long.");

            ValidateEmail(supplier.Email);
            ValidatePhone(supplier.Phone);
        }

        private static void ValidateSupplierUpdate(Supplier supplier)
        {
            if (supplier == null)
                throw new ValidationException("Supplier payload is required.");

            if (!string.IsNullOrWhiteSpace(supplier.CompanyName) && supplier.CompanyName.Length > 100)
                throw new ValidationException("Company name is too long.");

            if (!string.IsNullOrWhiteSpace(supplier.Email))
                ValidateEmail(supplier.Email);

            if (!string.IsNullOrWhiteSpace(supplier.Phone))
                ValidatePhone(supplier.Phone);
        }

        private static void ValidateEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return;

            if (email.Length > 100)
                throw new ValidationException("Email is too long.");

            try
            {
                var _ = new MailAddress(email);
            }
            catch
            {
                throw new ValidationException("Invalid email format.");
            }
        }

        private static void ValidatePhone(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return;

            if (phone.Length > 20)
                throw new ValidationException("Phone number is too long.");

            var regex = new Regex(@"^\+?[0-9\s\-]+$");
            if (!regex.IsMatch(phone))
                throw new ValidationException("Invalid phone number format.");
        }
    }
}
