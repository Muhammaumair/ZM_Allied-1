using ZMAllied.Domain.Common;
using ZMAllied.Domain.Entities.Parties;
using ZMAllied.Domain.Enums;

namespace ZMAllied.Domain.Entities.Accounting
{
    public class Receipt : BaseEntity
    {
        public string ReceiptNumber { get; set; } = string.Empty;
        public DateTime Date { get; set; }

        public int PartyId { get; set; }
        public Party Party { get; set; } = null!;

        public decimal Amount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Description { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public string? Remarks { get; set; }

        public ICollection<CashBookEntry> CashBookEntries { get; set; } = new List<CashBookEntry>();
    }
}
