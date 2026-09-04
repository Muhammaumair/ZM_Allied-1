using ZMAllied.Application.Common;
using ZMAllied.Application.DTOs.Vehicle;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Entities.Fleet;

namespace ZMAllied.Application.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _repository;
        private readonly IVehicleTypeRepository _vehicleTypeRepository;

        public VehicleService(IVehicleRepository repository, IVehicleTypeRepository vehicleTypeRepository)
        {
            _repository = repository;
            _vehicleTypeRepository = vehicleTypeRepository;
        }

        public async Task<VehicleResponseDto?> GetByIdAsync(int id)
        {
            var vehicle = await _repository.GetByIdAsync(id);
            return vehicle is null || vehicle.IsDeleted ? null : Map(vehicle);
        }

        public async Task<PagedResult<VehicleResponseDto>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? search,
            bool? isActive = null)
        {
            (pageNumber, pageSize) = NormalizePagination(pageNumber, pageSize);

            var result = await _repository.GetPagedAsync(pageNumber, pageSize, search, isActive);

            return new PagedResult<VehicleResponseDto>
            {
                Items = result.Items.ConvertAll(Map),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task<VehicleResponseDto> CreateAsync(VehicleCreateDto dto, int? userId)
        {
            await ValidateReferences(dto.VehicleTypeId, dto.OfficeId, dto.RegistrationNumber, null);
            var vehicle = CreateNew(dto, userId);
            var created = await _repository.AddAsync(vehicle);
            return Map(created);
        }

        public async Task<bool> UpdateAsync(int id, VehicleUpdateDto dto, int? userId)
        {
            var vehicle = await _repository.GetByIdAsync(id);

            if (vehicle is null || vehicle.IsDeleted)
            {
                return false;
            }

            await ValidateReferences(dto.VehicleTypeId, dto.OfficeId, dto.RegistrationNumber, id);
            ApplyUpdate(vehicle, dto);
            vehicle.UpdatedAt = DateTime.UtcNow;
            vehicle.UpdatedBy = userId;

            await _repository.UpdateAsync(vehicle);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, int? userId)
        {
            var vehicle = await _repository.GetByIdAsync(id);

            if (vehicle is null || vehicle.IsDeleted)
            {
                return false;
            }

            vehicle.IsDeleted = true;
            vehicle.UpdatedAt = DateTime.UtcNow;
            vehicle.UpdatedBy = userId;

            await _repository.UpdateAsync(vehicle);
            return true;
        }

        #region Private Methods

        private async Task ValidateReferences(int vehicleTypeId, int officeId, string registrationNumber, int? excludeId)
        {
            if (!await _vehicleTypeRepository.ExistsAsync(vehicleTypeId))
            {
                throw new ResourceNotFoundException("The specified vehicle type does not exist.");
            }

            if (!await _repository.OfficeExistsAsync(officeId))
            {
                throw new ResourceNotFoundException("The specified office does not exist.");
            }

            if (await _repository.ExistsByRegistrationNumberAsync(registrationNumber, excludeId))
            {
                throw new DuplicateException($"A vehicle with registration number '{registrationNumber}' already exists.");
            }
        }

        private static Vehicle CreateNew(VehicleCreateDto dto, int? userId)
        {
            return new Vehicle
            {
                VehicleTypeId = dto.VehicleTypeId,
                RegistrationNumber = dto.RegistrationNumber,
                Make = dto.Make,
                Model = dto.Model,
                ChassisNumber = dto.ChassisNumber,
                EngineNumber = dto.EngineNumber,
                Year = dto.Year,
                Capacity = dto.Capacity,
                CurrentMileage = dto.CurrentMileage,
                Status = dto.Status,
                OfficeId = dto.OfficeId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };
        }

        private static void ApplyUpdate(Vehicle vehicle, VehicleUpdateDto dto)
        {
            vehicle.VehicleTypeId = dto.VehicleTypeId;
            vehicle.RegistrationNumber = dto.RegistrationNumber;
            vehicle.Make = dto.Make;
            vehicle.Model = dto.Model;
            vehicle.ChassisNumber = dto.ChassisNumber;
            vehicle.EngineNumber = dto.EngineNumber;
            vehicle.Year = dto.Year;
            vehicle.Capacity = dto.Capacity;
            vehicle.CurrentMileage = dto.CurrentMileage;
            vehicle.Status = dto.Status;
            vehicle.OfficeId = dto.OfficeId;
            vehicle.IsActive = dto.IsActive;
        }

        private static VehicleResponseDto Map(Vehicle vehicle)
        {
            return new VehicleResponseDto
            {
                Id = vehicle.Id,
                VehicleTypeId = vehicle.VehicleTypeId,
                RegistrationNumber = vehicle.RegistrationNumber,
                Make = vehicle.Make,
                Model = vehicle.Model,
                ChassisNumber = vehicle.ChassisNumber,
                EngineNumber = vehicle.EngineNumber,
                Year = vehicle.Year,
                Capacity = vehicle.Capacity,
                CurrentMileage = vehicle.CurrentMileage,
                Status = vehicle.Status,
                OfficeId = vehicle.OfficeId,
                IsActive = vehicle.IsActive,
                CreatedAt = vehicle.CreatedAt,
                CreatedBy = vehicle.CreatedBy,
                UpdatedAt = vehicle.UpdatedAt,
                UpdatedBy = vehicle.UpdatedBy
            };
        }

        private static (int PageNumber, int PageSize) NormalizePagination(int pageNumber, int pageSize)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize > 100 ? 100 : pageSize;
            return (pageNumber, pageSize);
        }

        #endregion
    }
}