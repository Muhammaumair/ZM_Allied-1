using System.Threading.Tasks;
using ZMAllied.Application.Common;
using ZMAllied.Application.DTOs.Party;

namespace ZMAllied.Application.Interfaces
{
    public interface IPartyService
    {
        Task<PartyResponseDto?> GetByIdAsync(int id);
        Task<PagedResult<PartyResponseDto>> GetPagedAsync(int pageNumber, int pageSize, string? search);
        Task<PartyResponseDto> CreateAsync(PartyCreateDto dto, int? userId);
        Task<bool> UpdateAsync(int id, PartyUpdateDto dto, int? userId);
        Task<bool> DeleteAsync(int id, int? userId);
    }
}
