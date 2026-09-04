using System;
using ZMAllied.Domain.Enums;

namespace ZMAllied.Application.DTOs.Driver
{
    public class DriverResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? CNIC { get; set; }
        public string? Phone { get; set; }
        public string? LicenseNumber { get; set; }
        public DateTime? LicenseExpiryDate { get; set; }
        public string? Address { get; set; }
        public DateTime? JoiningDate { get; set; }
        public VehicleStatus Status { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
    }
}