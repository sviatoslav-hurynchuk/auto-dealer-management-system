using backend.Exceptions;
using backend.Models;
using backend.Repositories.Interfaces;

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
                throw new NotFoundException($"Supplier with id {id} not found.");

            return supplier;
        }

        public async Task<Supplier> CreateSupplierAsync(Supplier supplier)
        {
            ValidateSupplier(supplier);

            var existing = await _supplierRepository.GetSupplierByCompanyNameAsync(supplier.CompanyName);
            if (existing != null)
                throw new ConflictException("Supplier with this company name already exists.");

            return await _supplierRepository.CreateSupplierAsync(supplier);
        }

        public async Task<Supplier> UpdateSupplierAsync(Supplier supplier)
        {
            if (supplier.Id <= 0)
                throw new ValidationException("Supplier id is required.");

            ValidateSupplier(supplier);

            var exists = await _supplierRepository.ExistsByIdAsync(supplier.Id);
            if (!exists)
                throw new NotFoundException("Supplier not found.");

            var duplicate = await _supplierRepository.GetSupplierByCompanyNameAsync(supplier.CompanyName);
                if (duplicate != null && duplicate.Id != supplier.Id)
                throw new ConflictException("Another supplier with this company name already exists.");

            return await _supplierRepository.UpdateSupplierAsync(supplier);
        }

        public async Task DeleteSupplierAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Supplier id must be greater than zero.");

            var deleted = await _supplierRepository.DeleteSupplierAsync(id);
            if (!deleted)
                throw new NotFoundException("Supplier not found.");
        }

        private static void ValidateSupplier(Supplier supplier)
        {
            if (supplier == null)
                throw new ValidationException("Supplier payload is required.");

            if (string.IsNullOrWhiteSpace(supplier.CompanyName))
                throw new ValidationException("Company name is required.");

            if (supplier.CompanyName.Length > 100)
                throw new ValidationException("Company name is too long.");
        }
    }
}
