using ZMAllied.Domain.Common;
using ZMAllied.Domain.Entities.Accounting;
using ZMAllied.Domain.Entities.Bilty;
using ZMAllied.Domain.Entities.Dispatch;
using ZMAllied.Domain.Entities.Trips;
using ZMAllied.Domain.Enums;

namespace ZMAllied.Domain.Entities.Parties
{
    public class Party : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public PartyType PartyType { get; set; }
        public string? ContactPerson { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? NTN { get; set; }
        public string? CNIC { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
        public ICollection<Bilty.Bilty> ConsignorBilties { get; set; } = new List<Bilty.Bilty>();
        public ICollection<Bilty.Bilty> ConsigneeBilties { get; set; } = new List<Bilty.Bilty>();
        public ICollection<Bilty.Bilty> BrokerBilties { get; set; } = new List<Bilty.Bilty>();
        public ICollection<DDR> BrokerDDRs { get; set; } = new List<DDR>();
        public ICollection<DDR> DispatchDDRs { get; set; } = new List<DDR>();
        public ICollection<PartyTransaction> PartyTransactions { get; set; } = new List<PartyTransaction>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<Receipt> Receipts { get; set; } = new List<Receipt>();
    }
}
