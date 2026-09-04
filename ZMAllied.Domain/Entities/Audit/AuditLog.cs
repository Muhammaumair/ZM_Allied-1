using ZMAllied.Domain.Common;
using ZMAllied.Domain.Entities.Identity;

namespace ZMAllied.Domain.Entities.Audit
{
    public class AuditLog : BaseEntity
    {
        public int? UserId { get; set; }
        public User? User { get; set; }

        public string Action { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public string? EntityId { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public string? IpAddress { get; set; }
    }
}
