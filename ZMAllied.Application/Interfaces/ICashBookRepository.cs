using ZMAllied.Application.Common;
using ZMAllied.Domain.Entities.Accounting;

namespace ZMAllied.Application.Interfaces
{
    public interface ICashBookRepository
    {
        Task<CashBookEntry?> GetByIdAsync(int id);

        Task<PagedResult<CashBookEntry>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? search,
            DateTime? dateFrom = null,
            DateTime? dateTo = null,
            string? entryType = null,
            string? referenceNumber = null);

        Task<CashBookEntry?> GetByPaymentIdAsync(int paymentId);

        Task<CashBookEntry?> GetByReceiptIdAsync(int receiptId);

        Task<CashBookEntry> AddAsync(CashBookEntry entity);

        Task UpdateAsync(CashBookEntry entity);

        Task<bool> PaymentExistsAsync(int id);

        Task<bool> ReceiptExistsAsync(int id);
    }
}
