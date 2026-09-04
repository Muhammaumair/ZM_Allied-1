using ZMAllied.Domain.Common;

namespace ZMAllied.Domain.Entities.Parties
{
    public class PartyTransaction : BaseEntity
    {
        public int PartyId { get; set; }
        public Party Party { get; set; } = null!;

        public DateTime Date { get; set; }
        public string? ReferenceType { get; set; }
        public int? ReferenceId { get; set; }
        public string? Description { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string? Remarks { get; set; }
    }
}
