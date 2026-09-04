using Microsoft.EntityFrameworkCore;
using ZMAllied.Application.Common;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Entities.Fleet;
using ZMAllied.Infrastructure.Persistence;

namespace ZMAllied.Infrastructure.Repositories
{
    public class VehicleMaintenanceRepository : IVehicleMaintenanceRepository
    {
        private readonly ZMAlliedDbContext _context;

        public VehicleMaintenanceRepository(ZMAlliedDbContext context)
        {
            _context = context;
        }

        public Task<VehicleMaintenance?> GetByIdAsync(int id)
        {
            return _context.VehicleMaintenances.FindAsync(id).AsTask();
        }

        public async Task<PagedResult<VehicleMaintenance>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? search)
        {
            var query = _context.VehicleMaintenances.Where(e => !e.IsDeleted);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.ToLower();
                query = query.Where(e =>
                    (e.MaintenanceType ?? "").ToLower().Contains(searchTerm) ||
                    (e.Description ?? "").ToLower().Contains(searchTerm) ||
                    (e.Remarks ?? "").ToLower().Contains(searchTerm)
                );
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(e => e.MaintenanceDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<VehicleMaintenance>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<VehicleMaintenance> AddAsync(VehicleMaintenance entity)
        {
            _context.VehicleMaintenances.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(VehicleMaintenance entity)
        {
            _context.VehicleMaintenances.Update(entity);
            await _context.SaveChangesAsync();
        }

        public Task<bool> VehicleExistsAsync(int id)
        {
            return _context.Vehicles.AnyAsync(x => x.Id == id && !x.IsDeleted);
        }

        public Task<bool> SupplierExistsAsync(int id)
        {
            return _context.Suppliers.AnyAsync(x => x.Id == id && !x.IsDeleted);
        }
    }
}