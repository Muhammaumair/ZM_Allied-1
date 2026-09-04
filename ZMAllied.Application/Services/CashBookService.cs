using ZMAllied.Application.Common;
using ZMAllied.Application.DTOs.CashBook;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Entities.Accounting;

namespace ZMAllied.Application.Services
{
    public class CashBookService : ICashBookService
    {
        private const string DebitEntryType = "debit";
        private const string CreditEntryType = "credit";
        private readonly ICashBookRepository _repository;

        public CashBookService(ICashBookRepository repository)
        {
            _repository = repository;
        }

        public async Task<CashBookEntryResponseDto?> GetByIdAsync(int id)
        {
            var entry = await _repository.GetByIdAsync(id);
            return entry is null || entry.IsDeleted ? null : Map(entry);
        }

        public async Task<PagedResult<CashBookEntryResponseDto>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? search,
            DateTime? dateFrom = null,
            DateTime? dateTo = null,
            string? entryType = null,
            string? referenceNumber = null)
        {
            if (dateFrom > dateTo)
            {
                throw new ArgumentException("dateFrom cannot be later than dateTo.");
            }

            ValidateEntryType(entryType);
            (pageNumber, pageSize) = NormalizePagination(pageNumber, pageSize);

            var result = await _repository.GetPagedAsync(
                pageNumber,
                pageSize,
                search,
                dateFrom,
                dateTo,
                entryType,
                referenceNumber);

            return new PagedResult<CashBookEntryResponseDto>
            {
                Items = result.Items.ConvertAll(Map),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount
            };
        }

        public async Task<CashBookEntryResponseDto> CreateAsync(CashBookEntryCreateDto dto, int? userId)
        {
            await ValidateAsync(dto);

            var entry = CreateNew(dto, userId);
            var created = await _repository.AddAsync(entry);
            return Map(created);
        }

        public async Task<bool> UpdateAsync(int id, CashBookEntryUpdateDto dto, int? userId)
        {
            var entry = await _repository.GetByIdAsync(id);

            if (entry is null || entry.IsDeleted)
            {
                return false;
            }

            await ValidateAsync(dto, id);
            ApplyUpdate(entry, dto);
            entry.UpdatedAt = DateTime.UtcNow;
            entry.UpdatedBy = userId;

            await _repository.UpdateAsync(entry);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, int? userId)
        {
            var entry = await _repository.GetByIdAsync(id);

            if (entry is null || entry.IsDeleted)
            {
                return false;
            }

            entry.IsDeleted = true;
            entry.UpdatedAt = DateTime.UtcNow;
            entry.UpdatedBy = userId;

            await _repository.UpdateAsync(entry);
            return true;
        }

        public async Task SynchronizePaymentAsync(
            int paymentId,
            DateTime date,
            decimal amount,
            string referenceNumber,
            string description,
            int? userId)
        {
            var entry = await _repository.GetByPaymentIdAsync(paymentId);
            if (entry is null)
            {
                entry = new CashBookEntry
                {
                    PaymentId = paymentId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId
                };
                ApplyAutomaticEntry(entry, date, amount, 0m, "Payment", paymentId, referenceNumber, description);
                await _repository.AddAsync(entry);
                return;
            }

            ApplyAutomaticEntry(entry, date, amount, 0m, "Payment", paymentId, referenceNumber, description);
            entry.IsDeleted = false;
            entry.UpdatedAt = DateTime.UtcNow;
            entry.UpdatedBy = userId;
            await _repository.UpdateAsync(entry);
        }

        public async Task SynchronizeReceiptAsync(
            int receiptId,
            DateTime date,
            decimal amount,
            string referenceNumber,
            string description,
            int? userId)
        {
            var entry = await _repository.GetByReceiptIdAsync(receiptId);
            if (entry is null)
            {
                entry = new CashBookEntry
                {
                    ReceiptId = receiptId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId
                };
                ApplyAutomaticEntry(entry, date, 0m, amount, "Receipt", receiptId, referenceNumber, description);
                await _repository.AddAsync(entry);
                return;
            }

            ApplyAutomaticEntry(entry, date, 0m, amount, "Receipt", receiptId, referenceNumber, description);
            entry.IsDeleted = false;
            entry.UpdatedAt = DateTime.UtcNow;
            entry.UpdatedBy = userId;
            await _repository.UpdateAsync(entry);
        }

        public async Task DeletePaymentEntryAsync(int paymentId, int? userId)
        {
            var entry = await _repository.GetByPaymentIdAsync(paymentId);
            await SoftDeleteLinkedEntryAsync(entry, userId);
        }

