using System.ComponentModel.DataAnnotations;
using ZMAllied.Domain.Enums;

namespace ZMAllied.Application.DTOs.Payment
{
    public class PaymentCreateDto
    {
        [Required]
        [MaxLength(50)]
        public string PaymentNumber { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public int? PartyId { get; set; }

        public int? SupplierId { get; set; }

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Amount { get; set; }

        [MaxLength(100)]
        public string? PaymentMethod { get; set; }

        [MaxLength(100)]
        public string? ReferenceNumber { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [EnumDataType(typeof(PaymentStatus))]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        [MaxLength(500)]
        public string? Remarks { get; set; }
    }
}