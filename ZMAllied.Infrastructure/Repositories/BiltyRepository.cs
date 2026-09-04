using Microsoft.EntityFrameworkCore;
using ZMAllied.Application.Common;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Entities.Bilty;
using ZMAllied.Domain.Enums;
using ZMAllied.Infrastructure.Persistence;

namespace ZMAllied.Infrastructure.Repositories
{
    public class BiltyRepository : IBiltyRepository
    {
        private readonly ZMAlliedDbContext _context;

        public BiltyRepository(ZMAlliedDbContext context)
        {
            _context = context;
        }

        public Task<Bilty?> GetByIdAsync(int id)
        {
            return _context.Bilties
                .Include(x => x.BiltyItems)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<Bilty?> GetForPrintAsync(int id)
        {
            return _context.Bilties
                .Include(x => x.BiltyItems)
                .Include(x => x.Office)
                .Include(x => x.Consignor)
                .Include(x => x.Consignee)
                .Include(x => x.Vehicle)
                .Include(x => x.Driver)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PagedResult<Bilty>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? search,
            BiltyStatus? status = null,
            DateTime? dateFrom = null,
            DateTime? dateTo = null)
        {
            var query = _context.Bilties
                .Include(b => b.BiltyItems)
                .Where(b => !b.IsDeleted);

            if (status.HasValue)
            {
                query = query.Where(b => b.Status == status.Value);
            }

            if (dateFrom.HasValue)
            {
                query = query.Where(b => b.BiltyDate >= dateFrom.Value);
            }

            if (dateTo.HasValue)
            {
                query = query.Where(b => b.BiltyDate <= dateTo.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.ToLower();
                query = query.Where(b =>
                    b.BiltyNumber.ToLower().Contains(searchTerm) ||
                    (b.LoadingPoint != null && b.LoadingPoint.ToLower().Contains(searchTerm)) ||
                    (b.OffloadingPoint != null && b.OffloadingPoint.ToLower().Contains(searchTerm)) ||
                    (b.Consignor != null && b.Consignor.Name.ToLower().Contains(searchTerm)) ||
                    (b.Consignee != null && b.Consignee.Name.ToLower().Contains(searchTerm))
                );
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(b => b.BiltyDate)
                .ThenBy(b => b.BiltyNumber)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Bilty>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<Bilty> AddAsync(Bilty entity)
        {
            var items = entity.BiltyItems.ToList();
            await using var transaction = await _context.Database.BeginTransactionAsync();

            // Save the IDENTITY principal first; EF sets entity.Id from SQL Server.
            entity.BiltyItems.Clear();
            _context.Bilties.Add(entity);
            await _context.SaveChangesAsync();

            if (entity.Id <= 0)
            {
                throw new InvalidOperationException("SQL Server did not generate an ID for the Bilty.");
            }

            foreach (var item in items)
            {
                item.BiltyId = entity.Id;
                item.Bilty = entity;
                item.Amount = item.Quantity * item.Rate;
                item.CreatedAt = entity.CreatedAt;
                item.CreatedBy = entity.CreatedBy;
            }

            _context.BiltyItems.AddRange(items);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            entity.BiltyItems = items;
            return entity;
        }

        public async Task UpdateAsync(Bilty entity)
        {
            _context.BiltyItems.RemoveRange(_context.BiltyItems.Where(i => i.BiltyId == entity.Id));
            _context.Bilties.Update(entity);
            await _context.SaveChangesAsync();
        }

        public Task<bool> ExistsByBiltyNumberAsync(string number, int? excludeId = null)
        {
            return _context.Bilties.AnyAsync(x =>
                !x.IsDeleted &&
                x.BiltyNumber.ToLower() == number.ToLower() &&
                (!excludeId.HasValue || x.Id != excludeId)
            );
        }

        public Task<bool> TripExistsAsync(int id)
        {
            return _context.Trips.AnyAsync(x => x.Id == id && !x.IsDeleted);
        }

        public Task<bool> OfficeExistsAsync(int id)
        {
            return _context.Offices.AnyAsync(x => x.Id == id && !x.IsDeleted);
        }

        public Task<bool> PartyExistsAsync(int id)
        {
            return _context.Parties.AnyAsync(x => x.Id == id && !x.IsDeleted);
        }

        public Task<bool> VehicleExistsAsync(int id)
        {
            return _context.Vehicles.AnyAsync(x => x.Id == id && !x.IsDeleted);
        }

        public Task<bool> DriverExistsAsync(int id)
        {
            return _context.Drivers.AnyAsync(x => x.Id == id && !x.IsDeleted);
        }
    }
}
