using Microsoft.EntityFrameworkCore;
using ZMAllied.Application.Common;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Entities.Fleet;
using ZMAllied.Infrastructure.Persistence;

namespace ZMAllied.Infrastructure.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly ZMAlliedDbContext _context;

        public VehicleRepository(ZMAlliedDbContext context)
        {
            _context = context;
        }

        public Task<Vehicle?> GetByIdAsync(int id)
        {
            return _context.Vehicles.FindAsync(id).AsTask();
        }

        public async Task<PagedResult<Vehicle>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? search,
            bool? isActive = null)
        {
            var query = _context.Vehicles.Where(e => !e.IsDeleted);

            if (isActive.HasValue)
            {
                query = query.Where(e => e.IsActive == isActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.ToLower();
                query = query.Where(e =>
                    e.RegistrationNumber.ToLower().Contains(searchTerm) ||
                    (e.Make != null && e.Make.ToLower().Contains(searchTerm)) ||
                    (e.Model != null && e.Model.ToLower().Contains(searchTerm)) ||
                    (e.ChassisNumber != null && e.ChassisNumber.ToLower().Contains(searchTerm)) ||
                    (e.EngineNumber != null && e.EngineNumber.ToLower().Contains(searchTerm))
                );
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(e => e.RegistrationNumber)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Vehicle>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<Vehicle> AddAsync(Vehicle entity)
        {
            _context.Vehicles.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(Vehicle entity)
        {
            _context.Vehicles.Update(entity);
            await _context.SaveChangesAsync();
        }

        public Task<bool> ExistsByRegistrationNumberAsync(string registrationNumber, int? excludeId = null)
        {
            return _context.Vehicles.AnyAsync(e =>
                !e.IsDeleted &&
                e.RegistrationNumber.ToLower() == registrationNumber.ToLower() &&
                (!excludeId.HasValue || e.Id != excludeId.Value)
            );
        }

        public Task<bool> OfficeExistsAsync(int id)
        {
            return _context.Offices.AnyAsync(e => e.Id == id && !e.IsDeleted);
        }
    }
}