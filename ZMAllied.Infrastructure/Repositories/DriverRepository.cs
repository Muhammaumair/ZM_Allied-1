using Microsoft.EntityFrameworkCore;
using ZMAllied.Application.Common;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Entities.Fleet;
using ZMAllied.Infrastructure.Persistence;

namespace ZMAllied.Infrastructure.Repositories
{
    public class DriverRepository : IDriverRepository
    {
        private readonly ZMAlliedDbContext _context;

        public DriverRepository(ZMAlliedDbContext context)
        {
            _context = context;
        }

        public Task<Driver?> GetByIdAsync(int id)
        {
            return _context.Drivers.FindAsync(id).AsTask();
        }

        public async Task<PagedResult<Driver>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? search,
            bool? isActive = null)
        {
            var query = _context.Drivers.Where(e => !e.IsDeleted);

            if (isActive.HasValue)
            {
                query = query.Where(e => e.IsActive == isActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.ToLower();
                query = query.Where(e =>
                    e.Name.ToLower().Contains(searchTerm) ||
                    (e.CNIC != null && e.CNIC.ToLower().Contains(searchTerm)) ||
                    (e.Phone != null && e.Phone.ToLower().Contains(searchTerm)) ||
                    (e.LicenseNumber != null && e.LicenseNumber.ToLower().Contains(searchTerm))
                );
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(e => e.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Driver>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<Driver> AddAsync(Driver entity)
        {
            _context.Drivers.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(Driver entity)
        {
            _context.Drivers.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}