using ZMAllied.Application.Common;
using ZMAllied.Application.DTOs.Bilty;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Entities.Bilty;
using ZMAllied.Domain.Enums;

namespace ZMAllied.Application.Services
{
    public class BiltyService : IBiltyService
    {
        private readonly IBiltyRepository _repository;
        private readonly IBiltyPrintService _printService;

        public BiltyService(IBiltyRepository repository, IBiltyPrintService printService)
        {
            _repository = repository;
            _printService = printService;
        }

        public async Task<BiltyResponseDto?> GetByIdAsync(int id)
        {
            var bilty = await _repository.GetByIdAsync(id);
            return bilty is null || bilty.IsDeleted ? null : Map(bilty);
        }

        public async Task<PagedResult<BiltyResponseDto>> GetPagedAsync(
            int page,
            int pageSize,
            string? query,
            BiltyStatus? status = null,
            DateTime? from = null,
            DateTime? to = null)
        {
            (page, pageSize) = NormalizePagination(page, pageSize);

            if (from.HasValue && to.HasValue && from > to)
            {
                throw new ArgumentException("dateFrom cannot be later than dateTo.");
            }

            var result = await _repository.GetPagedAsync(page, pageSize, query, status, from, to);

            return new PagedResult<BiltyResponseDto>
            {
                Items = result.Items.ConvertAll(Map),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task<BiltyResponseDto> CreateAsync(BiltyCreateDto dto, int? userId)
        {
            await Validate(dto, dto.BiltyNumber, null);
            var bilty = Create(dto, userId);
            var created = await _repository.AddAsync(bilty);
            return Map(created);
        }

        public async Task<bool> UpdateAsync(int id, BiltyUpdateDto dto, int? userId)
        {
            var bilty = await _repository.GetByIdAsync(id);

            if (bilty is null || bilty.IsDeleted)
            {
                return false;
            }

            await Validate(dto, dto.BiltyNumber, id);
            Apply(bilty, dto);
            bilty.UpdatedAt = DateTime.Now;
            bilty.UpdatedBy = userId;

            await _repository.UpdateAsync(bilty);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, int? userId)
        {
            var bilty = await _repository.GetByIdAsync(id);

            if (bilty is null || bilty.IsDeleted)
            {
                return false;
            }

            bilty.IsDeleted = true;
            bilty.UpdatedAt = DateTime.UtcNow;
            bilty.UpdatedBy = userId;

            await _repository.UpdateAsync(bilty);
            return true;
        }

        public async Task<byte[]?> GeneratePrintPdfAsync(int id)
        {
            var bilty = await _repository.GetForPrintAsync(id);
            return bilty is null || bilty.IsDeleted ? null : _printService.Generate(bilty);
        }

        #region Private Methods

        private async Task Validate(BiltyCreateDto dto, string number, int? excludeId)
        {
            // Validate Trip
            if (!await _repository.TripExistsAsync(dto.TripId))
            {
                throw new ResourceNotFoundException("The specified trip does not exist.");
            }

            // Validate Office
            if (!await _repository.OfficeExistsAsync(dto.OfficeId))
            {
                throw new ResourceNotFoundException("The specified office does not exist.");
            }

            // Validate Consignor and Consignee
            if (!await _repository.PartyExistsAsync(dto.ConsignorId) ||
                !await _repository.PartyExistsAsync(dto.ConsigneeId))
            {
                throw new ResourceNotFoundException("The specified sender or receiver does not exist.");
            }

            // Validate Broker (if provided)
            if (dto.BrokerId.HasValue && !await _repository.PartyExistsAsync(dto.BrokerId.Value))
            {
                throw new ResourceNotFoundException("The specified broker does not exist.");
            }

            // Validate Vehicle and Driver
            if (!await _repository.VehicleExistsAsync(dto.VehicleId) ||
                !await _repository.DriverExistsAsync(dto.DriverId))
            {
                throw new ResourceNotFoundException("The specified vehicle or driver does not exist.");
            }

            // Check for duplicate Bilty Number
            if (await _repository.ExistsByBiltyNumberAsync(number, excludeId))
            {
                throw new DuplicateException($"A bilty with number '{number}' already exists.");
            }

            // Validate Advance vs Freight
            if (dto.Advance > dto.Freight)
            {
                throw new ArgumentException("Advance cannot exceed Freight.");
            }

            if (dto.Items is null || dto.Items.Count == 0)
            {
                throw new ArgumentException("At least one bilty item is required.");
            }

            if (dto.Items.Any(item => item.Quantity <= 0 || item.Weight <= 0 || item.Rate < 0))
            {
                throw new ArgumentException("Each bilty item must have a positive quantity and weight, and a non-negative rate.");
            }
        }

        private static Bilty Create(BiltyCreateDto dto, int? userId)
        {
            var bilty = new Bilty
            {
                CreatedAt = DateTime.Now,
                CreatedBy = userId
            };

            Apply(bilty, dto);
            return bilty;
        }

        private static void Apply(Bilty bilty, BiltyCreateDto dto)
        {
            bilty.TripId = dto.TripId;
            bilty.OfficeId = dto.OfficeId;
            bilty.BiltyNumber = dto.BiltyNumber;
            bilty.BiltyDate = dto.BiltyDate;
            bilty.ConsignorId = dto.ConsignorId;
            bilty.ConsigneeId = dto.ConsigneeId;
            bilty.BrokerId = dto.BrokerId;
            bilty.VehicleId = dto.VehicleId;
            bilty.DriverId = dto.DriverId;
            bilty.LoadingPoint = dto.LoadingPoint;
            bilty.OffloadingPoint = dto.OffloadingPoint;
            bilty.PO_Number = dto.PO_Number;
            bilty.Freight = dto.Freight;
            bilty.Advance = dto.Advance;
            bilty.Balance = dto.Freight - dto.Advance;
            bilty.PaymentTerms = dto.PaymentTerms;
            bilty.Status = dto.Status;
            bilty.Remarks = dto.Remarks;

            bilty.BiltyItems = dto.Items.Select(item => new BiltyItem
            {
                Description = item.Description,
                Quantity = item.Quantity,
                Weight = item.Weight,
                Unit = item.Unit,
                Rate = item.Rate,
                Amount = item.Quantity * item.Rate
            }).ToList();
        }

        private static (int Page, int PageSize) NormalizePagination(int page, int pageSize)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize > 100 ? 100 : pageSize;
            return (page, pageSize);
        }

        private static BiltyResponseDto Map(Bilty bilty)
        {
            return new BiltyResponseDto
            {
                Id = bilty.Id,
                TripId = bilty.TripId,
                OfficeId = bilty.OfficeId,
                BiltyNumber = bilty.BiltyNumber,
                BiltyDate = bilty.BiltyDate,
                ConsignorId = bilty.ConsignorId,
                ConsigneeId = bilty.ConsigneeId,
                BrokerId = bilty.BrokerId,
                VehicleId = bilty.VehicleId,
                DriverId = bilty.DriverId,
                LoadingPoint = bilty.LoadingPoint,
                OffloadingPoint = bilty.OffloadingPoint,
                PO_Number = bilty.PO_Number,
                Freight = bilty.Freight,
                Advance = bilty.Advance,
                Balance = bilty.Balance,
                PaymentTerms = bilty.PaymentTerms,
                Status = bilty.Status,
                Remarks = bilty.Remarks,
                Items = bilty.BiltyItems.Select(item => new BiltyItemResponseDto
                {
                    Id = item.Id,
                    Description = item.Description,
                    Quantity = item.Quantity,
                    Weight = item.Weight,
                    Unit = item.Unit,
                    Rate = item.Rate,
                    Amount = item.Amount
                }).ToList(),
                CreatedAt = bilty.CreatedAt,
                CreatedBy = bilty.CreatedBy,
                UpdatedAt = bilty.UpdatedAt,
                UpdatedBy = bilty.UpdatedBy
            };
        }

        #endregion
    }
}
