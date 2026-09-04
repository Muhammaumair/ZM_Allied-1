using ZMAllied.Application.Common;
using ZMAllied.Application.DTOs.Payment;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Entities.Accounting;
using ZMAllied.Domain.Enums;

namespace ZMAllied.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _repository;
        private readonly ICashBookService _cashBookService;

        public PaymentService(IPaymentRepository repository, ICashBookService cashBookService)
        {
            _repository = repository;
            _cashBookService = cashBookService;
        }

        public async Task<PaymentResponseDto?> GetByIdAsync(int id)
        {
            var payment = await _repository.GetByIdAsync(id);
            return payment is null || payment.IsDeleted ? null : Map(payment);
        }

        public async Task<PagedResult<PaymentResponseDto>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? search,
            PaymentStatus? status = null,
            DateTime? dateFrom = null,
            DateTime? dateTo = null)
        {
            if (dateFrom > dateTo)
            {
                throw new ArgumentException("dateFrom cannot be later than dateTo.");
            }

            (pageNumber, pageSize) = NormalizePagination(pageNumber, pageSize);

            var result = await _repository.GetPagedAsync(pageNumber, pageSize, search, status, dateFrom, dateTo);

            return new PagedResult<PaymentResponseDto>
            {
                Items = result.Items.ConvertAll(Map),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task<PaymentResponseDto> CreateAsync(PaymentCreateDto dto, int? userId)
        {
            await Validate(dto, null);
            var payment = CreateNew(dto, userId);
            var created = await _repository.AddAsync(payment);

            await SynchronizeCashBookAsync(created, userId);
            return Map(created);
        }

        public async Task<bool> UpdateAsync(int id, PaymentUpdateDto dto, int? userId)
        {
            var payment = await _repository.GetByIdAsync(id);

            if (payment is null || payment.IsDeleted)
            {
                return false;
            }

            await Validate(dto, id);
            ApplyUpdate(payment, dto);
            payment.UpdatedAt = DateTime.UtcNow;
            payment.UpdatedBy = userId;

            await _repository.UpdateAsync(payment);
            await SynchronizeCashBookAsync(payment, userId);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, int? userId)
        {
            var payment = await _repository.GetByIdAsync(id);

            if (payment is null || payment.IsDeleted)
            {
                return false;
            }

            payment.IsDeleted = true;
            payment.UpdatedAt = DateTime.UtcNow;
            payment.UpdatedBy = userId;

            await _repository.UpdateAsync(payment);
            await _cashBookService.DeletePaymentEntryAsync(payment.Id, userId);
            return true;
        }

        #region Private Methods

        private async Task Validate(PaymentCreateDto dto, int? excludeId)
        {
            if (await _repository.ExistsByPaymentNumberAsync(dto.PaymentNumber, excludeId))
            {
                throw new DuplicateException($"A payment with number '{dto.PaymentNumber}' already exists.");
            }

            if (dto.PartyId.HasValue && !await _repository.PartyExistsAsync(dto.PartyId.Value))
            {
                throw new ResourceNotFoundException("The specified party does not exist.");
            }

            if (dto.SupplierId.HasValue && !await _repository.SupplierExistsAsync(dto.SupplierId.Value))
            {
                throw new ResourceNotFoundException("The specified supplier does not exist.");
            }
        }

        private static Payment CreateNew(PaymentCreateDto dto, int? userId)
        {
            var payment = new Payment
            {
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            ApplyUpdate(payment, dto);
            return payment;
        }

        private static void ApplyUpdate(Payment payment, PaymentCreateDto dto)
        {
            payment.PaymentNumber = dto.PaymentNumber;
            payment.Date = dto.Date;
            payment.PartyId = dto.PartyId;
            payment.SupplierId = dto.SupplierId;
            payment.Amount = dto.Amount;
            payment.PaymentMethod = dto.PaymentMethod;
            payment.ReferenceNumber = dto.ReferenceNumber;
            payment.Description = dto.Description;
            payment.Status = dto.Status;
            payment.Remarks = dto.Remarks;
        }

        private static PaymentResponseDto Map(Payment payment)
        {
            return new PaymentResponseDto
            {
                Id = payment.Id,
                PaymentNumber = payment.PaymentNumber,
                Date = payment.Date,
                PartyId = payment.PartyId,
                SupplierId = payment.SupplierId,
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod,
                ReferenceNumber = payment.ReferenceNumber,
                Description = payment.Description,
                Status = payment.Status,
                Remarks = payment.Remarks,
                CreatedAt = payment.CreatedAt
            };
        }

        private async Task SynchronizeCashBookAsync(Payment payment, int? userId)
        {
            if (!RequiresCashBookEntry(payment.Status, payment.PaymentMethod))
            {
                await _cashBookService.DeletePaymentEntryAsync(payment.Id, userId);
                return;
            }

            await _cashBookService.SynchronizePaymentAsync(
                payment.Id,
                payment.Date,
                payment.Amount,
                payment.ReferenceNumber ?? payment.PaymentNumber,
                payment.Description ?? $"Payment {payment.PaymentNumber}",
                userId);
        }

        private static bool RequiresCashBookEntry(PaymentStatus status, string? paymentMethod) =>
            status == PaymentStatus.Completed &&
            (string.Equals(paymentMethod, "Cash", StringComparison.OrdinalIgnoreCase) ||
             string.Equals(paymentMethod, "Bank", StringComparison.OrdinalIgnoreCase));

        private static (int PageNumber, int PageSize) NormalizePagination(int pageNumber, int pageSize)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize > 100 ? 100 : pageSize;
            return (pageNumber, pageSize);
        }

        #endregion
    }
}
