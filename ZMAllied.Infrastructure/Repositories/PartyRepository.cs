using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZMAllied.Application.Common;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Entities.Parties;
using ZMAllied.Infrastructure.Persistence;

namespace ZMAllied.Infrastructure.Repositories
{
    public class PartyRepository : IPartyRepository
    {
        private readonly ZMAlliedDbContext _context;

        public PartyRepository(ZMAlliedDbContext context)
        {
            _context = context;
        }

        public async Task<Party?> GetByIdAsync(int id)
        {
            return await _context.Parties.FindAsync(id);
        }

        public async Task<PagedResult<Party>> GetPagedAsync(int pageNumber, int pageSize, string? search)
        {
            var query = _context.Parties.Where(p => !p.IsDeleted);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchLower = search.ToLower();
                query = query.Where(p =>
                    p.Name.ToLower().Contains(searchLower) ||
                    (p.Phone != null && p.Phone.ToLower().Contains(searchLower)) ||
                    (p.Email != null && p.Email.ToLower().Contains(searchLower)) ||
                    (p.CNIC != null && p.CNIC.ToLower().Contains(searchLower)) ||
                    (p.NTN != null && p.NTN.ToLower().Contains(searchLower)) ||
                    (p.ContactPerson != null && p.ContactPerson.ToLower().Contains(searchLower))
                );
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(p => p.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Party>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<Party> AddAsync(Party party)
        {
            _context.Parties.Add(party);
            await _context.SaveChangesAsync();
            return party;
        }

        public async Task UpdateAsync(Party party)
        {
            _context.Parties.Update(party);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            return await _context.Parties.AnyAsync(p =>
                !p.IsDeleted &&
                p.Name.ToLower() == name.ToLower() &&
                (!excludeId.HasValue || p.Id != excludeId.Value));
        }

        public async Task<bool> ExistsByPhoneAsync(string phone, int? excludeId = null)
        {
            return await _context.Parties.AnyAsync(p =>
                !p.IsDeleted &&
                p.Phone != null &&
                p.Phone.ToLower() == phone.ToLower() &&
                (!excludeId.HasValue || p.Id != excludeId.Value));
        }

        public async Task<bool> ExistsByCnicAsync(string cnic, int? excludeId = null)
        {
            return await _context.Parties.AnyAsync(p =>
                !p.IsDeleted &&
                p.CNIC != null &&
                p.CNIC.ToLower() == cnic.ToLower() &&
                (!excludeId.HasValue || p.Id != excludeId.Value));
        }

        public async Task<bool> ExistsByNtnAsync(string ntn, int? excludeId = null)
        {
            return await _context.Parties.AnyAsync(p =>
                !p.IsDeleted &&
                p.NTN != null &&
                p.NTN.ToLower() == ntn.ToLower() &&
                (!excludeId.HasValue || p.Id != excludeId.Value));
        }
    }
}
