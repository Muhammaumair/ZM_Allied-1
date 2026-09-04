using ZMAllied.Domain.Common;
using ZMAllied.Domain.Entities.Dispatch;
using ZMAllied.Domain.Entities.Fleet;
using ZMAllied.Domain.Entities.Organization;
using ZMAllied.Domain.Entities.Parties;
using ZMAllied.Domain.Entities.Trips;
using ZMAllied.Domain.Enums;

namespace ZMAllied.Domain.Entities.Bilty
{
    public class Bilty : BaseEntity
    {
        public int? TripId { get; set; }
        public Trip? Trip { get; set; }

        public int OfficeId { get; set; }
        public Office Office { get; set; } = null!;

        public string BiltyNumber { get; set; } = string.Empty;
        public DateTime BiltyDate { get; set; }

        public int? ConsignorId { get; set; }
        public Party? Consignor { get; set; }

        public int? ConsigneeId { get; set; }
        public Party? Consignee { get; set; }

        public int? BrokerId { get; set; }
        public Party? Broker { get; set; }

        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; } = null!;

        public int DriverId { get; set; }
        public Driver Driver { get; set; } = null!;

        public string? LoadingPoint { get; set; }
        public string? OffloadingPoint { get; set; }
        public string? PO_Number { get; set; }

        public decimal Freight { get; set; }
        public decimal Advance { get; set; }
        public decimal Balance { get; set; }
        public string? PaymentTerms { get; set; }
        public BiltyStatus Status { get; set; } = BiltyStatus.Draft;
        public string? Remarks { get; set; }

        public ICollection<BiltyItem> BiltyItems { get; set; } = new List<BiltyItem>();
        public ICollection<DDR> DDRs { get; set; } = new List<DDR>();
    }
}
