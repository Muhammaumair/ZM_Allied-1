using ZMAllied.Application.Common;
using ZMAllied.Domain.Entities.Accounting;
using ZMAllied.Domain.Enums;

namespace ZMAllied.Application.Interfaces
{
    public interface IReceiptRepository
    {
        Task<Receipt?> GetByIdAsync(int id);

        Task<PagedResult<Receipt>> GetPagedAsync(int pageNumber, int pageSize, string? search, PaymentStatus? status);

        Task<Receipt> AddAsync(Receipt entity);

        Task UpdateAsync(Receipt entity);

        Task<bool> ExistsByReceiptNumberAsync(string receiptNumber, int? excludeId = null);

        Task<bool> PartyExistsAsync(int id);
    }
}