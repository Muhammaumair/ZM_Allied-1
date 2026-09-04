using ZMAllied.Application.Common;
using ZMAllied.Application.DTOs.Payment;
using ZMAllied.Domain.Enums;

namespace ZMAllied.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto?> GetByIdAsync(int id);

        Task<PagedResult<PaymentResponseDto>> GetPagedAsync(int pageNumber, int pageSize, string? search, PaymentStatus? status = null, DateTime? dateFrom = null, DateTime? dateTo = null);

        Task<PaymentResponseDto> CreateAsync( PaymentCreateDto dto, int? userId);

        Task<bool> UpdateAsync( int id, PaymentUpdateDto dto, int? userId);

        Task<bool> DeleteAsync(int id, int? userId);
    }
}