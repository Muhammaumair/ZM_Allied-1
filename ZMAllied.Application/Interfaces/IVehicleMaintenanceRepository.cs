using ZMAllied.Application.Common;
using ZMAllied.Domain.Entities.Fleet;

namespace ZMAllied.Application.Interfaces
{
    public interface IVehicleMaintenanceRepository
    {
        Task<VehicleMaintenance?> GetByIdAsync(int id);

        Task<PagedResult<VehicleMaintenance>> GetPagedAsync( int pageNumber, int pageSize, string? search);

        Task<VehicleMaintenance> AddAsync(VehicleMaintenance entity);

        Task UpdateAsync(VehicleMaintenance entity);

        Task<bool> VehicleExistsAsync(int id);

        Task<bool> SupplierExistsAsync(int id);
    }
}