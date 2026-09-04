using ZMAllied.Application.Common;
using ZMAllied.Application.DTOs.Company;

namespace ZMAllied.Application.Interfaces
{
    public interface ICompanyService
    {
        Task<CompanyResponseDto?> GetByIdAsync(int id);

        Task<PagedResult<CompanyResponseDto>> GetPagedAsync(int pageNumber, int pageSize, string? search);

        Task<CompanyResponseDto> CreateAsync(CompanyCreateDto dto,int? userId);

        Task<bool> UpdateAsync( int id, CompanyUpdateDto dto, int? userId);

        Task<bool> DeleteAsync(int id, int? userId);
    }
}