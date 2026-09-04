using System;
using System.Threading.Tasks;
using ZMAllied.Application.Common;
using ZMAllied.Application.DTOs.FuelExpense;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Entities.Fleet;

namespace ZMAllied.Application.Services
{
    public class FuelExpenseService : IFuelExpenseService
    {
        private readonly IFuelExpenseRepository _fuelExpenseRepository;

        public FuelExpenseService(IFuelExpenseRepository fuelExpenseRepository)
        {
            _fuelExpenseRepository = fuelExpenseRepository;
        }

        public async Task<FuelExpenseResponseDto?> GetByIdAsync(int id)
        {
            var expense = await _fuelExpenseRepository.GetByIdAsync(id);
            if (expense == null || expense.IsDeleted)
                return null;

            return MapToResponse(expense);
        }

        public async Task<PagedResult<FuelExpenseResponseDto>> GetPagedAsync(int pageNumber, int pageSize, string? search)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var result = await _fuelExpenseRepository.GetPagedAsync(pageNumber, pageSize, search);

            return new PagedResult<FuelExpenseResponseDto>
            {
                Items = result.Items.ConvertAll(MapToResponse),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task<FuelExpenseResponseDto> CreateAsync(FuelExpenseCreateDto dto, int? userId)
        {
            if (!await _fuelExpenseRepository.VehicleExistsAsync(dto.VehicleId))
                throw new InvalidOperationException($"Vehicle with ID {dto.VehicleId} does not exist.");

            if (dto.TripId.HasValue && !await _fuelExpenseRepository.TripExistsAsync(dto.TripId.Value))
                throw new InvalidOperationException($"Trip with ID {dto.TripId.Value} does not exist.");

            if (dto.SupplierId.HasValue && !await _fuelExpenseRepository.SupplierExistsAsync(dto.SupplierId.Value))
                throw new InvalidOperationException($"Supplier with ID {dto.SupplierId.Value} does not exist.");

            var amount = dto.Quantity * dto.Rate;

            var expense = new FuelExpense
            {
                VehicleId = dto.VehicleId,
                TripId = dto.TripId,
                Date = dto.Date == default ? DateTime.UtcNow : dto.Date,
                FuelType = dto.FuelType,
                Quantity = dto.Quantity,
                Rate = dto.Rate,
                Amount = amount,
                Odometer = dto.Odometer,
                SupplierId = dto.SupplierId,
                Remarks = dto.Remarks,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            var created = await _fuelExpenseRepository.AddAsync(expense);
            return MapToResponse(created);
        }

        public async Task<bool> UpdateAsync(int id, FuelExpenseUpdateDto dto, int? userId)
        {
            var expense = await _fuelExpenseRepository.GetByIdAsync(id);
            if (expense == null || expense.IsDeleted)
                return false;

            if (!await _fuelExpenseRepository.VehicleExistsAsync(dto.VehicleId))
                throw new InvalidOperationException($"Vehicle with ID {dto.VehicleId} does not exist.");

            if (dto.TripId.HasValue && !await _fuelExpenseRepository.TripExistsAsync(dto.TripId.Value))
                throw new InvalidOperationException($"Trip with ID {dto.TripId.Value} does not exist.");

            if (dto.SupplierId.HasValue && !await _fuelExpenseRepository.SupplierExistsAsync(dto.SupplierId.Value))
                throw new InvalidOperationException($"Supplier with ID {dto.SupplierId.Value} does not exist.");

            expense.VehicleId = dto.VehicleId;
            expense.TripId = dto.TripId;
            expense.Date = dto.Date;
            expense.FuelType = dto.FuelType;
            expense.Quantity = dto.Quantity;
            expense.Rate = dto.Rate;
            expense.Amount = dto.Quantity * dto.Rate;
            expense.Odometer = dto.Odometer;
            expense.SupplierId = dto.SupplierId;
            expense.Remarks = dto.Remarks;
            expense.UpdatedAt = DateTime.UtcNow;
            expense.UpdatedBy = userId;

            await _fuelExpenseRepository.UpdateAsync(expense);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, int? userId)
        {
            var expense = await _fuelExpenseRepository.GetByIdAsync(id);
            if (expense == null || expense.IsDeleted)
                return false;

            expense.IsDeleted = true;
            expense.UpdatedAt = DateTime.UtcNow;
            expense.UpdatedBy = userId;

            await _fuelExpenseRepository.UpdateAsync(expense);
            return true;
        }

        private static FuelExpenseResponseDto MapToResponse(FuelExpense expense)
        {
            return new FuelExpenseResponseDto
            {
                Id = expense.Id,
                VehicleId = expense.VehicleId,
                TripId = expense.TripId,
                Date = expense.Date,
                FuelType = expense.FuelType,
                Quantity = expense.Quantity,
                Rate = expense.Rate,
                Amount = expense.Amount,
                Odometer = expense.Odometer,
                SupplierId = expense.SupplierId,
                Remarks = expense.Remarks
            };
        }
    }
}
