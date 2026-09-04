using System.ComponentModel.DataAnnotations;
using ZMAllied.Domain.Enums;

namespace ZMAllied.Application.DTOs.Trip
{
    public class TripCreateDto
    {
        [Required]
        [MaxLength(50)]
        public string TripNumber { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int CompanyId { get; set; }

        [Range(1, int.MaxValue)]
        public int PartyId { get; set; }

        [Range(1, int.MaxValue)]
        public int VehicleId { get; set; }

        [Range(1, int.MaxValue)]
        public int DriverId { get; set; }

        public DateTime TripDate { get; set; }

        [MaxLength(200)]
        public string? LoadingPoint { get; set; }

        [MaxLength(200)]
        public string? OffloadingPoint { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [EnumDataType(typeof(TripStatus))]
        public TripStatus Status { get; set; } = TripStatus.Draft;

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal TotalFreight { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Advance { get; set; }

        [MaxLength(500)]
        public string? Remarks { get; set; }
    }
}