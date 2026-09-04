using ZMAllied.Domain.Common;
using ZMAllied.Domain.Entities.Bilty;
using ZMAllied.Domain.Entities.Dispatch;
using ZMAllied.Domain.Entities.Trips;

namespace ZMAllied.Domain.Entities.Organization
{
    public class Company : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? ContactPerson { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public decimal Rate { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<Office> Offices { get; set; } = new List<Office>();
        public CompanyRate? CompanyRate { get; set; }
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
        public ICollection<DDR> DDRs { get; set; } = new List<DDR>();
    }
}
