using System;
using System.ComponentModel.DataAnnotations;
using ZMAllied.Domain.Enums;

namespace ZMAllied.Application.DTOs.Driver
{
    public class DriverUpdateDto
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? CNIC { get; set; }

        [Phone]
        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(50)]
        public string? LicenseNumber { get; set; }

        public DateTime? LicenseExpiryDate { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        public DateTime? JoiningDate { get; set; }

        [EnumDataType(typeof(VehicleStatus))]
        public VehicleStatus Status { get; set; }

        public bool IsActive { get; set; }
    }
}