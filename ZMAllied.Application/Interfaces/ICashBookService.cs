using ZMAllied.Application.Common;
using ZMAllied.Application.DTOs.CashBook;

namespace ZMAllied.Application.Interfaces
{
    public interface ICashBookService
    {
        Task<CashBookEntryResponseDto?> GetByIdAsync(int id);

        Task<PagedResult<CashBookEntryResponseDto>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? search,
            DateTime? dateFrom = null,
            DateTime? dateTo = null,
            string? entryType = null,
            string? referenceNumber = null);

        Task<CashBookEntryResponseDto> CreateAsync(CashBookEntryCreateDto dto,int? userId);

        Task<bool> UpdateAsync(int id,CashBookEntryUpdateDto dto,int? userId);

        Task<bool> DeleteAsync(int id, int? userId);

        Task SynchronizePaymentAsync(int paymentId, DateTime date, decimal amount, string referenceNumber, string description, int? userId);

        Task SynchronizeReceiptAsync(int receiptId, DateTime date, decimal amount, string referenceNumber, string description, int? userId);

        Task DeletePaymentEntryAsync(int paymentId, int? userId);

        Task DeleteReceiptEntryAsync(int receiptId, int? userId);
    }
}
