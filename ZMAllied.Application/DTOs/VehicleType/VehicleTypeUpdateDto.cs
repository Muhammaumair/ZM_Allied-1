using System.ComponentModel.DataAnnotations;

namespace ZMAllied.Application.DTOs.VehicleType
{
    public class VehicleTypeUpdateDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }
}