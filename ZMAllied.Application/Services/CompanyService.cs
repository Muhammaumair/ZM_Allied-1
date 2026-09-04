using ZMAllied.Application.Common;
using ZMAllied.Application.DTOs.Company;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Entities.Organization;

namespace ZMAllied.Application.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _repository;
        private readonly ICompanyRateRepository _companyRateRepository;

        public CompanyService(ICompanyRepository repository, ICompanyRateRepository companyRateRepository)
        {
            _repository = repository;
            _companyRateRepository = companyRateRepository;
        }

        public async Task<CompanyResponseDto?> GetByIdAsync(int id)
        {
            var company = await _repository.GetByIdAsync(id);
            if (company is null || company.IsDeleted) return null;
            return Map(company, await _companyRateRepository.GetByCompanyIdAsync(company.Id));
        }

        public async Task<PagedResult<CompanyResponseDto>> GetPagedAsync(int pageNumber,int pageSize,string? search)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize > 100 ? 100 : pageSize;

            var result = await _repository.GetPagedAsync(pageNumber, pageSize, search);

            return new PagedResult<CompanyResponseDto>
            {
                Items = (await Task.WhenAll(result.Items.Select(async company =>
                    Map(company, await _companyRateRepository.GetByCompanyIdAsync(company.Id))))).ToList(),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task<CompanyResponseDto> CreateAsync(CompanyCreateDto dto, int? userId)
        {
            await ValidateUniqueConstraints(dto.Name, dto.Code, null);

            var company = new Company
            {
                Name = dto.Name,
                Code = dto.Code,
                ContactPerson = dto.ContactPerson,
                Phone = dto.Phone,
                Address = dto.Address,
                Rate =dto.Rate,
                IsActive = true,
                CreatedAt = DateTime.Now,
                CreatedBy = userId
            };

            var created = await _repository.AddAsync(company);
            var companyRate = new CompanyRate
            {
                CompanyId = created.Id,
                RatePerTon = dto.Rate,
                EffectiveFrom = DateTime.Now,
                IsActive = true,
                CreatedAt = DateTime.Now,
                CreatedBy = userId
            };
            await _companyRateRepository.AddAsync(companyRate);
            return Map(created, companyRate);
        }

        public async Task<bool> UpdateAsync(int id, CompanyUpdateDto dto, int? userId)
        {
            var company = await _repository.GetByIdAsync(id);

            if (company is null || company.IsDeleted)
            {
                return false;
            }

            await ValidateUniqueConstraints(dto.Name, dto.Code, id);

            company.Name = dto.Name;
            company.Code = dto.Code;
            company.ContactPerson = dto.ContactPerson;
            company.Phone = dto.Phone;
            company.Address = dto.Address;
            company.Rate = dto.Rate;
            company.IsActive = dto.IsActive;
            company.UpdatedAt = DateTime.Now;
            company.UpdatedBy = userId;

            await _repository.UpdateAsync(company);

            var companyRate = await _companyRateRepository.GetByCompanyIdAsync(company.Id);
            if (companyRate is null)
            {
                await _companyRateRepository.AddAsync(new CompanyRate
                {
                    CompanyId = company.Id,
                    RatePerTon = dto.Rate,
                    EffectiveFrom = DateTime.Now,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    CreatedBy = userId
                });
            }
            else
            {
                companyRate.RatePerTon = dto.Rate;
                companyRate.UpdatedAt = DateTime.Now;
                companyRate.UpdatedBy = userId;
                await _companyRateRepository.UpdateAsync(companyRate);
            }
            return true;
        }

        public async Task<bool> DeleteAsync(int id, int? userId)
        {
            var company = await _repository.GetByIdAsync(id);

            if (company is null || company.IsDeleted)
            {
                return false;
            }

            company.IsDeleted = true;
            company.UpdatedAt = DateTime.Now;
            company.UpdatedBy = userId;

            await _repository.UpdateAsync(company);
            return true;
        }

        #region Private Methods

        private async Task ValidateUniqueConstraints(string name, string code, int? excludeId)
        {
            if (await _repository.ExistsByNameAsync(name, excludeId))
            {
                throw new DuplicateException($"A company with name '{name}' already exists.");
            }

            if (await _repository.ExistsByCodeAsync(code, excludeId))
            {
                throw new DuplicateException($"A company with code '{code}' already exists.");
            }
        }

        private static CompanyResponseDto Map(Company company, CompanyRate? companyRate)
        {
            return new CompanyResponseDto
            {
                Id = company.Id,
                Name = company.Name,
                Code = company.Code,
                ContactPerson = company.ContactPerson,
                Phone = company.Phone,
                Address = company.Address,
                Rate = companyRate?.RatePerTon ?? company.Rate,
                IsActive = company.IsActive,
                CreatedAt = company.CreatedAt,
                CreatedBy = company.CreatedBy,
                UpdatedAt = company.UpdatedAt,
                UpdatedBy = company.UpdatedBy
            };
        }

        #endregion
    }
}
