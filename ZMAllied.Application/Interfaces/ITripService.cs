using ZMAllied.Application.Common;
using ZMAllied.Application.DTOs.Trip;
using ZMAllied.Domain.Enums;

namespace ZMAllied.Application.Interfaces
{
    public interface ITripService
    {
        Task<TripResponseDto?> GetByIdAsync(int id);

        Task<PagedResult<TripResponseDto>> GetPagedAsync(int pageNumber,int pageSize,string? search,TripStatus? status = null,DateTime? dateFrom = null, DateTime? dateTo = null);

        Task<TripResponseDto> CreateAsync( TripCreateDto dto, int? userId);

        Task<bool> UpdateAsync( int id, TripUpdateDto dto, int? userId);

        Task<bool> DeleteAsync(int id, int? userId);
    }
}