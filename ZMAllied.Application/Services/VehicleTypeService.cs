using ZMAllied.Application.Common;
using ZMAllied.Application.DTOs.VehicleType;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Entities.Fleet;

namespace ZMAllied.Application.Services
{
    public class VehicleTypeService : IVehicleTypeService
    {
        private readonly IVehicleTypeRepository _repository;

        public VehicleTypeService(IVehicleTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<VehicleTypeResponseDto?> GetByIdAsync(int id)
        {
            var vehicleType = await _repository.GetByIdAsync(id);
            return vehicleType is null || vehicleType.IsDeleted ? null : Map(vehicleType);
        }

        public async Task<PagedResult<VehicleTypeResponseDto>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? search,
            bool? isActive = null)
        {
            (pageNumber, pageSize) = NormalizePagination(pageNumber, pageSize);

            var result = await _repository.GetPagedAsync(pageNumber, pageSize, search, isActive);

            return new PagedResult<VehicleTypeResponseDto>
            {
                Items = result.Items.ConvertAll(Map),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task<VehicleTypeResponseDto> CreateAsync(VehicleTypeCreateDto dto, int? userId)
        {
            var vehicleType = new VehicleType
            {
                Name = dto.Name,
                Description = dto.Description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            var created = await _repository.AddAsync(vehicleType);
            return Map(created);
        }

        public async Task<bool> UpdateAsync(int id, VehicleTypeUpdateDto dto, int? userId)
        {
            var vehicleType = await _repository.GetByIdAsync(id);

            if (vehicleType is null || vehicleType.IsDeleted)
            {
                return false;
            }

            vehicleType.Name = dto.Name;
            vehicleType.Description = dto.Description;
            vehicleType.IsActive = dto.IsActive;
            vehicleType.UpdatedAt = DateTime.UtcNow;
            vehicleType.UpdatedBy = userId;

            await _repository.UpdateAsync(vehicleType);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, int? userId)
        {
            var vehicleType = await _repository.GetByIdAsync(id);

            if (vehicleType is null || vehicleType.IsDeleted)
            {
                return false;
            }

            vehicleType.IsDeleted = true;
            vehicleType.UpdatedAt = DateTime.UtcNow;
            vehicleType.UpdatedBy = userId;

            await _repository.UpdateAsync(vehicleType);
            return true;
        }

        #region Private Methods

        private static (int PageNumber, int PageSize) NormalizePagination(int pageNumber, int pageSize)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize > 100 ? 100 : pageSize;
            return (pageNumber, pageSize);
        }

        private static VehicleTypeResponseDto Map(VehicleType vehicleType)
        {
            return new VehicleTypeResponseDto
            {
                Id = vehicleType.Id,
                Name = vehicleType.Name,
                Description = vehicleType.Description,
                IsActive = vehicleType.IsActive,
                CreatedAt = vehicleType.CreatedAt,
                CreatedBy = vehicleType.CreatedBy,
                UpdatedAt = vehicleType.UpdatedAt,
                UpdatedBy = vehicleType.UpdatedBy
            };
        }

        #endregion
    }
}