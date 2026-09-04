using ZMAllied.Domain.Common;
using ZMAllied.Domain.Entities.Bilty;
using ZMAllied.Domain.Entities.Fleet;

namespace ZMAllied.Domain.Entities.Organization
{
    public class Office : BaseEntity
    {
        public int CompanyId { get; set; }
        public Company Company { get; set; } = null!;

        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public bool IsHeadOffice { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public ICollection<Bilty.Bilty> Bilties { get; set; } = new List<Bilty.Bilty>();
    }
}
