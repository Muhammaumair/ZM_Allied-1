using ZMAllied.Domain.Common;
using ZMAllied.Domain.Enums;

namespace ZMAllied.Domain.Entities.Accounting
{
    public class AccountTransaction : BaseEntity
    {
        public DateTime Date { get; set; }
        public TransactionType TransactionType { get; set; }
        public string? ReferenceType { get; set; }
        public int? ReferenceId { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string? Description { get; set; }
        public string? Remarks { get; set; }
    }
}
