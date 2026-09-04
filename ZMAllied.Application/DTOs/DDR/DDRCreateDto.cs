using System.ComponentModel.DataAnnotations;

namespace ZMAllied.Application.DTOs.DDR
{
    public class DDRCreateDto
    {
        public DateTime Date { get; set; }

        [Range(1, int.MaxValue)]
        public int CompanyId { get; set; }

        public int? BiltyId { get; set; }

        [Range(1, int.MaxValue)]
        public int VehicleId { get; set; }

        [Range(typeof(decimal), "0.001", "79228162514264337593543950335")]
        public decimal Weight { get; set; }

        [MaxLength(100)]
        public string? Lot { get; set; }

        [MaxLength(200)]
        public string? Item { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal? Rent { get; set; }

        [MaxLength(20)]
        public string? MobileNumber { get; set; }

        [MaxLength(200)]
        public string? LoadingPoint { get; set; }

        [MaxLength(200)]
        public string? OffloadingPoint { get; set; }

        public int? BrokerId { get; set; }

        public int? PartyId { get; set; }

        [MaxLength(500)]
        public string? Remarks { get; set; }
    }
}
