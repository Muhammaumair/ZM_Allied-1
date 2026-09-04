using ZMAllied.Application.Common;
using ZMAllied.Domain.Entities.Bilty;
using ZMAllied.Domain.Enums;

namespace ZMAllied.Application.Interfaces
{
    public interface IBiltyRepository
    {
        Task<Bilty?> GetByIdAsync(int id);

        Task<Bilty?> GetForPrintAsync(int id);

        Task<PagedResult<Bilty>> GetPagedAsync(int pageNumber,int pageSize,string? search,BiltyStatus? status = null, DateTime? dateFrom = null, DateTime? dateTo = null);

        Task<Bilty> AddAsync(Bilty entity);

        Task UpdateAsync(Bilty entity);

        Task<bool> ExistsByBiltyNumberAsync(string number, int? excludeId = null);

        Task<bool> TripExistsAsync(int id);

        Task<bool> OfficeExistsAsync(int id);

        Task<bool> PartyExistsAsync(int id);

        Task<bool> VehicleExistsAsync(int id);

        Task<bool> DriverExistsAsync(int id);
    }
}