using ZMAllied.Application.Common;
using ZMAllied.Domain.Entities.Trips;
using ZMAllied.Domain.Enums;

namespace ZMAllied.Application.Interfaces
{
    public interface ITripRepository
    {
        Task<Trip?> GetByIdAsync(int id);

        Task<PagedResult<Trip>> GetPagedAsync(int pageNumber, int pageSize, string? search, TripStatus? status = null, DateTime? dateFrom = null, DateTime? dateTo = null);

        Task<Trip> AddAsync(Trip entity);

        Task UpdateAsync(Trip entity);

        Task<bool> ExistsByTripNumberAsync(string tripNumber, int? excludeId = null);

        Task<bool> CompanyExistsAsync(int id);

        Task<bool> PartyExistsAsync(int id);

        Task<bool> VehicleExistsAsync(int id);

        Task<bool> DriverExistsAsync(int id);
    }
}