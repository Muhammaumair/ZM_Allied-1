using Microsoft.EntityFrameworkCore;
using ZMAllied.Application.Common;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Entities.Dispatch;
using ZMAllied.Infrastructure.Persistence;

namespace ZMAllied.Infrastructure.Repositories
{
    public class DDRRepository : IDDRRepository
    {
        private readonly ZMAlliedDbContext _context;

        public DDRRepository(ZMAlliedDbContext context)
        {
            _context = context;
        }

        public Task<DDR?> GetByIdAsync(int id) =>
            IncludeReferences(_context.DDRs).FirstOrDefaultAsync(ddr => ddr.Id == id);

        public async Task<PagedResult<DDR>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? search,
            DateTime? dateFrom = null,
            DateTime? dateTo = null,
            int? companyId = null,
            int? vehicleId = null,
            int? biltyId = null,
            int? brokerId = null,
            int? partyId = null)
        {
            var query = IncludeReferences(_context.DDRs).Where(ddr => !ddr.IsDeleted);

            if (dateFrom.HasValue) query = query.Where(ddr => ddr.Date >= dateFrom.Value);
            if (dateTo.HasValue) query = query.Where(ddr => ddr.Date <= dateTo.Value);
            if (companyId.HasValue) query = query.Where(ddr => ddr.CompanyId == companyId.Value);
            if (vehicleId.HasValue) query = query.Where(ddr => ddr.VehicleId == vehicleId.Value);
            if (biltyId.HasValue) query = query.Where(ddr => ddr.BiltyId == biltyId.Value);
            if (brokerId.HasValue) query = query.Where(ddr => ddr.BrokerId == brokerId.Value);
            if (partyId.HasValue) query = query.Where(ddr => ddr.PartyId == partyId.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                query = query.Where(ddr =>
                    (ddr.Lot ?? string.Empty).ToLower().Contains(term) ||
                    (ddr.Item ?? string.Empty).ToLower().Contains(term) ||
                    (ddr.MobileNumber ?? string.Empty).ToLower().Contains(term) ||
                    (ddr.LoadingPoint ?? string.Empty).ToLower().Contains(term) ||
                    (ddr.OffloadingPoint ?? string.Empty).ToLower().Contains(term) ||
                    (ddr.Bilty != null && ddr.Bilty.BiltyNumber.ToLower().Contains(term)) ||
                    ddr.Vehicle.RegistrationNumber.ToLower().Contains(term) ||
                    (ddr.Broker != null && ddr.Broker.Name.ToLower().Contains(term)) ||
                    (ddr.Party != null && ddr.Party.Name.ToLower().Contains(term)));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(ddr => ddr.Date)
                .ThenByDescending(ddr => ddr.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<DDR>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<DDR> AddAsync(DDR entity)
        {
            _context.DDRs.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(DDR entity)
        {
            _context.DDRs.Update(entity);
            await _context.SaveChangesAsync();
        }

        public Task<bool> CompanyExistsAsync(int id) =>
            _context.Companies.AnyAsync(company => company.Id == id && !company.IsDeleted);

        public Task<bool> BiltyExistsAsync(int id) =>
            _context.Bilties.AnyAsync(bilty => bilty.Id == id && !bilty.IsDeleted);

        public Task<bool> VehicleExistsAsync(int id) =>
            _context.Vehicles.AnyAsync(vehicle => vehicle.Id == id && !vehicle.IsDeleted);

        public Task<bool> PartyExistsAsync(int id) =>
            _context.Parties.AnyAsync(party => party.Id == id && !party.IsDeleted);

        public Task<bool> BiltyMatchesVehicleAsync(int biltyId, int vehicleId) =>
            _context.Bilties.AnyAsync(bilty =>
                bilty.Id == biltyId && !bilty.IsDeleted && bilty.VehicleId == vehicleId);

        private static IQueryable<DDR> IncludeReferences(IQueryable<DDR> query) => query
            .Include(ddr => ddr.Company)
            .Include(ddr => ddr.Bilty)
            .Include(ddr => ddr.Vehicle)
            .Include(ddr => ddr.Broker)
            .Include(ddr => ddr.Party);
    }
}
