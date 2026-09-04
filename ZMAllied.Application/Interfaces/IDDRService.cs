using ZMAllied.Application.Common;
using ZMAllied.Application.DTOs.DDR;

namespace ZMAllied.Application.Interfaces
{
    public interface IDDRService
    {
        Task<DDRResponseDto?> GetByIdAsync(int id);

        Task<PagedResult<DDRResponseDto>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? search,
            DateTime? dateFrom = null,
            DateTime? dateTo = null,
            int? companyId = null,
            int? vehicleId = null,
            int? biltyId = null,
            int? brokerId = null,
            int? partyId = null);

        Task<DDRResponseDto> CreateAsync(DDRCreateDto dto, int? userId);

        Task<bool> UpdateAsync( int id, DDRUpdateDto dto, int? userId);

        Task<bool> DeleteAsync(int id, int? userId);
    }
}
