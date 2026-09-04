using ZMAllied.Application.Common;
using ZMAllied.Application.DTOs.VehicleType;

namespace ZMAllied.Application.Interfaces
{
    public interface IVehicleTypeService
    {
        Task<VehicleTypeResponseDto?> GetByIdAsync(int id);

        Task<PagedResult<VehicleTypeResponseDto>> GetPagedAsync( int pageNumber, int pageSize, string? search, bool? isActive = null);

        Task<VehicleTypeResponseDto> CreateAsync( VehicleTypeCreateDto dto, int? userId);

        Task<bool> UpdateAsync( int id, VehicleTypeUpdateDto dto, int? userId);

        Task<bool> DeleteAsync(int id, int? userId);
    }
}