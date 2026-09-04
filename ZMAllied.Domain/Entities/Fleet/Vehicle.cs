using ZMAllied.Domain.Common;
using ZMAllied.Domain.Entities.Bilty;
using ZMAllied.Domain.Entities.Dispatch;
using ZMAllied.Domain.Entities.Organization;
using ZMAllied.Domain.Entities.Trips;
using ZMAllied.Domain.Enums;

namespace ZMAllied.Domain.Entities.Fleet
{
    public class Vehicle : BaseEntity
    {
        public int VehicleTypeId { get; set; }
        public VehicleType VehicleType { get; set; } = null!;

        public string RegistrationNumber { get; set; } = string.Empty;
        public string? Make { get; set; }
        public string? Model { get; set; }
        public string? ChassisNumber { get; set; }
        public string? EngineNumber { get; set; }
        public int? Year { get; set; }
        public decimal Capacity { get; set; }
        public decimal CurrentMileage { get; set; }
        public VehicleStatus Status { get; set; } = VehicleStatus.Active;

        public int OfficeId { get; set; }
        public Office Office { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
        public ICollection<FuelExpense> FuelExpenses { get; set; } = new List<FuelExpense>();
        public ICollection<VehicleMaintenance> VehicleMaintenances { get; set; } = new List<VehicleMaintenance>();
        public ICollection<Bilty.Bilty> Bilties { get; set; } = new List<Bilty.Bilty>();
        public ICollection<DDR> DDRs { get; set; } = new List<DDR>();
    }
}
