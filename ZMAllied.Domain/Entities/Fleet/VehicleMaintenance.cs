using ZMAllied.Domain.Common;
using ZMAllied.Domain.Entities.Parties;

namespace ZMAllied.Domain.Entities.Fleet
{
    public class VehicleMaintenance : BaseEntity
    {
        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } = null!;

        public DateTime MaintenanceDate { get; set; }
        public string? MaintenanceType { get; set; }
        public string? Description { get; set; }
        public decimal Mileage { get; set; }
        public decimal Cost { get; set; }

        public int? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        public DateTime? NextMaintenanceDate { get; set; }
        public string? Remarks { get; set; }
    }
}
