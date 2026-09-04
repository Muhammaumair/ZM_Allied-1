using ZMAllied.Application.Common;
using ZMAllied.Application.DTOs.Driver;

namespace ZMAllied.Application.Interfaces
{
    public interface IDriverService
    {
        Task<DriverResponseDto?> GetByIdAsync(int id);

        Task<PagedResult<DriverResponseDto>> GetPagedAsync(int pageNumber,int pageSize,string? search,bool? isActive = null);

        Task<DriverResponseDto> CreateAsync(DriverCreateDto dto,int? userId);

        Task<bool> UpdateAsync(int id,DriverUpdateDto dto,int? userId);

        Task<bool> DeleteAsync(int id, int? userId);
    }
}