        public async Task DeleteReceiptEntryAsync(int receiptId, int? userId)
        {
            var entry = await _repository.GetByReceiptIdAsync(receiptId);
            await SoftDeleteLinkedEntryAsync(entry, userId);
        }

        private async Task ValidateAsync(CashBookEntryCreateDto dto, int? entryId = null)
        {
            if (dto.Date == default)
            {
                throw new ArgumentException("Date is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Description))
            {
                throw new ArgumentException("Description is required.");
            }

            if ((dto.Debit <= 0m && dto.Credit <= 0m) || (dto.Debit > 0m && dto.Credit > 0m))
            {
                throw new ArgumentException("Exactly one of Debit or Credit must be greater than zero.");
            }

            if (dto.PaymentId.HasValue && dto.ReceiptId.HasValue)
            {
                throw new ArgumentException("A cash book entry cannot be linked to both a payment and a receipt.");
            }

            if (dto.PaymentId.HasValue)
            {
                if (!await _repository.PaymentExistsAsync(dto.PaymentId.Value))
                {
                    throw new ResourceNotFoundException("The specified payment does not exist.");
                }

                var linkedEntry = await _repository.GetByPaymentIdAsync(dto.PaymentId.Value);
                if (linkedEntry is not null && linkedEntry.Id != entryId)
                {
                    throw new DuplicateException("This payment is already linked to a cash book entry.");
                }
            }

            if (dto.ReceiptId.HasValue)
            {
                if (!await _repository.ReceiptExistsAsync(dto.ReceiptId.Value))
                {
                    throw new ResourceNotFoundException("The specified receipt does not exist.");
                }

                var linkedEntry = await _repository.GetByReceiptIdAsync(dto.ReceiptId.Value);
                if (linkedEntry is not null && linkedEntry.Id != entryId)
                {
                    throw new DuplicateException("This receipt is already linked to a cash book entry.");
                }
            }
        }

        private static CashBookEntry CreateNew(CashBookEntryCreateDto dto, int? userId)
        {
            var entry = new CashBookEntry
            {
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            ApplyUpdate(entry, dto);
            return entry;
        }

        private static void ApplyUpdate(CashBookEntry entry, CashBookEntryCreateDto dto)
        {
            entry.Date = dto.Date;
            entry.Description = dto.Description.Trim();
            entry.Debit = dto.Debit;
            entry.Credit = dto.Credit;
            entry.ReferenceType = dto.ReferenceType;
            entry.ReferenceId = dto.ReferenceId;
            entry.ReferenceNumber = dto.ReferenceNumber;
            entry.PaymentId = dto.PaymentId;
            entry.ReceiptId = dto.ReceiptId;
            entry.Remarks = dto.Remarks;
        }

        private static void ApplyAutomaticEntry(
            CashBookEntry entry,
            DateTime date,
            decimal debit,
            decimal credit,
            string referenceType,
            int referenceId,
            string referenceNumber,
            string description)
        {
            entry.Date = date;
            entry.Debit = debit;
            entry.Credit = credit;
            entry.ReferenceType = referenceType;
            entry.ReferenceId = referenceId;
            entry.ReferenceNumber = referenceNumber;
            entry.Description = description;
        }

        private static CashBookEntryResponseDto Map(CashBookEntry entry)
        {
            return new CashBookEntryResponseDto
            {
                Id = entry.Id,
                Date = entry.Date,
                Description = entry.Description,
                Debit = entry.Debit,
                Credit = entry.Credit,
                Balance = entry.Balance,
                ReferenceType = entry.ReferenceType,
                ReferenceId = entry.ReferenceId,
                ReferenceNumber = entry.ReferenceNumber,
                PaymentId = entry.PaymentId,
                ReceiptId = entry.ReceiptId,
                Remarks = entry.Remarks,
                CreatedAt = entry.CreatedAt
            };
        }

        private async Task SoftDeleteLinkedEntryAsync(CashBookEntry? entry, int? userId)
        {
            if (entry is null || entry.IsDeleted)
            {
                return;
            }

            entry.IsDeleted = true;
            entry.UpdatedAt = DateTime.UtcNow;
            entry.UpdatedBy = userId;
            await _repository.UpdateAsync(entry);
        }

        private static void ValidateEntryType(string? entryType)
        {
            if (string.IsNullOrWhiteSpace(entryType))
            {
                return;
            }

            if (!entryType.Equals(DebitEntryType, StringComparison.OrdinalIgnoreCase) &&
                !entryType.Equals(CreditEntryType, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("entryType must be either 'debit' or 'credit'.");
            }
        }

        private static (int PageNumber, int PageSize) NormalizePagination(int pageNumber, int pageSize)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize > 100 ? 100 : pageSize;
            return (pageNumber, pageSize);
        }
    }
}
