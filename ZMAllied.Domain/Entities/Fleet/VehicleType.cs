using ZMAllied.Domain.Common;

namespace ZMAllied.Domain.Entities.Fleet
{
    public class VehicleType : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }
}
