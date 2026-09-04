using ZMAllied.Domain.Common;
using ZMAllied.Domain.Entities.Parties;

namespace ZMAllied.Domain.Entities.Trips
{
    public class TripExpense : BaseEntity
    {
        public int TripId { get; set; }
        public Trip Trip { get; set; } = null!;

        public string? ExpenseType { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }

        public int? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        public string? Remarks { get; set; }
    }
}
