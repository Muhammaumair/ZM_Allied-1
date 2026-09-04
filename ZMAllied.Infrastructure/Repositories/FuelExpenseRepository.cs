using Microsoft.EntityFrameworkCore;
using ZMAllied.Application.Common;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Entities.Fleet;
using ZMAllied.Infrastructure.Persistence;

namespace ZMAllied.Infrastructure.Repositories
{
    public class FuelExpenseRepository : IFuelExpenseRepository
    {
        private readonly ZMAlliedDbContext _context;

        public FuelExpenseRepository(ZMAlliedDbContext context)
        {
            _context = context;
        }

        public Task<FuelExpense?> GetByIdAsync(int id)
        {
            return _context.FuelExpenses.FindAsync(id).AsTask();
        }

        public async Task<PagedResult<FuelExpense>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? search)
        {
            var query = _context.FuelExpenses.Where(e => !e.IsDeleted);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.ToLower();
                query = query.Where(e =>
                    (e.FuelType ?? "").ToLower().Contains(searchTerm) ||
                    (e.Remarks ?? "").ToLower().Contains(searchTerm)
                );
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(e => e.Date)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<FuelExpense>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<FuelExpense> AddAsync(FuelExpense entity)
        {
            _context.FuelExpenses.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(FuelExpense entity)
        {
            _context.FuelExpenses.Update(entity);
            await _context.SaveChangesAsync();
        }

        public Task<bool> VehicleExistsAsync(int id)
        {
            return _context.Vehicles.AnyAsync(x => x.Id == id && !x.IsDeleted);
        }

        public Task<bool> TripExistsAsync(int id)
        {
            return _context.Trips.AnyAsync(x => x.Id == id && !x.IsDeleted);
        }

        public Task<bool> SupplierExistsAsync(int id)
        {
            return _context.Suppliers.AnyAsync(x => x.Id == id && !x.IsDeleted);
        }
    }
}