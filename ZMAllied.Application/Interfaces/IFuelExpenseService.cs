using ZMAllied.Application.Common;
using ZMAllied.Application.DTOs.FuelExpense;

namespace ZMAllied.Application.Interfaces
{
    public interface IFuelExpenseService
    {
        Task<FuelExpenseResponseDto?> GetByIdAsync(int id);

        Task<PagedResult<FuelExpenseResponseDto>> GetPagedAsync( int pageNumber, int pageSize, string? search);

        Task<FuelExpenseResponseDto> CreateAsync( FuelExpenseCreateDto dto, int? userId);

        Task<bool> UpdateAsync( int id, FuelExpenseUpdateDto dto, int? userId);

        Task<bool> DeleteAsync(int id, int? userId);
    }
}