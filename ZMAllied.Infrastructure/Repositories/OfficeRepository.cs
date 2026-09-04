using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZMAllied.Application.Common;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Entities.Organization;
using ZMAllied.Infrastructure.Persistence;

namespace ZMAllied.Infrastructure.Repositories
{
    public class OfficeRepository : IOfficeRepository
    {
        private readonly ZMAlliedDbContext _context;

        public OfficeRepository(ZMAlliedDbContext context)
        {
            _context = context;
        }

        public async Task<Office?> GetByIdAsync(int id) => await _context.Offices.FindAsync(id);

        public async Task<PagedResult<Office>> GetPagedAsync(int pageNumber, int pageSize, string? search, bool? isActive = null)
        {
            var query = _context.Offices.Where(o => !o.IsDeleted);

            if (isActive.HasValue)
                query = query.Where(o => o.IsActive == isActive.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower();
                query = query.Where(o =>
                    o.Name.ToLower().Contains(searchLower) ||
                    o.Code.ToLower().Contains(searchLower) ||
                    (o.City != null && o.City.ToLower().Contains(searchLower)) ||
                    (o.Address != null && o.Address.ToLower().Contains(searchLower)) ||
                    (o.Phone != null && o.Phone.ToLower().Contains(searchLower)) ||
                    (o.Email != null && o.Email.ToLower().Contains(searchLower)));
            }

            var totalCount = await query.CountAsync();
            var items = await query.OrderBy(o => o.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Office>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<Office> AddAsync(Office office)
        {
            _context.Offices.Add(office);
            await _context.SaveChangesAsync();
            return office;
        }

        public async Task UpdateAsync(Office office)
        {
            _context.Offices.Update(office);
            await _context.SaveChangesAsync();
        }

        public Task<bool> ExistsByCodeAsync(string code, int? excludeId = null) => _context.Offices.AnyAsync(o =>
            !o.IsDeleted &&
            o.Code.ToLower() == code.ToLower() &&
            (!excludeId.HasValue || o.Id != excludeId.Value));

        public Task<bool> CompanyExistsAsync(int companyId) =>
            _context.Companies.AnyAsync(c => c.Id == companyId && !c.IsDeleted);
    }
}
