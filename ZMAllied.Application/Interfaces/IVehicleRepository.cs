using ZMAllied.Application.Common;
using ZMAllied.Domain.Entities.Fleet;

namespace ZMAllied.Application.Interfaces
{
    public interface IVehicleRepository
    {
        Task<Vehicle?> GetByIdAsync(int id);

        Task<PagedResult<Vehicle>> GetPagedAsync(int pageNumber,int pageSize,string? search,bool? isActive = null);

        Task<Vehicle> AddAsync(Vehicle entity);

        Task UpdateAsync(Vehicle entity);

        Task<bool> ExistsByRegistrationNumberAsync(string registrationNumber, int? excludeId = null);

        Task<bool> OfficeExistsAsync(int officeId);
    }
}