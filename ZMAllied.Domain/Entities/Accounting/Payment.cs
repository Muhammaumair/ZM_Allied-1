using ZMAllied.Domain.Common;
using ZMAllied.Domain.Entities.Parties;
using ZMAllied.Domain.Enums;

namespace ZMAllied.Domain.Entities.Accounting
{
    public class Payment : BaseEntity
    {
        public string PaymentNumber { get; set; } = string.Empty;
        public DateTime Date { get; set; }

        public int? PartyId { get; set; }
        public Party? Party { get; set; }

        public int? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        public decimal Amount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Description { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public string? Remarks { get; set; }

        public ICollection<CashBookEntry> CashBookEntries { get; set; } = new List<CashBookEntry>();
    }
}
