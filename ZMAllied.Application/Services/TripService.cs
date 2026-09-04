using ZMAllied.Application.Common;
using ZMAllied.Application.DTOs.Trip;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Entities.Trips;
using ZMAllied.Domain.Enums;

namespace ZMAllied.Application.Services
{
    public class TripService : ITripService
    {
        private readonly ITripRepository _repository;

        public TripService(ITripRepository repository)
        {
            _repository = repository;
        }

        public async Task<TripResponseDto?> GetByIdAsync(int id)
        {
            var trip = await _repository.GetByIdAsync(id);
            return trip is null || trip.IsDeleted ? null : Map(trip);
        }

        public async Task<PagedResult<TripResponseDto>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? search,
            TripStatus? status = null,
            DateTime? dateFrom = null,
            DateTime? dateTo = null)
        {
            (pageNumber, pageSize) = NormalizePagination(pageNumber, pageSize);

            if (dateFrom.HasValue && dateTo.HasValue && dateFrom > dateTo)
            {
                throw new ArgumentException("dateFrom cannot be later than dateTo.");
            }

            var result = await _repository.GetPagedAsync(pageNumber, pageSize, search, status, dateFrom, dateTo);

            return new PagedResult<TripResponseDto>
            {
                Items = result.Items.ConvertAll(Map),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task<TripResponseDto> CreateAsync(TripCreateDto dto, int? userId)
        {
            await Validate(
                dto.CompanyId,
                dto.PartyId,
                dto.VehicleId,
                dto.DriverId,
                dto.TripNumber,
                dto.StartDate,
                dto.EndDate,
                dto.TotalFreight,
                dto.Advance,
                null);

            var trip = CreateNew(dto, userId);
            var created = await _repository.AddAsync(trip);
            return Map(created);
        }

        public async Task<bool> UpdateAsync(int id, TripUpdateDto dto, int? userId)
        {
            var trip = await _repository.GetByIdAsync(id);

            if (trip is null || trip.IsDeleted)
            {
                return false;
            }

            await Validate(
                dto.CompanyId,
                dto.PartyId,
                dto.VehicleId,
                dto.DriverId,
                dto.TripNumber,
                dto.StartDate,
                dto.EndDate,
                dto.TotalFreight,
                dto.Advance,
                id);

            ApplyUpdate(trip, dto);
            trip.UpdatedAt = DateTime.UtcNow;
            trip.UpdatedBy = userId;

            await _repository.UpdateAsync(trip);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, int? userId)
        {
            var trip = await _repository.GetByIdAsync(id);

            if (trip is null || trip.IsDeleted)
            {
                return false;
            }

            trip.IsDeleted = true;
            trip.UpdatedAt = DateTime.UtcNow;
            trip.UpdatedBy = userId;

            await _repository.UpdateAsync(trip);
            return true;
        }

        #region Private Methods

        private async Task Validate(
            int companyId,
            int partyId,
            int vehicleId,
            int driverId,
            string tripNumber,
            DateTime? startDate,
            DateTime? endDate,
            decimal totalFreight,
            decimal advance,
            int? excludeId)
        {
            if (!await _repository.CompanyExistsAsync(companyId))
            {
                throw new ResourceNotFoundException("The specified company does not exist.");
            }

            if (!await _repository.PartyExistsAsync(partyId))
            {
                throw new ResourceNotFoundException("The specified party does not exist.");
            }

            if (!await _repository.VehicleExistsAsync(vehicleId))
            {
                throw new ResourceNotFoundException("The specified vehicle does not exist.");
            }

            if (!await _repository.DriverExistsAsync(driverId))
            {
                throw new ResourceNotFoundException("The specified driver does not exist.");
            }

            if (await _repository.ExistsByTripNumberAsync(tripNumber, excludeId))
            {
                throw new DuplicateException($"A trip with number '{tripNumber}' already exists.");
            }

            if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            {
                throw new ArgumentException("StartDate cannot be later than EndDate.");
            }

            if (advance > totalFreight)
            {
                throw new ArgumentException("Advance cannot exceed TotalFreight.");
            }
        }

        private static Trip CreateNew(TripCreateDto dto, int? userId)
        {
            return new Trip
            {
                TripNumber = dto.TripNumber,
                CompanyId = dto.CompanyId,
                PartyId = dto.PartyId,
                VehicleId = dto.VehicleId,
                DriverId = dto.DriverId,
                TripDate = dto.TripDate,
                LoadingPoint = dto.LoadingPoint,
                OffloadingPoint = dto.OffloadingPoint,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = dto.Status,
                TotalFreight = dto.TotalFreight,
                Advance = dto.Advance,
                Balance = dto.TotalFreight - dto.Advance,
                Remarks = dto.Remarks,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };
        }

        private static void ApplyUpdate(Trip trip, TripUpdateDto dto)
        {
            trip.TripNumber = dto.TripNumber;
            trip.CompanyId = dto.CompanyId;
            trip.PartyId = dto.PartyId;
            trip.VehicleId = dto.VehicleId;
            trip.DriverId = dto.DriverId;
            trip.TripDate = dto.TripDate;
            trip.LoadingPoint = dto.LoadingPoint;
            trip.OffloadingPoint = dto.OffloadingPoint;
            trip.StartDate = dto.StartDate;
            trip.EndDate = dto.EndDate;
            trip.Status = dto.Status;
            trip.TotalFreight = dto.TotalFreight;
            trip.Advance = dto.Advance;
            trip.Balance = dto.TotalFreight - dto.Advance;
            trip.Remarks = dto.Remarks;
        }

        private static TripResponseDto Map(Trip trip)
        {
            return new TripResponseDto
            {
                Id = trip.Id,
                TripNumber = trip.TripNumber,
                CompanyId = trip.CompanyId,
                PartyId = trip.PartyId,
                VehicleId = trip.VehicleId,
                DriverId = trip.DriverId,
                TripDate = trip.TripDate,
                LoadingPoint = trip.LoadingPoint,
                OffloadingPoint = trip.OffloadingPoint,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate,
                Status = trip.Status,
                TotalFreight = trip.TotalFreight,
                Advance = trip.Advance,
                Balance = trip.Balance,
                Remarks = trip.Remarks,
                CreatedAt = trip.CreatedAt,
                CreatedBy = trip.CreatedBy,
                UpdatedAt = trip.UpdatedAt,
                UpdatedBy = trip.UpdatedBy
            };
        }

        private static (int PageNumber, int PageSize) NormalizePagination(int pageNumber, int pageSize)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize > 100 ? 100 : pageSize;
            return (pageNumber, pageSize);
        }

        #endregion
    }
}