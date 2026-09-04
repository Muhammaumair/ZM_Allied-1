using System.ComponentModel.DataAnnotations;

namespace ZMAllied.Application.DTOs.VehicleMaintenance
{
    public class VehicleMaintenanceCreateDto
    {
        [Range(1, int.MaxValue)]
        public int VehicleId { get; set; }

        public DateTime MaintenanceDate { get; set; }

        [MaxLength(100)]
        public string? MaintenanceType { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public decimal Mileage { get; set; }

        public decimal Cost { get; set; }

        public int? SupplierId { get; set; }

        public DateTime? NextMaintenanceDate { get; set; }

        [MaxLength(500)]
        public string? Remarks { get; set; }
    }

    public class VehicleMaintenanceUpdateDto : VehicleMaintenanceCreateDto
    {
    }

    public class VehicleMaintenanceResponseDto
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public DateTime MaintenanceDate { get; set; }
        public string? MaintenanceType { get; set; }
        public string? Description { get; set; }
        public decimal Mileage { get; set; }
        public decimal Cost { get; set; }
        public int? SupplierId { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
        public string? Remarks { get; set; }
    }
}