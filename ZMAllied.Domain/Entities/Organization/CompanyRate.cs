using ZMAllied.Domain.Common;

namespace ZMAllied.Domain.Entities.Organization
{
    public class CompanyRate : BaseEntity
    {
        public int CompanyId { get; set; }
        public Company Company { get; set; } = null!;

        public decimal RatePerTon { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
