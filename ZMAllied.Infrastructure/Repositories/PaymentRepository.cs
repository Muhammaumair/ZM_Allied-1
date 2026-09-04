using Microsoft.EntityFrameworkCore;
using ZMAllied.Application.Common;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Entities.Accounting;
using ZMAllied.Domain.Enums;
using ZMAllied.Infrastructure.Persistence;

namespace ZMAllied.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly ZMAlliedDbContext _context;

        public PaymentRepository(ZMAlliedDbContext context)
        {
            _context = context;
        }

        public Task<Payment?> GetByIdAsync(int id)
        {
            return _context.Payments.FindAsync(id).AsTask();
        }

        public async Task<PagedResult<Payment>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? search,
            PaymentStatus? status = null,
            DateTime? dateFrom = null,
            DateTime? dateTo = null)
        {
            var query = _context.Payments.Where(e => !e.IsDeleted);

            if (status.HasValue)
            {
                query = query.Where(e => e.Status == status);
            }

            if (dateFrom.HasValue)
            {
                query = query.Where(e => e.Date >= dateFrom);
            }

            if (dateTo.HasValue)
            {
                query = query.Where(e => e.Date <= dateTo);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.ToLower();
                query = query.Where(e =>
                    e.PaymentNumber.ToLower().Contains(searchTerm) ||
                    (e.Description ?? "").ToLower().Contains(searchTerm) ||
                    (e.ReferenceNumber ?? "").ToLower().Contains(searchTerm)
                );
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(e => e.Date)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Payment>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<Payment> AddAsync(Payment entity)
        {
            _context.Payments.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(Payment entity)
        {
            _context.Payments.Update(entity);
            await _context.SaveChangesAsync();
        }

        public Task<bool> ExistsByPaymentNumberAsync(string paymentNumber, int? excludeId = null)
        {
            return _context.Payments.AnyAsync(x =>
                !x.IsDeleted &&
                x.PaymentNumber.ToLower() == paymentNumber.ToLower() &&
                (!excludeId.HasValue || x.Id != excludeId)
            );
        }

        public Task<bool> PartyExistsAsync(int id)
        {
            return _context.Parties.AnyAsync(x => x.Id == id && !x.IsDeleted);
        }

        public Task<bool> SupplierExistsAsync(int id)
        {
            return _context.Suppliers.AnyAsync(x => x.Id == id && !x.IsDeleted);
        }
    }
}