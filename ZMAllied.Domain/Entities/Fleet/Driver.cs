using ZMAllied.Domain.Common;
using ZMAllied.Domain.Entities.Bilty;
using ZMAllied.Domain.Entities.Trips;
using ZMAllied.Domain.Enums;

namespace ZMAllied.Domain.Entities.Fleet
{
    public class Driver : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? CNIC { get; set; }
        public string? Phone { get; set; }
        public string? LicenseNumber { get; set; }
        public DateTime? LicenseExpiryDate { get; set; }
        public string? Address { get; set; }
        public DateTime? JoiningDate { get; set; }
        public VehicleStatus Status { get; set; } = VehicleStatus.Active;
        public bool IsActive { get; set; } = true;

        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
        public ICollection<Bilty.Bilty> Bilties { get; set; } = new List<Bilty.Bilty>();
    }
}
