using ZMAllied.Application.Common;
using ZMAllied.Domain.Entities.Accounting;
using ZMAllied.Domain.Enums;

namespace ZMAllied.Application.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(int id);

        Task<PagedResult<Payment>> GetPagedAsync( int pageNumber, int pageSize, string? search, PaymentStatus? status = null, DateTime? dateFrom = null, DateTime? dateTo = null);

        Task<Payment> AddAsync(Payment entity);

        Task UpdateAsync(Payment entity);

        Task<bool> ExistsByPaymentNumberAsync(string paymentNumber, int? excludeId = null);

        Task<bool> PartyExistsAsync(int id);

        Task<bool> SupplierExistsAsync(int id);
    }
}