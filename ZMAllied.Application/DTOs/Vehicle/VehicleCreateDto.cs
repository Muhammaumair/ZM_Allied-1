using System.ComponentModel.DataAnnotations;
using ZMAllied.Domain.Enums;

namespace ZMAllied.Application.DTOs.Vehicle
{
    public class VehicleCreateDto
    {
        [Range(1, int.MaxValue)]
        public int VehicleTypeId { get; set; }

        [Required]
        [MaxLength(50)]
        public string RegistrationNumber { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Make { get; set; }

        [MaxLength(100)]
        public string? Model { get; set; }

        [MaxLength(100)]
        public string? ChassisNumber { get; set; }

        [MaxLength(100)]
        public string? EngineNumber { get; set; }

        public int? Year { get; set; }

        public decimal Capacity { get; set; }

        public decimal CurrentMileage { get; set; }

        [EnumDataType(typeof(VehicleStatus))]
        public VehicleStatus Status { get; set; } = VehicleStatus.Active;

        [Range(1, int.MaxValue)]
        public int OfficeId { get; set; }
    }
}