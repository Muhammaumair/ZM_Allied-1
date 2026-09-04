using System.Threading.Tasks;
using ZMAllied.Application.Common;
using ZMAllied.Domain.Entities.Organization;

namespace ZMAllied.Application.Interfaces
{
    public interface IOfficeRepository
    {
        Task<Office?> GetByIdAsync(int id);
        Task<PagedResult<Office>> GetPagedAsync(int pageNumber, int pageSize, string? search, bool? isActive = null);
        Task<Office> AddAsync(Office office);
        Task UpdateAsync(Office office);
        Task<bool> ExistsByCodeAsync(string code, int? excludeId = null);
        Task<bool> CompanyExistsAsync(int companyId);
    }
}
