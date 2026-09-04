using Microsoft.EntityFrameworkCore;
using ZMAllied.Application.Common;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Entities.Accounting;
using ZMAllied.Infrastructure.Persistence;

namespace ZMAllied.Infrastructure.Repositories
{
    public class CashBookRepository : ICashBookRepository
    {
        private readonly ZMAlliedDbContext _context;

        public CashBookRepository(ZMAlliedDbContext context)
        {
            _context = context;
        }

        public Task<CashBookEntry?> GetByIdAsync(int id) =>
            _context.CashBookEntries.FirstOrDefaultAsync(entry => entry.Id == id);

        public async Task<PagedResult<CashBookEntry>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? search,
            DateTime? dateFrom = null,
            DateTime? dateTo = null,
            string? entryType = null,
            string? referenceNumber = null)
        {
            var query = _context.CashBookEntries.Where(entry => !entry.IsDeleted);

            if (dateFrom.HasValue)
            {
                query = query.Where(entry => entry.Date >= dateFrom.Value);
            }

            if (dateTo.HasValue)
            {
                query = query.Where(entry => entry.Date <= dateTo.Value);
            }

            if (entryType?.Equals("debit", StringComparison.OrdinalIgnoreCase) == true)
            {
                query = query.Where(entry => entry.Debit > 0m);
            }
            else if (entryType?.Equals("credit", StringComparison.OrdinalIgnoreCase) == true)
            {
                query = query.Where(entry => entry.Credit > 0m);
            }

            if (!string.IsNullOrWhiteSpace(referenceNumber))
            {
                var referenceSearch = referenceNumber.ToLower();
                query = query.Where(entry => (entry.ReferenceNumber ?? string.Empty).ToLower().Contains(referenceSearch));
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.ToLower();
                query = query.Where(entry =>
                    (entry.Description ?? string.Empty).ToLower().Contains(searchTerm) ||
                    (entry.ReferenceType ?? string.Empty).ToLower().Contains(searchTerm) ||
                    (entry.ReferenceNumber ?? string.Empty).ToLower().Contains(searchTerm) ||
                    (entry.Remarks ?? string.Empty).ToLower().Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(entry => entry.Date)
                .ThenBy(entry => entry.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<CashBookEntry>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public Task<CashBookEntry?> GetByPaymentIdAsync(int paymentId) =>
            _context.CashBookEntries.FirstOrDefaultAsync(entry => entry.PaymentId == paymentId);

        public Task<CashBookEntry?> GetByReceiptIdAsync(int receiptId) =>
            _context.CashBookEntries.FirstOrDefaultAsync(entry => entry.ReceiptId == receiptId);

        public async Task<CashBookEntry> AddAsync(CashBookEntry entity)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            _context.CashBookEntries.Add(entity);
            await _context.SaveChangesAsync();
            await RecalculateBalancesAsync();
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return entity;
        }

        public async Task UpdateAsync(CashBookEntry entity)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            _context.CashBookEntries.Update(entity);
            await _context.SaveChangesAsync();
            await RecalculateBalancesAsync();
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        public Task<bool> PaymentExistsAsync(int id) =>
            _context.Payments.AnyAsync(payment => payment.Id == id && !payment.IsDeleted);

        public Task<bool> ReceiptExistsAsync(int id) =>
            _context.Receipts.AnyAsync(receipt => receipt.Id == id && !receipt.IsDeleted);

        private async Task RecalculateBalancesAsync()
        {
            var entries = await _context.CashBookEntries
                .Where(entry => !entry.IsDeleted)
                .OrderBy(entry => entry.Date)
                .ThenBy(entry => entry.Id)
                .ToListAsync();

            var runningBalance = 0m;
            foreach (var entry in entries)
            {
                runningBalance += entry.Debit - entry.Credit;
                entry.Balance = runningBalance;
            }
        }
    }
}
