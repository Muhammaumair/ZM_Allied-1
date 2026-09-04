using ZMAllied.Application.Common;
using ZMAllied.Application.DTOs.DDR;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Entities.Dispatch;

namespace ZMAllied.Application.Services
{
    public class DDRService : IDDRService
    {
        private readonly IDDRRepository _repository;

        public DDRService(IDDRRepository repository)
        {
            _repository = repository;
        }

        public async Task<DDRResponseDto?> GetByIdAsync(int id)
        {
            var ddr = await _repository.GetByIdAsync(id);
            return ddr is null || ddr.IsDeleted ? null : Map(ddr);
        }

        public async Task<PagedResult<DDRResponseDto>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? search,
            DateTime? dateFrom = null,
            DateTime? dateTo = null,
            int? companyId = null,
            int? vehicleId = null,
            int? biltyId = null,
            int? brokerId = null,
            int? partyId = null)
        {
            if (dateFrom > dateTo)
            {
                throw new ArgumentException("dateFrom cannot be later than dateTo.");
            }

            (pageNumber, pageSize) = NormalizePagination(pageNumber, pageSize);
            var result = await _repository.GetPagedAsync(
                pageNumber,
                pageSize,
                search,
                dateFrom,
                dateTo,
                companyId,
                vehicleId,
                biltyId,
                brokerId,
                partyId);

            return new PagedResult<DDRResponseDto>
            {
                Items = result.Items.ConvertAll(Map),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task<DDRResponseDto> CreateAsync(DDRCreateDto dto, int? userId)
        {
            await ValidateAsync(dto);
            var created = await _repository.AddAsync(CreateNew(dto, userId));
            var result = await _repository.GetByIdAsync(created.Id) ?? created;
            return Map(result);
        }

        public async Task<bool> UpdateAsync(int id, DDRUpdateDto dto, int? userId)
        {
            var ddr = await _repository.GetByIdAsync(id);
            if (ddr is null || ddr.IsDeleted)
            {
                return false;
            }

            await ValidateAsync(dto);
            ApplyUpdate(ddr, dto);
            ddr.UpdatedAt = DateTime.UtcNow;
            ddr.UpdatedBy = userId;
            await _repository.UpdateAsync(ddr);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, int? userId)
        {
            var ddr = await _repository.GetByIdAsync(id);
            if (ddr is null || ddr.IsDeleted)
            {
                return false;
            }

            ddr.IsDeleted = true;
            ddr.UpdatedAt = DateTime.UtcNow;
            ddr.UpdatedBy = userId;
            await _repository.UpdateAsync(ddr);
            return true;
        }

        private async Task ValidateAsync(DDRCreateDto dto)
        {
            if (dto.Date == default)
            {
                throw new ArgumentException("Date is required.");
            }

            if (!await _repository.CompanyExistsAsync(dto.CompanyId))
            {
                throw new ResourceNotFoundException("The specified company does not exist.");
            }

            if (!await _repository.VehicleExistsAsync(dto.VehicleId))
            {
                throw new ResourceNotFoundException("The specified vehicle does not exist.");
            }

            if (dto.BiltyId.HasValue)
            {
                if (!await _repository.BiltyExistsAsync(dto.BiltyId.Value))
                {
                    throw new ResourceNotFoundException("The specified bilty does not exist.");
                }

                if (!await _repository.BiltyMatchesVehicleAsync(dto.BiltyId.Value, dto.VehicleId))
                {
                    throw new ArgumentException("The selected bilty is not assigned to the selected vehicle.");
                }
            }

            if (dto.BrokerId.HasValue && !await _repository.PartyExistsAsync(dto.BrokerId.Value))
            {
                throw new ResourceNotFoundException("The specified broker does not exist.");
            }

            if (dto.PartyId.HasValue && !await _repository.PartyExistsAsync(dto.PartyId.Value))
            {
                throw new ResourceNotFoundException("The specified party does not exist.");
            }
        }

        private static DDR CreateNew(DDRCreateDto dto, int? userId)
        {
            var ddr = new DDR
            {
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };
            ApplyUpdate(ddr, dto);
            return ddr;
        }

        private static void ApplyUpdate(DDR ddr, DDRCreateDto dto)
        {
            ddr.Date = dto.Date;
            ddr.CompanyId = dto.CompanyId;
            ddr.BiltyId = dto.BiltyId;
            ddr.VehicleId = dto.VehicleId;
            ddr.Weight = dto.Weight;
            ddr.Lot = dto.Lot;
            ddr.Item = dto.Item;
            ddr.Rent = dto.Rent;
            ddr.MobileNumber = dto.MobileNumber;
            ddr.LoadingPoint = dto.LoadingPoint;
            ddr.OffloadingPoint = dto.OffloadingPoint;
            ddr.BrokerId = dto.BrokerId;
            ddr.PartyId = dto.PartyId;
            ddr.Remarks = dto.Remarks;
        }

        private static DDRResponseDto Map(DDR ddr) => new()
        {
            Id = ddr.Id,
            Date = ddr.Date,
            CompanyId = ddr.CompanyId,
            CompanyName = ddr.Company?.Name,
            BiltyId = ddr.BiltyId,
            BiltyNumber = ddr.Bilty?.BiltyNumber,
            VehicleId = ddr.VehicleId,
            VehicleNumber = ddr.Vehicle?.RegistrationNumber,
            Weight = ddr.Weight,
            Lot = ddr.Lot,
            Item = ddr.Item,
            Rent = ddr.Rent,
            MobileNumber = ddr.MobileNumber,
            LoadingPoint = ddr.LoadingPoint,
            OffloadingPoint = ddr.OffloadingPoint,
            BrokerId = ddr.BrokerId,
            BrokerName = ddr.Broker?.Name,
            PartyId = ddr.PartyId,
            PartyName = ddr.Party?.Name,
            Remarks = ddr.Remarks,
            CreatedAt = ddr.CreatedAt
        };

        private static (int PageNumber, int PageSize) NormalizePagination(int pageNumber, int pageSize)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize > 100 ? 100 : pageSize;
            return (pageNumber, pageSize);
        }
    }
}
