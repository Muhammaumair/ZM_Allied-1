using Microsoft.EntityFrameworkCore;
using ZMAllied.Application.Common;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Entities.Accounting;
using ZMAllied.Domain.Enums;
using ZMAllied.Infrastructure.Persistence;

namespace ZMAllied.Infrastructure.Repositories
{
    public class ReceiptRepository : IReceiptRepository
    {
        private readonly ZMAlliedDbContext _context;

        public ReceiptRepository(ZMAlliedDbContext context)
        {
            _context = context;
        }

        public Task<Receipt?> GetByIdAsync(int id)
        {
            return _context.Receipts.FindAsync(id).AsTask();
        }

        public async Task<PagedResult<Receipt>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? search,
            PaymentStatus? status)
        {
            var query = _context.Receipts.Where(e => !e.IsDeleted);

            if (status.HasValue)
            {
                query = query.Where(e => e.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.ToLower();
                query = query.Where(e =>
                    e.ReceiptNumber.ToLower().Contains(searchTerm)
                );
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(e => e.Date)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Receipt>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<Receipt> AddAsync(Receipt entity)
        {
            _context.Receipts.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(Receipt entity)
        {
            _context.Receipts.Update(entity);
            await _context.SaveChangesAsync();
        }

        public Task<bool> ExistsByReceiptNumberAsync(string receiptNumber, int? excludeId = null)
        {
            return _context.Receipts.AnyAsync(x =>
                !x.IsDeleted &&
                x.ReceiptNumber == receiptNumber &&
                (!excludeId.HasValue || x.Id != excludeId)
            );
        }

        public Task<bool> PartyExistsAsync(int id)
        {
            return _context.Parties.AnyAsync(x => x.Id == id && !x.IsDeleted);
        }
    }
}