using ZMAllied.Domain.Enums;

namespace ZMAllied.Application.DTOs.Payment
{
    public class PaymentResponseDto
    {
        public int Id { get; set; }
        public string PaymentNumber { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int? PartyId { get; set; }
        public int? SupplierId { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Description { get; set; }
        public PaymentStatus Status { get; set; }
        public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}