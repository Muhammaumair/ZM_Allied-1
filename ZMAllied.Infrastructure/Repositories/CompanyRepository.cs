using Microsoft.EntityFrameworkCore;
using ZMAllied.Application.Common;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Entities.Organization;
using ZMAllied.Infrastructure.Persistence;

namespace ZMAllied.Infrastructure.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ZMAlliedDbContext _context;

        public CompanyRepository(ZMAlliedDbContext context)
        {
            _context = context;
        }

        public Task<Company?> GetByIdAsync(int id)
        {
            return _context.Companies.FindAsync(id).AsTask();
        }

        public async Task<PagedResult<Company>> GetPagedAsync(int pageNumber,int pageSize,string? search)
        {
            var query = _context.Companies.Where(c => !c.IsDeleted);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.ToLower();
                query = query.Where(c =>
                    c.Name.ToLower().Contains(searchTerm) ||
                    c.Code.ToLower().Contains(searchTerm) ||
                    (c.ContactPerson != null && c.ContactPerson.ToLower().Contains(searchTerm)) ||
                    (c.Phone != null && c.Phone.ToLower().Contains(searchTerm))
                );
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(c => c.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Company>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<Company> AddAsync(Company company)
        {
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();
            return company;
        }

        public async Task UpdateAsync(Company company)
        {
            _context.Companies.Update(company);
            await _context.SaveChangesAsync();
        }

        public Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            return _context.Companies.AnyAsync(x =>
                !x.IsDeleted &&
                x.Name.ToLower() == name.ToLower() &&
                (!excludeId.HasValue || x.Id != excludeId)
            );
        }

        public Task<bool> ExistsByCodeAsync(string code, int? excludeId = null)
        {
            return _context.Companies.AnyAsync(x =>
                !x.IsDeleted &&
                x.Code.ToLower() == code.ToLower() &&
                (!excludeId.HasValue || x.Id != excludeId)
            );
        }
    }
}