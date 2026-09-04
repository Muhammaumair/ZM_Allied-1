using ZMAllied.Domain.Common;
using ZMAllied.Domain.Entities.Bilty;
using ZMAllied.Domain.Entities.Fleet;
using ZMAllied.Domain.Entities.Organization;
using ZMAllied.Domain.Entities.Parties;
using ZMAllied.Domain.Enums;

namespace ZMAllied.Domain.Entities.Trips
{
    public class Trip : BaseEntity
    {
        public string TripNumber { get; set; } = string.Empty;

        public int CompanyId { get; set; }
        public Company Company { get; set; } = null!;

        public int PartyId { get; set; }
        public Party Party { get; set; } = null!;

        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } = null!;

        public int DriverId { get; set; }
        public Driver Driver { get; set; } = null!;

        public DateTime TripDate { get; set; }
        public string? LoadingPoint { get; set; }
        public string? OffloadingPoint { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TripStatus Status { get; set; } = TripStatus.Draft;

        public decimal TotalFreight { get; set; }
        public decimal Advance { get; set; }
        public decimal Balance { get; set; }
        public string? Remarks { get; set; }

        public ICollection<Bilty.Bilty> Bilties { get; set; } = new List<Bilty.Bilty>();
        public ICollection<TripExpense> TripExpenses { get; set; } = new List<TripExpense>();
    }
}
