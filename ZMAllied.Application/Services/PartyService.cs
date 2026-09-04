using System;
using System.Threading.Tasks;
using ZMAllied.Application.Common;
using ZMAllied.Application.DTOs.Party;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Entities.Parties;

namespace ZMAllied.Application.Services
{
    public class PartyService : IPartyService
    {
        private readonly IPartyRepository _partyRepository;

        public PartyService(IPartyRepository partyRepository)
        {
            _partyRepository = partyRepository;
        }

        public async Task<PartyResponseDto?> GetByIdAsync(int id)
        {
            var party = await _partyRepository.GetByIdAsync(id);
            if (party == null || party.IsDeleted)
                return null;

            return MapToResponse(party);
        }

        public async Task<PagedResult<PartyResponseDto>> GetPagedAsync(int pageNumber, int pageSize, string? search)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var result = await _partyRepository.GetPagedAsync(pageNumber, pageSize, search);

            return new PagedResult<PartyResponseDto>
            {
                Items = result.Items.ConvertAll(MapToResponse),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task<PartyResponseDto> CreateAsync(PartyCreateDto dto, int? userId)
        {
            await ValidateDuplicates(dto.Name, dto.Phone, dto.CNIC, dto.NTN, null);

            var party = new Party
            {
                Name = dto.Name,
                PartyType = dto.PartyType,
                ContactPerson = dto.ContactPerson,
                Phone = dto.Phone,
                Email = dto.Email,
                Address = dto.Address,
                NTN = dto.NTN,
                CNIC = dto.CNIC,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            var created = await _partyRepository.AddAsync(party);
            return MapToResponse(created);
        }

        public async Task<bool> UpdateAsync(int id, PartyUpdateDto dto, int? userId)
        {
            var party = await _partyRepository.GetByIdAsync(id);
            if (party == null || party.IsDeleted)
                return false;

            await ValidateDuplicates(dto.Name, dto.Phone, dto.CNIC, dto.NTN, id);

            party.Name = dto.Name;
            party.PartyType = dto.PartyType;
            party.ContactPerson = dto.ContactPerson;
            party.Phone = dto.Phone;
            party.Email = dto.Email;
            party.Address = dto.Address;
            party.NTN = dto.NTN;
            party.CNIC = dto.CNIC;
            party.IsActive = dto.IsActive;
            party.UpdatedAt = DateTime.UtcNow;
            party.UpdatedBy = userId;

            await _partyRepository.UpdateAsync(party);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, int? userId)
        {
            var party = await _partyRepository.GetByIdAsync(id);
            if (party == null || party.IsDeleted)
                return false;

            party.IsDeleted = true;
            party.UpdatedAt = DateTime.UtcNow;
            party.UpdatedBy = userId;

            await _partyRepository.UpdateAsync(party);
            return true;
        }

        private async Task ValidateDuplicates(string name, string? phone, string? cnic, string? ntn, int? excludeId)
        {
            if (await _partyRepository.ExistsByNameAsync(name, excludeId))
                throw new DuplicateException($"A party with name '{name}' already exists.");

            if (!string.IsNullOrWhiteSpace(phone) && await _partyRepository.ExistsByPhoneAsync(phone, excludeId))
                throw new DuplicateException($"A party with phone '{phone}' already exists.");

            if (!string.IsNullOrWhiteSpace(cnic) && await _partyRepository.ExistsByCnicAsync(cnic, excludeId))
                throw new DuplicateException($"A party with CNIC '{cnic}' already exists.");

            if (!string.IsNullOrWhiteSpace(ntn) && await _partyRepository.ExistsByNtnAsync(ntn, excludeId))
                throw new DuplicateException($"A party with NTN '{ntn}' already exists.");
        }

        private static PartyResponseDto MapToResponse(Party party)
        {
            return new PartyResponseDto
            {
                Id = party.Id,
                Name = party.Name,
                PartyType = party.PartyType,
                ContactPerson = party.ContactPerson,
                Phone = party.Phone,
                Email = party.Email,
                Address = party.Address,
                NTN = party.NTN,
                CNIC = party.CNIC,
                IsActive = party.IsActive,
                CreatedAt = party.CreatedAt,
                CreatedBy = party.CreatedBy,
                UpdatedAt = party.UpdatedAt,
                UpdatedBy = party.UpdatedBy
            };
        }
    }

    public class DuplicateException : Exception
    {
        public DuplicateException(string message) : base(message) { }
    }
}
