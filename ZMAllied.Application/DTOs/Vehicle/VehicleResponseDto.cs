using System;
using ZMAllied.Domain.Enums;

namespace ZMAllied.Application.DTOs.Vehicle
{
    public class VehicleResponseDto
    {
        public int Id { get; set; }
        public int VehicleTypeId { get; set; }
        public string RegistrationNumber { get; set; } = string.Empty;
        public string? Make { get; set; }
        public string? Model { get; set; }
        public string? ChassisNumber { get; set; }
        public string? EngineNumber { get; set; }
        public int? Year { get; set; }
        public decimal Capacity { get; set; }
        public decimal CurrentMileage { get; set; }
        public VehicleStatus Status { get; set; }
        public int OfficeId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
    }
}