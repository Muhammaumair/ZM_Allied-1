using Microsoft.EntityFrameworkCore;
using ZMAllied.Application.Common;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Entities.Trips;
using ZMAllied.Domain.Enums;
using ZMAllied.Infrastructure.Persistence;

namespace ZMAllied.Infrastructure.Repositories
{
    public class TripRepository : ITripRepository
    {
        private readonly ZMAlliedDbContext _context;

        public TripRepository(ZMAlliedDbContext context)
        {
            _context = context;
        }

        public Task<Trip?> GetByIdAsync(int id)
        {
            return _context.Trips.FindAsync(id).AsTask();
        }

        public async Task<PagedResult<Trip>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? search,
            TripStatus? status = null,
            DateTime? dateFrom = null,
            DateTime? dateTo = null)
        {
            var query = _context.Trips.Where(e => !e.IsDeleted);

            if (status.HasValue)
            {
                query = query.Where(e => e.Status == status.Value);
            }

            if (dateFrom.HasValue)
            {
                query = query.Where(e => e.TripDate >= dateFrom.Value);
            }

            if (dateTo.HasValue)
            {
                query = query.Where(e => e.TripDate <= dateTo.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.ToLower();
                query = query.Where(e =>
                    e.TripNumber.ToLower().Contains(searchTerm) ||
                    (e.LoadingPoint != null && e.LoadingPoint.ToLower().Contains(searchTerm)) ||
                    (e.OffloadingPoint != null && e.OffloadingPoint.ToLower().Contains(searchTerm)) ||
                    e.Party.Name.ToLower().Contains(searchTerm) ||
                    e.Vehicle.RegistrationNumber.ToLower().Contains(searchTerm) ||
                    e.Driver.Name.ToLower().Contains(searchTerm)
                );
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(e => e.TripDate)
                .ThenBy(e => e.TripNumber)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Trip>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<Trip> AddAsync(Trip entity)
        {
            _context.Trips.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(Trip entity)
        {
            _context.Trips.Update(entity);
            await _context.SaveChangesAsync();
        }

        public Task<bool> ExistsByTripNumberAsync(string tripNumber, int? excludeId = null)
        {
            return _context.Trips.AnyAsync(e =>
                !e.IsDeleted &&
                e.TripNumber.ToLower() == tripNumber.ToLower() &&
                (!excludeId.HasValue || e.Id != excludeId.Value)
            );
        }

        public Task<bool> CompanyExistsAsync(int id)
        {
            return _context.Companies.AnyAsync(e => e.Id == id && !e.IsDeleted);
        }

        public Task<bool> PartyExistsAsync(int id)
        {
            return _context.Parties.AnyAsync(e => e.Id == id && !e.IsDeleted);
        }

        public Task<bool> VehicleExistsAsync(int id)
        {
            return _context.Vehicles.AnyAsync(e => e.Id == id && !e.IsDeleted);
        }

        public Task<bool> DriverExistsAsync(int id)
        {
            return _context.Drivers.AnyAsync(e => e.Id == id && !e.IsDeleted);
        }
    }
}