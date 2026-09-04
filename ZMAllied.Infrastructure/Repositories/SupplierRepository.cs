using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZMAllied.Application.Common;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Entities.Parties;
using ZMAllied.Infrastructure.Persistence;

namespace ZMAllied.Infrastructure.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly ZMAlliedDbContext _context;

        public SupplierRepository(ZMAlliedDbContext context)
        {
            _context = context;
        }

        public async Task<Supplier?> GetByIdAsync(int id)
        {
            return await _context.Suppliers.FindAsync(id);
        }

        public async Task<PagedResult<Supplier>> GetPagedAsync(int pageNumber, int pageSize, string? search, bool? isActive = null)
        {
            var query = _context.Suppliers.Where(s => !s.IsDeleted);

            if (isActive.HasValue)
            {
                query = query.Where(s => s.IsActive == isActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower();
                query = query.Where(s =>
                    s.Name.ToLower().Contains(searchLower) ||
                    (s.Phone != null && s.Phone.ToLower().Contains(searchLower)) ||
                    (s.Email != null && s.Email.ToLower().Contains(searchLower)) ||
                    (s.NTN != null && s.NTN.ToLower().Contains(searchLower)) ||
                    (s.ContactPerson != null && s.ContactPerson.ToLower().Contains(searchLower)) ||
                    (s.PaymentTerms != null && s.PaymentTerms.ToLower().Contains(searchLower))
                );
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(s => s.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Supplier>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<Supplier> AddAsync(Supplier supplier)
        {
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();
            return supplier;
        }

        public async Task UpdateAsync(Supplier supplier)
        {
            _context.Suppliers.Update(supplier);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            return await _context.Suppliers.AnyAsync(s =>
                !s.IsDeleted &&
                s.Name.ToLower() == name.ToLower() &&
                (!excludeId.HasValue || s.Id != excludeId.Value));
        }

        public async Task<bool> ExistsByPhoneAsync(string phone, int? excludeId = null)
        {
            return await _context.Suppliers.AnyAsync(s =>
                !s.IsDeleted &&
                s.Phone != null &&
                s.Phone.ToLower() == phone.ToLower() &&
                (!excludeId.HasValue || s.Id != excludeId.Value));
        }

        public async Task<bool> ExistsByNtnAsync(string ntn, int? excludeId = null)
        {
            return await _context.Suppliers.AnyAsync(s =>
                !s.IsDeleted &&
                s.NTN != null &&
                s.NTN.ToLower() == ntn.ToLower() &&
                (!excludeId.HasValue || s.Id != excludeId.Value));
        }
    }
}
