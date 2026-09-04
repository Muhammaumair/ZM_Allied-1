using ZMAllied.Domain.Enums;

namespace ZMAllied.Application.DTOs.Bilty
{
    public class BiltyResponseDto
    {
        public int Id { get; set; }
        public int? TripId { get; set; }
        public int OfficeId { get; set; }
        public string BiltyNumber { get; set; } = string.Empty;
        public DateTime BiltyDate { get; set; }
        public int? ConsignorId { get; set; }
        public int? ConsigneeId { get; set; }
        public int? BrokerId { get; set; }
        public int VehicleId { get; set; }
        public int DriverId { get; set; }
        public string? LoadingPoint { get; set; }
        public string? OffloadingPoint { get; set; }
        public string? PO_Number { get; set; }
        public decimal Freight { get; set; }
        public decimal Advance { get; set; }
        public decimal Balance { get; set; }
        public string? PaymentTerms { get; set; }
        public BiltyStatus Status { get; set; }
        public string? Remarks { get; set; }
        public List<BiltyItemResponseDto> Items { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
    }
}