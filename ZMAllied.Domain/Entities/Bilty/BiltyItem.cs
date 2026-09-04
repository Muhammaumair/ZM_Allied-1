using ZMAllied.Domain.Common;

namespace ZMAllied.Domain.Entities.Bilty
{
    public class BiltyItem : BaseEntity
    {
        public int BiltyId { get; set; }
        public Bilty Bilty { get; set; } = null!;

        public string? Description { get; set; }
        public decimal Quantity { get; set; }
        public decimal Weight { get; set; }
        public string? Unit { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
    }
}
