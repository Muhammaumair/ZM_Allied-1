using System.Threading.Tasks;
using ZMAllied.Application.Common;
using ZMAllied.Application.DTOs.Office;

namespace ZMAllied.Application.Interfaces
{
    public interface IOfficeService
    {
        Task<OfficeResponseDto?> GetByIdAsync(int id);
        Task<PagedResult<OfficeResponseDto>> GetPagedAsync(int pageNumber, int pageSize, string? search, bool? isActive = null);
        Task<OfficeResponseDto> CreateAsync(OfficeCreateDto dto, int? userId);
        Task<bool> UpdateAsync(int id, OfficeUpdateDto dto, int? userId);
        Task<bool> DeleteAsync(int id, int? userId);
    }
}
