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

        /// <summary>
        /// Retrieves the supplier with the specified identifier.
        /// </summary>
        /// <param name="id">The supplier identifier; must be greater than zero.</param>
        /// <returns>The supplier that matches the specified identifier.</returns>
        /// <exception cref="ValidationException">Thrown when <paramref name="id"/> is less than or equal to zero.</exception>
        /// <exception cref="NotFoundException">Thrown when no supplier with the specified identifier exists.</exception>
        public async Task<Supplier> GetSupplierByIdAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("Supplier id must be greater than zero.");

            var supplier = await _supplierRepository.GetSupplierByIdAsync(id);
            if (supplier == null)
                throw new NotFoundException($"Supplier with id {id} not found.");

            return supplier;
        }

        /// <summary>
        /// Retrieves a supplier by its company name.
        /// </summary>
        /// <param name="companyName">The supplier's company name; must not be null, empty, or whitespace.</param>
        /// <returns>The matching <see cref="Supplier"/>.</returns>
        /// <exception cref="ValidationException">Thrown when <paramref name="companyName"/> is null, empty, or whitespace.</exception>
        /// <exception cref="NotFoundException">Thrown when no supplier with the specified company name exists.</exception>
        public async Task<Supplier> GetSupplierByCompanyNameAsync(string companyName)
        {
            if (string.IsNullOrWhiteSpace(companyName))
                throw new ValidationException("Company name is required.");

            var supplier = await _supplierRepository.GetSupplierByCompanyNameAsync(companyName);
            if (supplier == null)
                throw new NotFoundException($"Supplier with company name '{companyName}' not found.");

            return supplier;
        }

        /// <summary>
        /// Creates a new supplier when the provided supplier is valid and no other supplier exists with the same company name.
        /// </summary>
        /// <param name="supplier">Supplier entity to create; CompanyName must be non-empty and at most 100 characters.</param>
        /// <returns>The created <see cref="Supplier"/>.</returns>
        /// <exception cref="ValidationException">Thrown when <paramref name="supplier"/> is null, CompanyName is null/empty/whitespace, or CompanyName exceeds 100 characters.</exception>
        /// <exception cref="ConflictException">Thrown when a supplier with the same company name already exists.</exception>
        public async Task<Supplier> CreateSupplierAsync(Supplier supplier)
        {
            ValidateSupplier(supplier);

            var existing = await _supplierRepository.GetSupplierByCompanyNameAsync(supplier.CompanyName);
            if (existing != null)
                throw new ConflictException("Supplier with this company name already exists.");

            return await _supplierRepository.CreateSupplierAsync(supplier);
        }

        /// <summary>
        /// Updates an existing supplier after validating the payload and enforcing unique company names.
        /// </summary>
        /// <param name="supplier">The supplier entity containing updated values; its Id must be greater than zero.</param>
        /// <returns>The updated Supplier.</returns>
        /// <exception cref="ValidationException">Thrown if the supplier is null, has an invalid CompanyName, or if Id is not greater than zero.</exception>
        /// <exception cref="NotFoundException">Thrown if no supplier exists with the specified Id.</exception>
        /// <exception cref="ConflictException">Thrown if another supplier with the same company name already exists.</exception>
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