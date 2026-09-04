using System.ComponentModel.DataAnnotations;
using ZMAllied.Domain.Enums;

namespace ZMAllied.Application.DTOs.Bilty
{
    public class BiltyCreateDto
    {
        [Range(1, int.MaxValue)]
        public int TripId { get; set; }

        [Range(1, int.MaxValue)]
        public int OfficeId { get; set; }

        [Required]
        [MaxLength(50)]
        public string BiltyNumber { get; set; } = string.Empty;

        public DateTime BiltyDate { get; set; }

        [Range(1, int.MaxValue)]
        public int ConsignorId { get; set; }

        [Range(1, int.MaxValue)]
        public int ConsigneeId { get; set; }

        public int? BrokerId { get; set; }

        [Range(1, int.MaxValue)]
        public int VehicleId { get; set; }

        [Range(1, int.MaxValue)]
        public int DriverId { get; set; }

        [MaxLength(200)]
        public string? LoadingPoint { get; set; }

        [MaxLength(200)]
        public string? OffloadingPoint { get; set; }

        [MaxLength(100)]
        public string? PO_Number { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Freight { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Advance { get; set; }

        [MaxLength(200)]
        public string? PaymentTerms { get; set; }

        [EnumDataType(typeof(BiltyStatus))]
        public BiltyStatus Status { get; set; } = BiltyStatus.Draft;

        [MaxLength(500)]
        public string? Remarks { get; set; }

        [Required]
        [MinLength(1)]
        public List<BiltyItemCreateDto> Items { get; set; } = new();
    }
}