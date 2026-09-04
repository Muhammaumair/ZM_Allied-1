using ZMAllied.Application.Common;
using ZMAllied.Domain.Entities.Organization;

namespace ZMAllied.Application.Interfaces
{
    public interface ICompanyRepository
    {
        Task<Company?> GetByIdAsync(int id);

        Task<PagedResult<Company>> GetPagedAsync( int pageNumber, int pageSize,string? search);

        Task<Company> AddAsync(Company company);

        Task UpdateAsync(Company company);

        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);

        Task<bool> ExistsByCodeAsync(string code, int? excludeId = null);
    }
}