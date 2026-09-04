using ZMAllied.Application.Common;
using ZMAllied.Application.DTOs.Vehicle;

namespace ZMAllied.Application.Interfaces
{
    public interface IVehicleService
    {
        Task<VehicleResponseDto?> GetByIdAsync(int id);

        Task<PagedResult<VehicleResponseDto>> GetPagedAsync(int pageNumber,int pageSize, string? search, bool? isActive = null);

        Task<VehicleResponseDto> CreateAsync(VehicleCreateDto dto, int? userId);

        Task<bool> UpdateAsync( int id, VehicleUpdateDto dto, int? userId);

        Task<bool> DeleteAsync(int id, int? userId);
    }
}