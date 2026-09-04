using System;
using System.Threading.Tasks;
using ZMAllied.Application.Common;
using ZMAllied.Application.DTOs.Office;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Entities.Organization;

namespace ZMAllied.Application.Services
{
    public class OfficeService : IOfficeService
    {
        private readonly IOfficeRepository _officeRepository;

        public OfficeService(IOfficeRepository officeRepository)
        {
            _officeRepository = officeRepository;
        }

        public async Task<OfficeResponseDto?> GetByIdAsync(int id)
        {
            var office = await _officeRepository.GetByIdAsync(id);
            return office == null || office.IsDeleted ? null : MapToResponse(office);
        }

        public async Task<PagedResult<OfficeResponseDto>> GetPagedAsync(int pageNumber, int pageSize, string? search, bool? isActive = null)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var result = await _officeRepository.GetPagedAsync(pageNumber, pageSize, search, isActive);
            return new PagedResult<OfficeResponseDto>
            {
                Items = result.Items.ConvertAll(MapToResponse),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task<OfficeResponseDto> CreateAsync(OfficeCreateDto dto, int? userId)
        {
            await ValidateCompanyAndCodeAsync(dto.CompanyId, dto.Code, null);

            var office = new Office
            {
                CompanyId = dto.CompanyId,
                Name = dto.Name,
                Code = dto.Code,
                Address = dto.Address,
                City = dto.City,
                Phone = dto.Phone,
                Email = dto.Email,
                IsHeadOffice = dto.IsHeadOffice,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            return MapToResponse(await _officeRepository.AddAsync(office));
        }

        public async Task<bool> UpdateAsync(int id, OfficeUpdateDto dto, int? userId)
        {
            var office = await _officeRepository.GetByIdAsync(id);
            if (office == null || office.IsDeleted)
                return false;

            await ValidateCompanyAndCodeAsync(dto.CompanyId, dto.Code, id);

            office.CompanyId = dto.CompanyId;
            office.Name = dto.Name;
            office.Code = dto.Code;
            office.Address = dto.Address;
            office.City = dto.City;
            office.Phone = dto.Phone;
            office.Email = dto.Email;
            office.IsHeadOffice = dto.IsHeadOffice;
            office.IsActive = dto.IsActive;
            office.UpdatedAt = DateTime.UtcNow;
            office.UpdatedBy = userId;

            await _officeRepository.UpdateAsync(office);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, int? userId)
        {
            var office = await _officeRepository.GetByIdAsync(id);
            if (office == null || office.IsDeleted)
                return false;

            office.IsDeleted = true;
            office.UpdatedAt = DateTime.UtcNow;
            office.UpdatedBy = userId;
            await _officeRepository.UpdateAsync(office);
            return true;
        }

        private async Task ValidateCompanyAndCodeAsync(int companyId, string code, int? excludeId)
        {
            if (!await _officeRepository.CompanyExistsAsync(companyId))
                throw new ResourceNotFoundException("The specified company does not exist.");

            if (await _officeRepository.ExistsByCodeAsync(code, excludeId))
                throw new DuplicateException($"An office with code '{code}' already exists.");
        }

        private static OfficeResponseDto MapToResponse(Office office) => new()
        {
            Id = office.Id,
            CompanyId = office.CompanyId,
            Name = office.Name,
            Code = office.Code,
            Address = office.Address,
            City = office.City,
            Phone = office.Phone,
            Email = office.Email,
            IsHeadOffice = office.IsHeadOffice,
            IsActive = office.IsActive,
            CreatedAt = office.CreatedAt,
            CreatedBy = office.CreatedBy,
            UpdatedAt = office.UpdatedAt,
            UpdatedBy = office.UpdatedBy
        };
    }

    public class ResourceNotFoundException : Exception
    {
        public ResourceNotFoundException(string message) : base(message) { }
    }
}
