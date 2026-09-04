using System.Threading.Tasks;
using ZMAllied.Application.Common;
using ZMAllied.Application.DTOs.Supplier;

namespace ZMAllied.Application.Interfaces
{
    public interface ISupplierService
    {
        Task<SupplierResponseDto?> GetByIdAsync(int id);
        Task<PagedResult<SupplierResponseDto>> GetPagedAsync(int pageNumber, int pageSize, string? search, bool? isActive = null);
        Task<SupplierResponseDto> CreateAsync(SupplierCreateDto dto, int? userId);
        Task<bool> UpdateAsync(int id, SupplierUpdateDto dto, int? userId);
        Task<bool> DeleteAsync(int id, int? userId);
    }
}
