using ZMAllied.Application.Common;
using ZMAllied.Domain.Entities.Dispatch;

namespace ZMAllied.Application.Interfaces
{
    public interface IDDRRepository
    {
        Task<DDR?> GetByIdAsync(int id);

        Task<PagedResult<DDR>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? search,
            DateTime? dateFrom = null,
            DateTime? dateTo = null,
            int? companyId = null,
            int? vehicleId = null,
            int? biltyId = null,
            int? brokerId = null,
            int? partyId = null);

        Task<DDR> AddAsync(DDR entity);

        Task UpdateAsync(DDR entity);

        Task<bool> CompanyExistsAsync(int id);

        Task<bool> BiltyExistsAsync(int id);

        Task<bool> VehicleExistsAsync(int id);

        Task<bool> PartyExistsAsync(int id);

        Task<bool> BiltyMatchesVehicleAsync(int biltyId, int vehicleId);
    }
}
