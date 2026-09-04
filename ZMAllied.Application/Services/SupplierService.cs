using System;
using System.Threading.Tasks;
using ZMAllied.Application.Common;
using ZMAllied.Application.DTOs.Supplier;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Entities.Parties;

namespace ZMAllied.Application.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;

        public SupplierService(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public async Task<SupplierResponseDto?> GetByIdAsync(int id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null || supplier.IsDeleted)
                return null;

            return MapToResponse(supplier);
        }

        public async Task<PagedResult<SupplierResponseDto>> GetPagedAsync(int pageNumber, int pageSize, string? search, bool? isActive = null)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var result = await _supplierRepository.GetPagedAsync(pageNumber, pageSize, search, isActive);

            return new PagedResult<SupplierResponseDto>
            {
                Items = result.Items.ConvertAll(MapToResponse),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task<SupplierResponseDto> CreateAsync(SupplierCreateDto dto, int? userId)
        {
            await ValidateDuplicates(dto.Name, dto.Phone, dto.NTN, null);

            var supplier = new Supplier
            {
                Name = dto.Name,
                ContactPerson = dto.ContactPerson,
                Phone = dto.Phone,
                Email = dto.Email,
                Address = dto.Address,
                NTN = dto.NTN,
                PaymentTerms = dto.PaymentTerms,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            var created = await _supplierRepository.AddAsync(supplier);
            return MapToResponse(created);
        }

        public async Task<bool> UpdateAsync(int id, SupplierUpdateDto dto, int? userId)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null || supplier.IsDeleted)
                return false;

            await ValidateDuplicates(dto.Name, dto.Phone, dto.NTN, id);

            supplier.Name = dto.Name;
            supplier.ContactPerson = dto.ContactPerson;
            supplier.Phone = dto.Phone;
            supplier.Email = dto.Email;
            supplier.Address = dto.Address;
            supplier.NTN = dto.NTN;
            supplier.PaymentTerms = dto.PaymentTerms;
            supplier.IsActive = dto.IsActive;
            supplier.UpdatedAt = DateTime.UtcNow;
            supplier.UpdatedBy = userId;

            await _supplierRepository.UpdateAsync(supplier);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, int? userId)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null || supplier.IsDeleted)
                return false;

            supplier.IsDeleted = true;
            supplier.UpdatedAt = DateTime.UtcNow;
            supplier.UpdatedBy = userId;

            await _supplierRepository.UpdateAsync(supplier);
            return true;
        }

        private async Task ValidateDuplicates(string name, string? phone, string? ntn, int? excludeId)
        {
            if (await _supplierRepository.ExistsByNameAsync(name, excludeId))
                throw new DuplicateException($"A supplier with name '{name}' already exists.");

            if (!string.IsNullOrWhiteSpace(phone) && await _supplierRepository.ExistsByPhoneAsync(phone, excludeId))
                throw new DuplicateException($"A supplier with phone '{phone}' already exists.");

            if (!string.IsNullOrWhiteSpace(ntn) && await _supplierRepository.ExistsByNtnAsync(ntn, excludeId))
                throw new DuplicateException($"A supplier with NTN '{ntn}' already exists.");
        }

        private static SupplierResponseDto MapToResponse(Supplier supplier)
        {
            return new SupplierResponseDto
            {
                Id = supplier.Id,
                Name = supplier.Name,
                ContactPerson = supplier.ContactPerson,
                Phone = supplier.Phone,
                Email = supplier.Email,
                Address = supplier.Address,
                NTN = supplier.NTN,
                PaymentTerms = supplier.PaymentTerms,
                IsActive = supplier.IsActive,
                CreatedAt = supplier.CreatedAt,
                CreatedBy = supplier.CreatedBy,
                UpdatedAt = supplier.UpdatedAt,
                UpdatedBy = supplier.UpdatedBy
            };
        }
    }
}
