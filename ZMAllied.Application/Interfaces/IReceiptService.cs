using ZMAllied.Application.Common;
using ZMAllied.Application.DTOs.Receipt;
using ZMAllied.Domain.Enums;

namespace ZMAllied.Application.Interfaces
{
    public interface IReceiptService
    {
        Task<ReceiptResponseDto?> GetByIdAsync(int id);

        Task<PagedResult<ReceiptResponseDto>> GetPagedAsync(int pageNumber,int pageSize,string? search,PaymentStatus? status = null);

        Task<ReceiptResponseDto> CreateAsync( ReceiptCreateDto dto, int? userId);

        Task<bool> UpdateAsync(int id, ReceiptUpdateDto dto, int? userId);

        Task<bool> DeleteAsync(int id, int? userId);
    }
}