using System.Threading.Tasks;
using ZMAllied.Application.Common;
using ZMAllied.Domain.Entities.Parties;

namespace ZMAllied.Application.Interfaces
{
    public interface ISupplierRepository
    {
        Task<Supplier?> GetByIdAsync(int id);
        Task<PagedResult<Supplier>> GetPagedAsync(int pageNumber, int pageSize, string? search, bool? isActive = null);
        Task<Supplier> AddAsync(Supplier supplier);
        Task UpdateAsync(Supplier supplier);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
        Task<bool> ExistsByPhoneAsync(string phone, int? excludeId = null);
        Task<bool> ExistsByNtnAsync(string ntn, int? excludeId = null);
    }
}
