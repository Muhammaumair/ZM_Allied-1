using Microsoft.EntityFrameworkCore;
using ZMAllied.Application.Common;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Entities.Fleet;
using ZMAllied.Infrastructure.Persistence;

namespace ZMAllied.Infrastructure.Repositories
{
    public class VehicleTypeRepository : IVehicleTypeRepository
    {
        private readonly ZMAlliedDbContext _context;

        public VehicleTypeRepository(ZMAlliedDbContext context)
        {
            _context = context;
        }

        public Task<VehicleType?> GetByIdAsync(int id)
        {
            return _context.VehicleTypes.FindAsync(id).AsTask();
        }

        public async Task<PagedResult<VehicleType>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? search,
            bool? isActive = null)
        {
            var query = _context.VehicleTypes.Where(e => !e.IsDeleted);

            if (isActive.HasValue)
            {
                query = query.Where(e => e.IsActive == isActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.ToLower();
                query = query.Where(e =>
                    e.Name.ToLower().Contains(searchTerm) ||
                    (e.Description != null && e.Description.ToLower().Contains(searchTerm))
                );
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(e => e.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<VehicleType>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<VehicleType> AddAsync(VehicleType entity)
        {
            _context.VehicleTypes.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(VehicleType entity)
        {
            _context.VehicleTypes.Update(entity);
            await _context.SaveChangesAsync();
        }

        public Task<bool> ExistsAsync(int id)
        {
            return _context.VehicleTypes.AnyAsync(e => e.Id == id && !e.IsDeleted);
        }
    }
}