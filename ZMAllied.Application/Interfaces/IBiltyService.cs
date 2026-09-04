using ZMAllied.Application.Common;
using ZMAllied.Application.DTOs.Bilty;
using ZMAllied.Domain.Enums;

namespace ZMAllied.Application.Interfaces
{
    public interface IBiltyService
    {
        Task<BiltyResponseDto?> GetByIdAsync(int id);

        Task<PagedResult<BiltyResponseDto>> GetPagedAsync(int pageNumber,int pageSize,string? search,BiltyStatus? status = null,DateTime? dateFrom = null,DateTime? dateTo = null);

        Task<BiltyResponseDto> CreateAsync(BiltyCreateDto dto, int? userId);

        Task<bool> UpdateAsync(int id, BiltyUpdateDto dto, int? userId);

        Task<bool> DeleteAsync(int id, int? userId);

        Task<byte[]?> GeneratePrintPdfAsync(int id);
    }
}