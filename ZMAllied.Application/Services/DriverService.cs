using ZMAllied.Application.Common;
using ZMAllied.Application.DTOs.Driver;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Entities.Fleet;

namespace ZMAllied.Application.Services
{
    public class DriverService : IDriverService
    {
        private readonly IDriverRepository _repository;

        public DriverService(IDriverRepository repository)
        {
            _repository = repository;
        }

        public async Task<DriverResponseDto?> GetByIdAsync(int id)
        {
            var driver = await _repository.GetByIdAsync(id);
            return driver is null || driver.IsDeleted ? null : Map(driver);
        }

        public async Task<PagedResult<DriverResponseDto>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? search,
            bool? isActive = null)
        {
            (pageNumber, pageSize) = NormalizePagination(pageNumber, pageSize);

            var result = await _repository.GetPagedAsync(pageNumber, pageSize, search, isActive);

            return new PagedResult<DriverResponseDto>
            {
                Items = result.Items.ConvertAll(Map),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task<DriverResponseDto> CreateAsync(DriverCreateDto dto, int? userId)
        {
            var driver = new Driver
            {
                Name = dto.Name,
                CNIC = dto.CNIC,
                Phone = dto.Phone,
                LicenseNumber = dto.LicenseNumber,
                LicenseExpiryDate = dto.LicenseExpiryDate,
                Address = dto.Address,
                JoiningDate = dto.JoiningDate,
                Status = dto.Status,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            var created = await _repository.AddAsync(driver);
            return Map(created);
        }

        public async Task<bool> UpdateAsync(int id, DriverUpdateDto dto, int? userId)
        {
            var driver = await _repository.GetByIdAsync(id);

            if (driver is null || driver.IsDeleted)
            {
                return false;
            }

            driver.Name = dto.Name;
            driver.CNIC = dto.CNIC;
            driver.Phone = dto.Phone;
            driver.LicenseNumber = dto.LicenseNumber;
            driver.LicenseExpiryDate = dto.LicenseExpiryDate;
            driver.Address = dto.Address;
            driver.JoiningDate = dto.JoiningDate;
            driver.Status = dto.Status;
            driver.IsActive = dto.IsActive;
            driver.UpdatedAt = DateTime.UtcNow;
            driver.UpdatedBy = userId;

            await _repository.UpdateAsync(driver);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, int? userId)
        {
            var driver = await _repository.GetByIdAsync(id);

            if (driver is null || driver.IsDeleted)
            {
                return false;
            }

            driver.IsDeleted = true;
            driver.UpdatedAt = DateTime.UtcNow;
            driver.UpdatedBy = userId;

            await _repository.UpdateAsync(driver);
            return true;
        }

        #region Private Methods

        private static (int PageNumber, int PageSize) NormalizePagination(int pageNumber, int pageSize)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize > 100 ? 100 : pageSize;
            return (pageNumber, pageSize);
        }

        private static DriverResponseDto Map(Driver driver)
        {
            return new DriverResponseDto
            {
                Id = driver.Id,
                Name = driver.Name,
                CNIC = driver.CNIC,
                Phone = driver.Phone,
                LicenseNumber = driver.LicenseNumber,
                LicenseExpiryDate = driver.LicenseExpiryDate,
                Address = driver.Address,
                JoiningDate = driver.JoiningDate,
                Status = driver.Status,
                IsActive = driver.IsActive,
                CreatedAt = driver.CreatedAt,
                CreatedBy = driver.CreatedBy,
                UpdatedAt = driver.UpdatedAt,
                UpdatedBy = driver.UpdatedBy
            };
        }

        #endregion
    }
}