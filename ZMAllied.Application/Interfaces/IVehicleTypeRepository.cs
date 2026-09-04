using ZMAllied.Application.Common;
using ZMAllied.Domain.Entities.Fleet;

namespace ZMAllied.Application.Interfaces
{
    public interface IVehicleTypeRepository
    {
        Task<VehicleType?> GetByIdAsync(int id);

        Task<PagedResult<VehicleType>> GetPagedAsync(int pageNumber,int pageSize,string? search,bool? isActive = null);

        Task<VehicleType> AddAsync(VehicleType entity);

        Task UpdateAsync(VehicleType entity);

        Task<bool> ExistsAsync(int id);
    }
}