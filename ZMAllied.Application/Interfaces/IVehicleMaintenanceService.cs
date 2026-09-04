using ZMAllied.Application.Common;
using ZMAllied.Application.DTOs.VehicleMaintenance;

namespace ZMAllied.Application.Interfaces
{
    public interface IVehicleMaintenanceService
    {
        Task<VehicleMaintenanceResponseDto?> GetByIdAsync(int id);

        Task<PagedResult<VehicleMaintenanceResponseDto>> GetPagedAsync( int pageNumber, int pageSize, string? search);

        Task<VehicleMaintenanceResponseDto> CreateAsync( VehicleMaintenanceCreateDto dto, int? userId);

        Task<bool> UpdateAsync( int id, VehicleMaintenanceUpdateDto dto, int? userId);

        Task<bool> DeleteAsync(int id, int? userId);
    }
}