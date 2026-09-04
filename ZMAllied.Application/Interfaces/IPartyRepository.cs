using System.Threading.Tasks;
using ZMAllied.Application.Common;
using ZMAllied.Domain.Entities.Parties;

namespace ZMAllied.Application.Interfaces
{
    public interface IPartyRepository
    {
        Task<Party?> GetByIdAsync(int id);
        Task<PagedResult<Party>> GetPagedAsync(int pageNumber, int pageSize, string? search);
        Task<Party> AddAsync(Party party);
        Task UpdateAsync(Party party);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
        Task<bool> ExistsByPhoneAsync(string phone, int? excludeId = null);
        Task<bool> ExistsByCnicAsync(string cnic, int? excludeId = null);
        Task<bool> ExistsByNtnAsync(string ntn, int? excludeId = null);
    }
}
