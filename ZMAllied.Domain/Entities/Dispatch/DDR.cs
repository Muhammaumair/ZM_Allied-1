using ZMAllied.Domain.Common;
using ZMAllied.Domain.Entities.Bilty;
using ZMAllied.Domain.Entities.Fleet;
using ZMAllied.Domain.Entities.Organization;
using ZMAllied.Domain.Entities.Parties;

namespace ZMAllied.Domain.Entities.Dispatch
{
    public class DDR : BaseEntity
    {
        public DateTime Date { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; } = null!;

        public int? BiltyId { get; set; }
        public Bilty.Bilty? Bilty { get; set; }

        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } = null!;

        public decimal Weight { get; set; }
        public string? Lot { get; set; }
        public string? Item { get; set; }
        public decimal? Rent { get; set; }
        public string? MobileNumber { get; set; }
        public string? LoadingPoint { get; set; }
        public string? OffloadingPoint { get; set; }

        public int? BrokerId { get; set; }
        public Party? Broker { get; set; }

        public int? PartyId { get; set; }
        public Party? Party { get; set; }

        public string? Remarks { get; set; }
    }
}
