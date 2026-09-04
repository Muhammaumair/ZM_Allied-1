using System;
using System.Threading.Tasks;
using ZMAllied.Application.Common;
using ZMAllied.Application.DTOs.Receipt;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Entities.Accounting;
using ZMAllied.Domain.Enums;

namespace ZMAllied.Application.Services
{
    public class ReceiptService : IReceiptService
    {
        private readonly IReceiptRepository _receiptRepository;
        private readonly ICashBookService _cashBookService;

        public ReceiptService(IReceiptRepository receiptRepository, ICashBookService cashBookService)
        {
            _receiptRepository = receiptRepository;
            _cashBookService = cashBookService;
        }

        public async Task<ReceiptResponseDto?> GetByIdAsync(int id)
        {
            var receipt = await _receiptRepository.GetByIdAsync(id);
            if (receipt == null || receipt.IsDeleted)
                return null;

            return MapToResponse(receipt);
        }

        public async Task<PagedResult<ReceiptResponseDto>> GetPagedAsync(int pageNumber, int pageSize, string? search, Domain.Enums.PaymentStatus? status = null)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var result = await _receiptRepository.GetPagedAsync(pageNumber, pageSize, search, status);

            return new PagedResult<ReceiptResponseDto>
            {
                Items = result.Items.ConvertAll(MapToResponse),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task<ReceiptResponseDto> CreateAsync(ReceiptCreateDto dto, int? userId)
        {
            if (!await _receiptRepository.PartyExistsAsync(dto.PartyId))
                throw new InvalidOperationException($"Party with ID {dto.PartyId} does not exist.");

            if (await _receiptRepository.ExistsByReceiptNumberAsync(dto.ReceiptNumber))
                throw new DuplicateException($"Receipt with number '{dto.ReceiptNumber}' already exists.");

            var receipt = new Receipt
            {
                ReceiptNumber = dto.ReceiptNumber,
                Date = dto.Date == default ? DateTime.UtcNow : dto.Date,
                PartyId = dto.PartyId,
                Amount = dto.Amount,
                PaymentMethod = dto.PaymentMethod,
                ReferenceNumber = dto.ReferenceNumber,
                Description = dto.Description,
                Status = dto.Status,
                Remarks = dto.Remarks,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            var created = await _receiptRepository.AddAsync(receipt);
            await SynchronizeCashBookAsync(created, userId);
            return MapToResponse(created);
        }

        public async Task<bool> UpdateAsync(int id, ReceiptUpdateDto dto, int? userId)
        {
            var receipt = await _receiptRepository.GetByIdAsync(id);
            if (receipt == null || receipt.IsDeleted)
                return false;

            if (!await _receiptRepository.PartyExistsAsync(dto.PartyId))
                throw new InvalidOperationException($"Party with ID {dto.PartyId} does not exist.");

            if (await _receiptRepository.ExistsByReceiptNumberAsync(dto.ReceiptNumber, id))
                throw new DuplicateException($"Receipt with number '{dto.ReceiptNumber}' already exists.");

            receipt.ReceiptNumber = dto.ReceiptNumber;
            receipt.Date = dto.Date;
            receipt.PartyId = dto.PartyId;
            receipt.Amount = dto.Amount;
            receipt.PaymentMethod = dto.PaymentMethod;
            receipt.ReferenceNumber = dto.ReferenceNumber;
            receipt.Description = dto.Description;
            receipt.Status = dto.Status;
            receipt.Remarks = dto.Remarks;
            receipt.UpdatedAt = DateTime.UtcNow;
            receipt.UpdatedBy = userId;

            await _receiptRepository.UpdateAsync(receipt);
            await SynchronizeCashBookAsync(receipt, userId);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, int? userId)
        {
            var receipt = await _receiptRepository.GetByIdAsync(id);
            if (receipt == null || receipt.IsDeleted)
                return false;

            receipt.IsDeleted = true;
            receipt.UpdatedAt = DateTime.UtcNow;
            receipt.UpdatedBy = userId;

            await _receiptRepository.UpdateAsync(receipt);
            await _cashBookService.DeleteReceiptEntryAsync(receipt.Id, userId);
            return true;
        }

        private static ReceiptResponseDto MapToResponse(Receipt receipt)
        {
            return new ReceiptResponseDto
            {
                Id = receipt.Id,
                ReceiptNumber = receipt.ReceiptNumber,
                Date = receipt.Date,
                PartyId = receipt.PartyId,
                Amount = receipt.Amount,
                PaymentMethod = receipt.PaymentMethod,
                ReferenceNumber = receipt.ReferenceNumber,
                Description = receipt.Description,
                Status = receipt.Status,
                Remarks = receipt.Remarks
            };
        }

        private async Task SynchronizeCashBookAsync(Receipt receipt, int? userId)
        {
            if (!RequiresCashBookEntry(receipt.Status, receipt.PaymentMethod))
            {
                await _cashBookService.DeleteReceiptEntryAsync(receipt.Id, userId);
                return;
            }

            await _cashBookService.SynchronizeReceiptAsync(
                receipt.Id,
                receipt.Date,
                receipt.Amount,
                receipt.ReferenceNumber ?? receipt.ReceiptNumber,
                receipt.Description ?? $"Receipt {receipt.ReceiptNumber}",
                userId);
        }

        private static bool RequiresCashBookEntry(PaymentStatus status, string? paymentMethod) =>
            status == PaymentStatus.Completed &&
            (string.Equals(paymentMethod, "Cash", StringComparison.OrdinalIgnoreCase) ||
             string.Equals(paymentMethod, "Bank", StringComparison.OrdinalIgnoreCase));
    }
}
