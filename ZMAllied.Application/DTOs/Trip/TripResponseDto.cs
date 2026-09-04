using ZMAllied.Domain.Enums;

namespace ZMAllied.Application.DTOs.Trip
{
    public class TripResponseDto
    {
        public int Id { get; set; }
        public string TripNumber { get; set; } = string.Empty;
        public int CompanyId { get; set; }
        public int PartyId { get; set; }
        public int VehicleId { get; set; }
        public int DriverId { get; set; }
        public DateTime TripDate { get; set; }
        public string? LoadingPoint { get; set; }
        public string? OffloadingPoint { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TripStatus Status { get; set; }
        public decimal TotalFreight { get; set; }
        public decimal Advance { get; set; }
        public decimal Balance { get; set; }
        public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
    }
}