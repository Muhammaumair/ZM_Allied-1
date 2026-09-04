using System;
using System.Threading.Tasks;
using ZMAllied.Application.Common;
using ZMAllied.Application.DTOs.VehicleMaintenance;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Entities.Fleet;

namespace ZMAllied.Application.Services
{
    public class VehicleMaintenanceService : IVehicleMaintenanceService
    {
        private readonly IVehicleMaintenanceRepository _maintenanceRepository;

        public VehicleMaintenanceService(IVehicleMaintenanceRepository maintenanceRepository)
        {
            _maintenanceRepository = maintenanceRepository;
        }

        public async Task<VehicleMaintenanceResponseDto?> GetByIdAsync(int id)
        {
            var item = await _maintenanceRepository.GetByIdAsync(id);
            if (item == null || item.IsDeleted)
                return null;

            return MapToResponse(item);
        }

        public async Task<PagedResult<VehicleMaintenanceResponseDto>> GetPagedAsync(int pageNumber, int pageSize, string? search)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var result = await _maintenanceRepository.GetPagedAsync(pageNumber, pageSize, search);

            return new PagedResult<VehicleMaintenanceResponseDto>
            {
                Items = result.Items.ConvertAll(MapToResponse),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task<VehicleMaintenanceResponseDto> CreateAsync(VehicleMaintenanceCreateDto dto, int? userId)
        {
            if (!await _maintenanceRepository.VehicleExistsAsync(dto.VehicleId))
                throw new InvalidOperationException($"Vehicle with ID {dto.VehicleId} does not exist.");

            if (dto.SupplierId.HasValue && !await _maintenanceRepository.SupplierExistsAsync(dto.SupplierId.Value))
                throw new InvalidOperationException($"Supplier with ID {dto.SupplierId.Value} does not exist.");

            var item = new VehicleMaintenance
            {
                VehicleId = dto.VehicleId,
                MaintenanceDate = dto.MaintenanceDate == default ? DateTime.UtcNow : dto.MaintenanceDate,
                MaintenanceType = dto.MaintenanceType,
                Description = dto.Description,
                Mileage = dto.Mileage,
                Cost = dto.Cost,
                SupplierId = dto.SupplierId,
                NextMaintenanceDate = dto.NextMaintenanceDate,
                Remarks = dto.Remarks,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            var created = await _maintenanceRepository.AddAsync(item);
            return MapToResponse(created);
        }

        public async Task<bool> UpdateAsync(int id, VehicleMaintenanceUpdateDto dto, int? userId)
        {
            var item = await _maintenanceRepository.GetByIdAsync(id);
            if (item == null || item.IsDeleted)
                return false;

            if (!await _maintenanceRepository.VehicleExistsAsync(dto.VehicleId))
                throw new InvalidOperationException($"Vehicle with ID {dto.VehicleId} does not exist.");

            if (dto.SupplierId.HasValue && !await _maintenanceRepository.SupplierExistsAsync(dto.SupplierId.Value))
                throw new InvalidOperationException($"Supplier with ID {dto.SupplierId.Value} does not exist.");

            item.VehicleId = dto.VehicleId;
            item.MaintenanceDate = dto.MaintenanceDate;
            item.MaintenanceType = dto.MaintenanceType;
            item.Description = dto.Description;
            item.Mileage = dto.Mileage;
            item.Cost = dto.Cost;
            item.SupplierId = dto.SupplierId;
            item.NextMaintenanceDate = dto.NextMaintenanceDate;
            item.Remarks = dto.Remarks;
            item.UpdatedAt = DateTime.UtcNow;
            item.UpdatedBy = userId;

            await _maintenanceRepository.UpdateAsync(item);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, int? userId)
        {
            var item = await _maintenanceRepository.GetByIdAsync(id);
            if (item == null || item.IsDeleted)
                return false;

            item.IsDeleted = true;
            item.UpdatedAt = DateTime.UtcNow;
            item.UpdatedBy = userId;

            await _maintenanceRepository.UpdateAsync(item);
            return true;
        }

        private static VehicleMaintenanceResponseDto MapToResponse(VehicleMaintenance item)
        {
            return new VehicleMaintenanceResponseDto
            {
                Id = item.Id,
                VehicleId = item.VehicleId,
                MaintenanceDate = item.MaintenanceDate,
                MaintenanceType = item.MaintenanceType,
                Description = item.Description,
                Mileage = item.Mileage,
                Cost = item.Cost,
                SupplierId = item.SupplierId,
                NextMaintenanceDate = item.NextMaintenanceDate,
                Remarks = item.Remarks
            };
        }
    }
}
