using ZMAllied.Application.Common;
using ZMAllied.Domain.Entities.Fleet;

namespace ZMAllied.Application.Interfaces
{
    public interface IFuelExpenseRepository
    {
        Task<FuelExpense?> GetByIdAsync(int id);

        Task<PagedResult<FuelExpense>> GetPagedAsync(int pageNumber,int pageSize,string? search);

        Task<FuelExpense> AddAsync(FuelExpense entity);

        Task UpdateAsync(FuelExpense entity);

        Task<bool> VehicleExistsAsync(int id);

        Task<bool> TripExistsAsync(int id);

        Task<bool> SupplierExistsAsync(int id);
    }
}