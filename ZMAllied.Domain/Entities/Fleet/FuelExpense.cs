using ZMAllied.Domain.Common;
using ZMAllied.Domain.Entities.Parties;
using ZMAllied.Domain.Entities.Trips;

namespace ZMAllied.Domain.Entities.Fleet
{
    public class FuelExpense : BaseEntity
    {
        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } = null!;

        public int? TripId { get; set; }
        public Trip? Trip { get; set; }

        public DateTime Date { get; set; }
        public string? FuelType { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public decimal Odometer { get; set; }

        public int? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        public string? Remarks { get; set; }
    }
}
