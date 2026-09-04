using ZMAllied.Application.Common;
using ZMAllied.Domain.Entities.Fleet;

namespace ZMAllied.Application.Interfaces
{
    public interface IDriverRepository
    {
        Task<Driver?> GetByIdAsync(int id);

        Task<PagedResult<Driver>> GetPagedAsync(int pageNumber,int pageSize,string? search,bool? isActive = null);

        Task<Driver> AddAsync(Driver entity);

        Task UpdateAsync(Driver entity);
    }
}