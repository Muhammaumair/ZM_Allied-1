using ZMAllied.Domain.Common;
using ZMAllied.Domain.Entities.Accounting;
using ZMAllied.Domain.Entities.Fleet;
using ZMAllied.Domain.Entities.Trips;

namespace ZMAllied.Domain.Entities.Parties
{
    public class Supplier : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? ContactPerson { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? NTN { get; set; }
        public string? PaymentTerms { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<FuelExpense> FuelExpenses { get; set; } = new List<FuelExpense>();
        public ICollection<VehicleMaintenance> VehicleMaintenances { get; set; } = new List<VehicleMaintenance>();
        public ICollection<TripExpense> TripExpenses { get; set; } = new List<TripExpense>();
    }
}
